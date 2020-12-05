using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cartomatic.Utils.Data;

namespace MapHive.Core.DataModel.Map
{
    public partial class DataStore
    {
        /// <summary>
        /// Processes a shp file and uploads it to a db
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <param name="path">Path to a directory a shapefile has been uploaded to</param>
        /// <param name="dsc"></param>
        /// <returns></returns>
        public static async Task<DataStore> ProcessShp(DbContext dbCtx, string path, DataSourceCredentials dsc)
        {
            //assuming a single zip can only be present in a directory, as uploading data for a single layer

            //if there is a zip archive, need to extract it
            ExtractZip(path);


            //test for required shp format files presence...
            var shp = string.Empty;
            ValidateShpFilePresence(path, out shp);


            var fName = Path.GetFileNameWithoutExtension(shp);

            var output = GetDataStore(fName, "shp", dsc);


            //need this for a proper code page handling when reading dbf
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            //looks like it should be possible to read shp now...
            using (var shpReader = new NetTopologySuite.IO.ShapefileDataReader(shp, new GeometryFactory()))
            {
                ExtractShpDataBbox(shpReader, output);

                ExtractShpColumns(shpReader, output);

                //create object straight away, so when something goes wrong with import, etc. there is a chance for a cleanup bot
                //to pick it up and cleanup the orphaned data when necessary
                await output.CreateAsync(dbCtx);

                await ReadAndLoadShpData(shpReader, output);
            }


            return output;
        }

        /// <summary>
        /// Validates presence of all the required shp format files
        /// </summary>
        /// <param name="path"></param>
        public static void ValidateShpFilePresence(string path, out string shpPath)
        {
            var shp = Directory.GetFiles(path, "*.shp").FirstOrDefault();
            if (string.IsNullOrEmpty(shp))
                throw MapHive.Core.DataModel.Validation.Utils.GenerateValidationFailedException("SHP", "no_shp",
                    "SHP file has not been found");

            var fName = Path.GetFileNameWithoutExtension(shp);

            var shx = Directory.GetFiles(path, $"{fName}.shx").FirstOrDefault();
            if (string.IsNullOrEmpty(shx))
                throw MapHive.Core.DataModel.Validation.Utils.GenerateValidationFailedException("SHX", "no_shx",
                    "SHX file has not been found");

            var dbf = Directory.GetFiles(path, $"{fName}.dbf").FirstOrDefault();
            if (string.IsNullOrEmpty(dbf))
                throw MapHive.Core.DataModel.Validation.Utils.GenerateValidationFailedException("DBF", "no_dbf",
                    "DBF file has not been found");

            shpPath = shp;
        }

        /// <summary>
        /// Extracts shp data bbox and pushes data to data store
        /// </summary>
        /// <param name="shp"></param>
        /// <param name="dataStore"></param>
        public static void ExtractShpDataBbox(string shp, DataStore dataStore)
        {
            //need this for a proper code page handling when reading dbf
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var shpReader = new NetTopologySuite.IO.ShapefileDataReader(shp, new GeometryFactory()))
            {
                ExtractShpDataBbox(shpReader, dataStore);
            }
        }

        /// <summary>
        /// Extracts shp data bbox and pushes data to data store
        /// </summary> 
        /// <param name="shpReader"></param>
        /// <param name="dataStore"></param>
        public static void ExtractShpDataBbox(NetTopologySuite.IO.ShapefileDataReader shpReader, DataStore dataStore)
        {
            var shpHdr = shpReader.ShapeHeader;

            //grab bbox
            dataStore.MinX = shpHdr.Bounds.MinX;
            dataStore.MinY = shpHdr.Bounds.MinY;
            dataStore.MaxX = shpHdr.Bounds.MaxX;
            dataStore.MaxY = shpHdr.Bounds.MaxY;
        }

