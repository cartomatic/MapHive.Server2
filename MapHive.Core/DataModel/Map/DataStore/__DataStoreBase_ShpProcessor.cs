using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MapHive.Core.DataModel.Map
{
    public abstract partial class DataStoreBase
    {
        /// <summary>
        /// Processes a shp file and uploads it to a db
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <param name="path">Path to a directory a shapefile has been uploaded to</param>
        /// <returns></returns>
        public static async Task<DataStoreBase> ProcessShp(DbContext dbCtx, string path)
        {
            //assuming a single zip can only be present in a directory, as uploading data for a single layer

            //if there is a zip archive, need to extract it
            ExtractZip(path);


            //test for required shp format files presence...
            var shp = string.Empty;
            ValidateShpFilePresence(path, out shp);


            var fName = Path.GetFileNameWithoutExtension(shp);

            var output = GetDataStore(fName, "shp");


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
        protected static void ValidateShpFilePresence(string path, out string shpPath)
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
        protected static void ExtractShpDataBbox(string shp, DataStoreBase dataStore)
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
        protected static void ExtractShpDataBbox(NetTopologySuite.IO.ShapefileDataReader shpReader, DataStoreBase dataStore)
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
        protected static void ExtractShpColumns(string shp, DataStoreBase dataStore)
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
        protected static void ExtractShpColumns(NetTopologySuite.IO.ShapefileDataReader shpReader, DataStoreBase dataStore)
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
        protected static async Task ReadAndLoadShpData(string shp, DataStoreBase dataStore)
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
        protected static async Task ReadAndUpdateShpData(string shp, DataStoreBase dataStoreToUpdate, DataStoreBase newDataStore, string updateMode, IEnumerable<string> key = null)
        {
            if (updateMode == "overwrite")
                await ExecuteTableDropAsync(dataStoreToUpdate);


            //need this for a proper code page handling when reading dbf
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var shpReader = new NetTopologySuite.IO.ShapefileDataReader(shp, new GeometryFactory()))
            {
                await ReadAndLoadShpData(shpReader, newDataStore, updateMode == "upsert", key);
            }
        }

        /// <summary>
        /// Reads & loads shp data
        /// </summary>
        /// <param name="shpReader"></param>
        /// <param name="dataStore"></param>
        /// <param name="upsert">Whether or not should perform upsert rather than insert</param>
        /// <returns></returns>
        protected static async Task ReadAndLoadShpData(NetTopologySuite.IO.ShapefileDataReader shpReader, DataStoreBase dataStore, bool upsert = false, IEnumerable<string> key = null)
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
                    skipCols = dataStore.DataSource.Columns.Where(c => c.Type != ColumnDataType.Geom && dBaseHdr.Fields.All(f => f.Name != c.Name)).Select(c => c.Name).ToList();
                }

                while (shpReader.Read())
                {
                    if (processed > 0 && processed % batchSize == 0)
                    {
                        if (upsert)
                            await ExecuteUpsertBatch(cmd, dataStore, insertData, key, skipCols);
                        else
                            await ExecuteInsertBatch(cmd, dataStore, insertData);
                    }

                    processed += 1;

                    var data = new string[dBaseHdr.Fields.Length + 1];
                    for (var i = 0; i < dBaseHdr.Fields.Length; i++)
                    {
                        //in upsert mode the original data model does not change, therefore should only update
                        //columns that are in the original model ignoring all the extra ones
                        if (upsert && dataStore.DataSource.Columns.All(c => c.Name != dBaseHdr.Fields[i].Name))
                            continue;

                        data[i] = $"@r{processed}_p{i}";

                        var value = shpReader.GetValue(i + 1);
                        var valueType = value.GetType();

                        if (valueType == typeof(string) && dBaseHdr.Fields[i].Type != valueType &&
                            value.ToString().Contains("***")) //this indicates a null in shp field
                        {
                            cmd.Parameters.AddWithValue(data[i], DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue(data[i], value);
                        }
                    }

                    //geom
                    data[data.Length - 1] =
                        $"ST_SetSRID(ST_GeomFromText('{shpReader.Geometry.AsText()}'),3857)"; //assume geom always in spherical mercator!
                    //TODO - dynamic projection!

                    insertData.Add($"SELECT {string.Join(",", data)}");
                }

                if (insertData.Count > 0)
                {
                    if (upsert)
                        await ExecuteUpsertBatch(cmd, dataStore, insertData, key, skipCols);
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
