using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MapHive.Core.DataModel.Map
{
    public partial class DataStore
    {
        /// <summary>
        /// Processes a flat json file; file has got to have numeric lon / longitude & lat / latitude properties; coords are assumed to be in lon/lat
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<DataStore> ProcessJson(DbContext dbCtx, string path)
        {
            //assuming a single zip can only be present in a directory, as uploading data for a single layer

            //if there is a zip archive, need to extract it
            ExtractZip(path);


            //test for required shp format files presence...
            var file = Directory.GetFiles(path, "*.json").FirstOrDefault();
            if (string.IsNullOrEmpty(file))
                throw MapHive.Core.DataModel.Validation.Utils.GenerateValidationFailedException("JSON", "no_json_file",
                    "JSON file has not been found");

            var fName = Path.GetFileNameWithoutExtension(file);

            var output = GetDataStore(fName, "json");


            var json = JsonConvert.DeserializeObject(File.ReadAllText(file));
            if (json.GetType() != typeof(JArray))
                throw MapHive.Core.DataModel.Validation.Utils.GenerateValidationFailedException("JSON", "not_array",
                    "JSON file has not been deserialized to array");

            var data = new List<Dictionary<string, object>>(((JArray)json).Count);

            foreach (JObject jRec in (JArray)json)
            {
                var rec = new Dictionary<string, object>();

                foreach (var jProp in jRec.Properties())
                {
                    var propName = jProp.Name.ToLower();

                    //basically looking at a flat json file, BUT allowing convenience parsing for some nested properties
                    if (FlatLonProps.Contains(propName))
                    {
                        rec.Add("lo", GetValueFromJsonProperty(jProp.Value));
                    }
                    else if (FlatLatProps.Contains(propName))
                    {
                        rec.Add("la", GetValueFromJsonProperty(jProp.Value));
                    }
                    else if (FlatGeoLocationProps.Contains(propName))
                    {
                        if (TryExtractLoLaFromJsonObject((JObject)jProp.Value, out var lo, out var la))
                        {
                            rec.Add("lo", lo);
                            rec.Add("la", la);
                        };
                    }
                    else if (FlatGeomProps.Contains(propName))
                    {
                        rec.Add("wkt", GetValueFromJsonProperty(jProp.Value));
                    }
                    else
                    {
                        rec.Add(jProp.Name, GetValueFromJsonProperty(jProp.Value));
                    }
                }

                if (rec.ContainsKey("lo") && rec.ContainsKey("la") || rec.ContainsKey("wkt"))
                    data.Add(rec);
            }


            //work out a data model - this is json, so can be totally unpredictable
            foreach (var rec in data)
            {
                foreach (var fProp in rec)
                {
                    //ignore lon/lat, this will be turned into a point
                    //testing for lon/lat only as data is already normalized
                    if (fProp.Key == "lo" || fProp.Key == "la" || fProp.Key == "wkt")
                        continue;

                    if (output.DataSource.Columns.Any(c => c.Name == GetSafeDbObjectName(fProp.Key)))
                        continue;

                    if (!CheckIfObjectSafe(fProp.Key))
                        throw MapHive.Core.DataModel.Validation.Utils.GenerateValidationFailedException("ColName", "bad_col_name",
                            "Column name contains forbidden words");

                    var colType = SystemTypeToColumnDataType(fProp.Value.GetType());
                    if (colType != ColumnDataType.Unknown)
                    {
                        output.DataSource.Columns.Add(new Column
                        {
                            Type = colType,
                            Name = GetSafeDbObjectName(fProp.Key),
                            FriendlyName = fProp.Key
                        });
                    }
                }
            }

            //create object straight away, so when something goes wrong with import, etc. there is a chance for a cleanup bot
            //to pick it up and cleanup the orphaned data when necessary
            await output.CreateAsync(dbCtx);

            return await ProcessFlatData(dbCtx, output, data);
        }


        /// <summary>
        /// Processes a flat csv file; file has got to have numeric lon / longitude & lat / latitude properties; coords are assumed to be in lon/lat
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<DataStore> ProcessCsv(DbContext dbCtx, string path)
        {
            //assuming a single zip can only be present in a directory, as uploading data for a single layer

            //if there is a zip archive, need to extract it
            var zip = Directory.GetFiles(path, "*.zip").FirstOrDefault();
            if (!string.IsNullOrEmpty(zip))
            {
                System.IO.Compression.ZipFile.ExtractToDirectory(zip, path);
            }


            //test for required shp format files presence...
            var file = Directory.GetFiles(path, "*.csv").FirstOrDefault();
            if (string.IsNullOrEmpty(file))
                throw MapHive.Core.DataModel.Validation.Utils.GenerateValidationFailedException("CSV", "no_csv_file",
                    "CSV file has not been found");

            var fName = Path.GetFileNameWithoutExtension(file);

            var output = GetDataStore(fName, "csv");

            ExtractCsvColumns(file, output);

            var data = ExtractCsvData(file, output);

            //create object straight away, so when something goes wrong with import, etc. there is a chance for a cleanup bot
            //to pick it up and cleanup the orphaned data when necessary
            await output.CreateAsync(dbCtx);

            return await ProcessFlatData(dbCtx, output, data);
        }

        /// <summary>
        /// Extracts csv columns
        /// </summary>
        /// <param name="file"></param>
        /// <param name="dataStore"></param>
        /// <param name="delimiter"></param>
        /// <param name="hasHeader"></param>
        /// <param name="colNamesRemap">A dictionary used to remap column names to required names</param>
        /// <param name="colTypesMap">map of safe column names to column types to enforce a specified csv data parsing</param>
        protected static void ExtractCsvColumns(
            string file,
            DataStore dataStore,
            string delimiter = ";",
            bool hasHeader = true,
            Dictionary<string, string> colNamesRemap = null,
            Dictionary<string, ColumnDataType> colTypesMap = null
        )
        {
            using (var rdr = new StreamReader(file))
            using (var csvRdr = new CsvReader(rdr))
            {
                csvRdr.Configuration.Delimiter = delimiter;
                csvRdr.Configuration.HasHeaderRecord = hasHeader;

                if (hasHeader)
                {
                    csvRdr.Read();
                }
                csvRdr.ReadHeader();


                foreach (var colName in csvRdr.Context.HeaderRecord)
                {
                    if (FlatLonProps.Contains(colName) || FlatLatProps.Contains(colName))
                        continue;

                    if (!CheckIfObjectSafe(colName))
                        throw MapHive.Core.DataModel.Validation.Utils.GenerateValidationFailedException("ColName", "bad_col_name",
                            "Column name contains forbidden words");

                    var safeColName = GetSafeDbObjectName(colName);

                    if (colNamesRemap?.ContainsKey(safeColName) == true)
                    {
                        safeColName = colNamesRemap[safeColName];
                    }

                    dataStore.DataSource.Columns.Add(new Column
                    {
                        Type = colTypesMap?.ContainsKey(safeColName) == true ? colTypesMap[safeColName] : ColumnDataType.String,
                        Name = safeColName,
                        FriendlyName = colName
                    });
                }
            }
        }

        /// <summary>
        /// Extracts data off a csv file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="dataStore"></param>
        /// <param name="delimiter"></param>
        /// <param name="hasHeader"></param>
        /// <returns></returns>
        protected static List<Dictionary<string, object>> ExtractCsvData(string file, DataStore dataStore, string delimiter = ";", bool hasHeader = true, bool hasGeo = true)
        {
            var data = new List<Dictionary<string, object>>();

            using (var rdr = new StreamReader(file))
            using (var csvRdr = new CsvReader(rdr))
            {
                csvRdr.Configuration.Delimiter = delimiter;
                csvRdr.Configuration.HasHeaderRecord = hasHeader;

                if (hasHeader)
                {
                    csvRdr.Read();
                }
                csvRdr.ReadHeader();


                //create col names map from data store, so can match csv header col names to safe db col names
                var colNamesMap = new Dictionary<string, string>();
                var colTypesMap = new Dictionary<string, ColumnDataType>();
                foreach (var c in dataStore.DataSource.Columns)
                {
                    colNamesMap[c.FriendlyName] = c.Name;
                    colTypesMap[c.Name] = c.Type;
                }

                //data 
                while (csvRdr.Read())
                {
                    var rec = new Dictionary<string, object>();
                    foreach (var colName in csvRdr.Context.HeaderRecord)
                    {
                        var cName = string.Empty;
                        if (FlatLonProps.Contains(colName))
                            cName = "lo";
                        else if (FlatLatProps.Contains(colName))
                            cName = "la";
                        else if (FlatGeomProps.Contains(colName))
                            cName = "wkt";
                        else
                            cName = colNamesMap[colName];

                        rec.Add(cName, ParseToColumnDataType(csvRdr.GetField(colName), colTypesMap.ContainsKey(cName) ? colTypesMap[cName] : ColumnDataType.String));
                    }

                    if (hasGeo && !(rec.ContainsKey("lo") && rec.ContainsKey("la") || rec.ContainsKey("wkt")))
                        throw MapHive.Core.DataModel.Validation.Utils.GenerateValidationFailedException("FlatData", "no_geo",
                            "Flat file does not contain any spatial information (usually lon / lat)");

                    data.Add(rec);
                }
            }

            return data;
        }


        protected static object GetValueFromJsonProperty(JToken t)
        {
            switch (t.Type)
            {
                case JTokenType.Boolean:
                    return t.Value<bool>();
                case JTokenType.Guid:
                    return t.Value<Guid>();
                case JTokenType.Date:
                    return t.Value<DateTime>();
                case JTokenType.Float:
                    return t.Value<float>();
                case JTokenType.Integer:
                    return t.Value<int>();
                case JTokenType.String:
                    return t.Value<string>();
                default:
                    return null;
            }
        }

        /// <summary>
        /// Extracts lo/la from a json object
        /// </summary>
        /// <param name="jObj"></param>
        /// <param name="lo"></param>
        /// <param name="la"></param>
        /// <returns></returns>
        protected static bool TryExtractLoLaFromJsonObject(JObject jObj, out object lo, out object la)
        {
            lo = null;
            la = null;

            var jProps = jObj.Properties();
            var loProp = jProps.FirstOrDefault(p => FlatLonProps.Contains(p.Name.ToLower()));
            if (loProp != null)
                lo = loProp.Value;

            var laProp = jProps.FirstOrDefault(p => FlatLatProps.Contains(p.Name.ToLower()));
            if (laProp != null)
                la = laProp.Value;

            if (lo == null && la == null)
            {
                var locationProp = jProps.FirstOrDefault(p => FlatGeoLocationProps.Contains(p.Name.ToLower()));
                if (locationProp != null)
                {
                    if (locationProp.Value is JArray array && array.Count == 2)
                    {
                        lo = array.First.Value<double>();
                        la = array.Last.Value<double>();
                    }
                    else
                    {
                        TryExtractLoLaFromJsonObject((JObject)locationProp.Value, out lo, out la);
                    }
                }
            }

            return lo != null && la != null;
        }


        /// <summary>
        /// Longitude props
        /// </summary>
        private static readonly string[] FlatLonProps = new[] { "lo", "lon", "longitude" };

        /// <summary>
        /// Latitude props
        /// </summary>
        private static readonly string[] FlatLatProps = new[] { "la", "lat", "latitude" };

        /// <summary>
        /// location props
        /// </summary>
        private static readonly string[] FlatGeoLocationProps = new[] { "location", "geolocation", "coords", "coordinates" };

        /// <summary>
        /// geom props
        /// </summary>
        private static readonly string[] FlatGeomProps = new[] { "areageometryastext", "geom", "geometry", "wkt" };


        /// <summary>
        /// Processes flat data
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <param name="dataStore"></param>
        /// <param name="flatData"></param>
        /// <param name="hasGeom">When set to false allows skipping geo presence check for datasets without lo/la or geometry</param>
        /// <param name="upsert"></param>
        /// <param name="key">key to perform conflict testing when upserting</param>
        /// <returns></returns>
        protected static async Task<DataStore> ProcessFlatData(DbContext dbCtx, DataStore dataStore, List<Dictionary<string, object>> flatData, bool hasGeom = true, bool upsert = false, IEnumerable<string> key = null)
        {
            if (flatData == null || flatData.Count == 0)
                throw MapHive.Core.DataModel.Validation.Utils.GenerateValidationFailedException("FlatData", "no_data",
                    "Flat file could not be read");


            if (hasGeom)
            {
                var hasGeo =
                    flatData.First().Keys.Any(k => FlatLonProps.Contains(k)) &&
                    flatData.First().Keys.Any(k => FlatLatProps.Contains(k))
                    ||
                    flatData.First().Keys.Any(k => FlatGeomProps.Contains(k));

                if (!hasGeo)
                {
                    throw MapHive.Core.DataModel.Validation.Utils.GenerateValidationFailedException("FlatData", "no_geo",
                        "Flat file does not contain any spatial information (usually lon / lat)");
                }

                //Since reading spatial, there is also a geometry column
                dataStore.DataSource.Columns.Add(new Column
                {
                    Type = ColumnDataType.Geom,
                    Name = "geom",
                    FriendlyName = "Geom"
                });

            }


            using (var conn = new NpgsqlConnection(dataStore.DataSource.DataSourceCredentials.GetConnectionString()))
            using (var cmd = new NpgsqlCommand())
            {
                await conn.OpenAsync();
                cmd.Connection = conn;

                //create storage for new data
                if (!upsert)
                {
                    cmd.CommandText = GetEnsureSchemaSql(dataStore);
                    await cmd.ExecuteNonQueryAsync();

                    cmd.CommandText = GetCreateTableSql(dataStore);
                    await cmd.ExecuteNonQueryAsync();
                }



                //table ready, so pump in the data
                //assume the geoms to be in EPSG:4326 (GeoJSON :)

                var batchSize = 25;
                var processed = 0;
                var insertData = new List<string>(batchSize);
                List<string> skipCols = null;

                //in upsert mode can only upsert columns that are in the original data store
                //therefore need to know what columns skip on conflict, when performing update
                //so if a column does not have corresponding data in the incoming dataset it needs to be discarded
                if (upsert)
                {
                    var rec = flatData.First();
                    skipCols = dataStore.DataSource.Columns.Where(c => !rec.ContainsKey(c.Name)).Select(c => c.Name).ToList();
                }

                //prepare insert data...
                foreach (var rec in flatData)
                {
                    if (processed > 0 && processed % batchSize == 0)
                    {
                        if (upsert)
                            await ExecuteUpsertBatch(cmd, dataStore, insertData, key, skipCols);
                        else
                            await ExecuteInsertBatch(cmd, dataStore, insertData);
                    }

                    processed += 1;


                    var data = new string[dataStore.DataSource.Columns.Count]; //-1 for geom
                    var pIdx = 0;

                    foreach (var c in dataStore.DataSource.Columns)
                    {
                        //in upsert mode the original data model does not change, therefore should only update
                        //columns that are in the original model ignoring all the extra ones
                        if (upsert && !rec.ContainsKey(c.Name))
                            continue;

                        //geom cols handled specially
                        if (c.Type == ColumnDataType.Geom)
                            continue;

                        data[pIdx] = $"@r{processed}_p{pIdx}";

                        if (rec.ContainsKey(c.Name))
                        {
                            if (rec[c.Name] != null)
                            {
                                cmd.Parameters.AddWithValue(data[pIdx], rec[c.Name]);
                            }
                            else
                            {
                                cmd.Parameters.Add(GetDbNullValueParamForColumn(data[pIdx], c));
                            }
                        }
                        else
                        {
                            cmd.Parameters.Add(GetDbNullValueParamForColumn(data[pIdx], c));
                        }

                        pIdx += 1;
                    }

                    //geom
                    if (hasGeom)
                    {
                        //assume flat rec to be always in 4326 and transform it to spherical mercator!
                        if (
                            rec.ContainsKey("lo") &&
                            double.TryParse(rec["lo"].ToString().Replace(",", "."), NumberStyles.Any,
                                CultureInfo.InvariantCulture, out var lo) &&
                            rec.ContainsKey("la") &&
                            double.TryParse(rec["la"].ToString().Replace(",", "."), NumberStyles.Any,
                                CultureInfo.InvariantCulture, out var la)
                        )
                        {
                            data[data.Length - 1] =
                                $"ST_Transform(ST_SetSRID(ST_MakePoint({lo.ToString(CultureInfo.InvariantCulture)}, {la.ToString(CultureInfo.InvariantCulture)}),4326),3857)";
                        }
                        else if (rec.ContainsKey("wkt"))
                        {
                            data[data.Length - 1] =
                                $"ST_Transform(ST_SetSRID(ST_GeomFromText('{rec["wkt"]}'),4326),3857)";
                        }
                        else
                        {
                            data[data.Length - 1] = "NULL";
                        }
                    }

                    insertData.Add($"SELECT {string.Join(",", data)}");
                }


                if (insertData.Count > 0)
                {
                    if (upsert)
                        await ExecuteUpsertBatch(cmd, dataStore, insertData, key, skipCols);
                    else
                        await ExecuteInsertBatch(cmd, dataStore, insertData);
                }

                //not adding indexes in the upsert mode. they are already there for updated data store
                if (upsert)
                    return dataStore;

                if (hasGeom)
                {
                    //when ready, index the geom col, so geom reads are quicker
                    cmd.CommandText = GetCreateGeomIdxSql(dataStore);
                    await cmd.ExecuteNonQueryAsync();

                    //work out the actual bbox of data as imported

                    await CalculateBBox(cmd, dataStore);
                }

                //location col indexing as required
                await CreateLocationIndex(cmd, dataStore);
            }

            return dataStore;
        }

        /// <summary>
        /// Updates data in a flat data store
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <param name="dataStoreToUpdate"></param>
        /// <param name="newDataStore"></param>
        /// <param name="flatData"></param>
        /// <param name="updateMode"></param>
        /// <param name="hasGeom"></param>
        /// <returns></returns>
        protected static async Task<DataStore> UpdateFlatData(DbContext dbCtx, DataStore dataStoreToUpdate, DataStore newDataStore,
            List<Dictionary<string, object>> flatData, string updateMode, bool hasGeom = true, IEnumerable<string> key = null)
        {
            if (updateMode == "overwrite")
                await ExecuteTableDropAsync(dataStoreToUpdate);

            return await ProcessFlatData(dbCtx, newDataStore, flatData, hasGeom, upsert: updateMode == "upsert", key: key);
        }

        /// <summary>
        /// Extracts bbox from flat data 
        /// </summary>
        /// <param name="flatData"></param>
        /// <returns></returns>
        protected static (double? minX, double? minY, double? maxX, double? maxY) ExtractFlatRecBbox(List<Dictionary<string, object>> flatData)
        {
            double? minX = null;
            double? minY = null;
            double? maxX = null;
            double? maxY = null;

            flatData.ForEach(rec =>
            {
                var lonStr = string.Empty;
                var latStr = string.Empty;

                if (rec.ContainsKey("lo"))
                    lonStr = rec["lo"].ToString();

                if (rec.ContainsKey("la"))
                    lonStr = rec["la"].ToString();

                if (string.IsNullOrWhiteSpace(lonStr) || string.IsNullOrWhiteSpace(latStr) ||
                    !double.TryParse(lonStr.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture,
                        out var lon) || !double.TryParse(latStr.Replace(",", "."), NumberStyles.Any,
                        CultureInfo.InvariantCulture, out var lat))
                {
                    return;
                }

                minX = minX.HasValue ? Math.Min(minX.Value, lon) : lon;
                minY = minY.HasValue ? Math.Min(minY.Value, lat) : lat;
                maxX = maxX.HasValue ? Math.Max(maxX.Value, lon) : lon;
                maxY = maxY.HasValue ? Math.Max(maxY.Value, lat) : lat;
            });

            return (minX, minY, maxX, maxY);
        }


    }
}