        /// <summary>
        /// Extracts shp columns and pushes data to data store
        /// </summary>
        /// <param name="shp"></param>
        /// <param name="dataStore"></param>
        public static void ExtractShpColumns(string shp, DataStore dataStore)
        {
            //need this for a proper code page handling when reading dbf
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var shpReader = new NetTopologySuite.IO.ShapefileDataReader(shp, new GeometryFactory()))
            {
                ExtractShpColumns(shpReader, dataStore);
            }
        }


        /// <summary>
        /// Extracts shp columns and pushes data to data store
        /// </summary>
        /// <param name="shpReader"></param>
        /// <param name="dataStore"></param>
        public static void ExtractShpColumns(NetTopologySuite.IO.ShapefileDataReader shpReader, DataStore dataStore)
        {
            var dBaseHdr = shpReader.DbaseHeader;

            foreach (var dbaseFieldDescriptor in dBaseHdr.Fields)
            {
                if (!CheckIfObjectSafe(dbaseFieldDescriptor.Name))
                    throw MapHive.Core.DataModel.Validation.Utils.GenerateValidationFailedException("ColName", "bad_col_name",
                        "Column name contains forbidden words");

                var colType = SystemTypeToColumnDataType(dbaseFieldDescriptor.Type);
                if (colType != ColumnDataType.Unknown)
                {
                    dataStore.DataSource.Columns.Add(new Column
                    {
                        Type = colType,
                        Name = GetSafeDbObjectName(dbaseFieldDescriptor.Name),
                        FriendlyName = dbaseFieldDescriptor.Name
                    });
                }
            }

            //since reading shp, there is also a geometry column
            dataStore.DataSource.Columns.Add(new Column
            {
                Type = ColumnDataType.Geom,
                Name = "geom",
                FriendlyName = "Geom"
            });
        }

        /// <summary>
        /// Reads shp data and loads it into db
        /// </summary>
        /// <param name="shp"></param>
        /// <param name="dataStore"></param>
        public static async Task ReadAndLoadShpData(string shp, DataStore dataStore)
        {
            //need this for a proper code page handling when reading dbf
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var shpReader = new NetTopologySuite.IO.ShapefileDataReader(shp, new GeometryFactory()))
            {
                await ReadAndLoadShpData(shpReader, dataStore);
            }
        }

        /// <summary>
        /// Reads shp data and updates a table
        /// </summary>
        /// <param name="shp"></param>
        /// <param name="dataStoreToUpdate"></param>
        /// <param name="newDataStore"></param>
        /// <param name="updateMode"></param>
        /// <returns></returns>
        public static async Task ReadAndUpdateShpData(string shp, DataStore dataStoreToUpdate, DataStore newDataStore, string updateMode, IEnumerable<string> key = null)
        {
            if (updateMode == "overwrite")
                await ExecuteTableDropAsync(dataStoreToUpdate);

            var upsert = updateMode == "upsert";

            //need this for a proper code page handling when reading dbf
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var shpReader = new NetTopologySuite.IO.ShapefileDataReader(shp, new GeometryFactory()))
            {
                //with upsert need to use the basic store as the sql generator, otherwise the new one
                await ReadAndLoadShpData(shpReader, upsert ? dataStoreToUpdate : newDataStore, upsert, key);
            }
        }

        /// <summary>
        /// Reads & loads shp data
        /// </summary>
        /// <param name="shpReader"></param>
        /// <param name="dataStore"></param>
        /// <param name="upsert">Whether or not should perform upsert rather than insert</param>
        /// <returns></returns>
        protected static async Task ReadAndLoadShpData(NetTopologySuite.IO.ShapefileDataReader shpReader, DataStore dataStore, bool upsert = false, IEnumerable<string> key = null)
        {
            var dBaseHdr = shpReader.DbaseHeader;


            using (var conn = new NpgsqlConnection(dataStore.DataSource.DataSourceCredentials.GetConnectionString()))
            using (var cmd = new NpgsqlCommand())
            {
                await conn.OpenAsync();
                cmd.Connection = conn;

                //need db objects for new data
                if (!upsert)
                {
                    cmd.CommandText = GetEnsureSchemaSql(dataStore);
                    await cmd.ExecuteNonQueryAsync();

                    cmd.CommandText = GetCreateTableSql(dataStore);
                    await cmd.ExecuteNonQueryAsync();
                }



                //table ready, so pump in the data
                //for the time being assume the geoms to be in EPSG:3857
                //TODO - make the projection dynamic!!!

                var batchSize = 25;
                var processed = 0;
                var insertData = new List<string>(batchSize);
                List<string> skipCols = null;

                //in upsert mode can only upsert columns that are in the original data store
                //therefore need to know what columns skip on conflict, when performing update
                //so if a column does not appear in the shp, then needs to be excluded
                if (upsert)
                {
                    skipCols = dataStore.DataSource.Columns.Where(c => c.Type != ColumnDataType.Geom && dBaseHdr.Fields.All(f => GetSafeDbObjectName(f.Name) != c.Name)).Select(c => c.Name).ToList();
                }

                //in upsert mode need column in the order of data store!!!
                var dBaseFieldsToRead = dBaseHdr.Fields.Select((fld, idx) => (idx, fld)).ToList();
                if (upsert)
                {
                    dBaseFieldsToRead.Clear();

                    foreach (var col in dataStore.DataSource.Columns)
                    {
                        if (skipCols != null && !skipCols.Contains(col.Name))
                        {
                            for (var f = 0; f < dBaseHdr.Fields.Length; f++)
                            {
                                var fld = dBaseHdr.Fields[f];
                                if (GetSafeDbObjectName(fld.Name) == col.Name)
                                {
                                    dBaseFieldsToRead.Add((f, fld));
                                }
                            }
                        }
                    }
                }

                //col count for the upsert
                var colCnt = dBaseFieldsToRead.Count + 1; //+1 to account for geom!

                while (shpReader.Read())
                {
                    if (processed > 0 && processed % batchSize == 0)
                    {
                        if (upsert)
                            await ExecuteUpsertBatch(cmd, dataStore, insertData, colCnt, key, skipCols);
                        else
                            await ExecuteInsertBatch(cmd, dataStore, insertData);
                    }

                    processed += 1;


                    var data = new string[dBaseFieldsToRead.Count + 1];
                    
                   
                    for (var i = 0; i < dBaseFieldsToRead.Count; i++)
                    {
                        var safeFldName = GetSafeDbObjectName(dBaseFieldsToRead[i].fld.Name);

                        //in upsert mode the original data model does not change, therefore should only update
                        //columns that are in the original model ignoring all the extra ones
                        if (upsert && dataStore.DataSource.Columns.All(c => c.Name != safeFldName))
                            continue;

                        data[i] = $"@r{processed}_p{i}";

                        var col = dataStore.DataSource.Columns.FirstOrDefault(c => c.Name == safeFldName);
                        

                        var value = shpReader.GetValue(dBaseFieldsToRead[i].idx + 1); //shp reads by 1 based index
                        var valueType = value.GetType();

                        if (valueType == typeof(string) && dBaseFieldsToRead[i].fld.Type != valueType)
                        {
                            if (value.ToString().Contains("***") || value.ToString().Contains("*")) //this indicates a null in shp field)
                            {
                                cmd.Parameters.Add(data[i], ColumnDataTypeToNpgsqlDbType(col.Type)).Value = DBNull.Value;
                            }

                            //not using default values, nulls should do just fine
                            //if (string.IsNullOrWhiteSpace(value.ToString()))
                            //{
                            //    var defltValue = Activator.CreateInstance(dBaseFieldsToRead[i].fld.Type);
                            //    cmd.Parameters.Add(data[i], ColumnDataTypeToNpgsqlDbType(col.Type)).Value = defltValue;
                            //}
                        }
                        else
                        {
                            //cmd.Parameters.AddWithValue(data[i], value);
                            cmd.Parameters.Add(data[i], ColumnDataTypeToNpgsqlDbType(col.Type)).Value = value;
                        }
                    }

                    //geom
                    var geomPName = $"@r{processed}_p{dBaseFieldsToRead.Count}";
                    data[^1] = $"ST_SetSRID(ST_GeomFromText({geomPName}),3857)";
                    cmd.Parameters.AddWithValue(geomPName, shpReader.Geometry.AsText());
                    //assume geom always in spherical mercator!

                    //TODO - dynamic projection!

                    //note: since col indexing is per dbaseHdr, some cols may be empty. need to discard them when generating sql!
                    insertData.Add($"SELECT {string.Join(",", data)}");
                }

                if (insertData.Count > 0)
                {
                    if (upsert)
                        await ExecuteUpsertBatch(cmd, dataStore, insertData, colCnt, key, skipCols);
                    else
                        await ExecuteInsertBatch(cmd, dataStore, insertData);
                }

                //no point in creating indexes in upsert mode, as table has been indexed before
                if (upsert)
                    return;

                //when ready, index the geom col, so geom reads are quicker
                cmd.CommandText = GetCreateGeomIdxSql(dataStore);
                await cmd.ExecuteNonQueryAsync();

                //this should not change much really, so leaving the bbox as read from file
                //await CalculateBBox(cmd, output);

                //location col indexing as required
                await CreateLocationIndex(cmd, dataStore);

            }
        }

    }
}
