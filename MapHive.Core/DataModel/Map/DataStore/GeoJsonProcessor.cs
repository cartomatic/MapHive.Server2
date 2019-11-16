using GeoJSON.Net;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FeatureCollection = GeoJSON.Net.Feature.FeatureCollection;

namespace MapHive.Core.DataModel.Map
{
    public partial class DataStore
    {
        /// <summary>
        /// Processes a GeoJSON file and uploads it to a db
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <param name="path">Path to a directory a GeoJSON has been uploaded to</param>
        /// <returns></returns>
        public static async Task<DataStore> ProcessGeoJson(DbContext dbCtx, string path)
        {
            //assuming a single zip can only be present in a directory, as uploading data for a single layer

            //if there is a zip archive, need to extract it
            ExtractZip(path);


            //test for required shp format files presence...
            var geoJson = Directory.GetFiles(path, "*.geojson").FirstOrDefault();
            if (string.IsNullOrEmpty(geoJson))
                throw MapHive.Core.DataModel.Validation.Utils.GenerateValidationFailedException("GeoJSON", "no_geojson_file",
                    "GeoJSON file has not been found");

            var fName = Path.GetFileNameWithoutExtension(geoJson);


            var output = GetDataStore(fName, "geojson");


            var jsonData = File.ReadAllText(geoJson);
            var geoJsonReader = new NetTopologySuite.IO.GeoJsonReader();

            // pass geoJson's FeatureCollection to read all the features
            var featureCollection = geoJsonReader.Read<GeoJSON.Net.Feature.FeatureCollection>(jsonData);

            // if feature collection is null then return 
            if (featureCollection == null)
                throw MapHive.Core.DataModel.Validation.Utils.GenerateValidationFailedException("GeoJSON", "no_data",
                    "GeoJSON file could not be deserialized");


            //grab bbox - assume it to be in 4326
            //(output.MinX, output.MinY, output.MaxX, output.MaxY) = ExtractGeoJsonBbox(featureCollection);
            //Note: calculating bbox after import succeeded

            //first work out a data model - this is json, so can have different props per object
            foreach (var f in featureCollection.Features)
            {
                foreach (var fProperty in f.Properties)
                {
                    if (output.DataSource.Columns.Any(c => c.Name == GetSafeDbObjectName(fProperty.Key)))
                        continue;

                    if (!CheckIfObjectSafe(fProperty.Key))
                        throw MapHive.Core.DataModel.Validation.Utils.GenerateValidationFailedException("ColName", "bad_col_name",
                            "Column name contains forbidden words");

                    var colType = SystemTypeToColumnDataType(fProperty.Value.GetType());
                    if (colType != ColumnDataType.Unknown)
                    {
                        output.DataSource.Columns.Add(new Column
                        {
                            Type = colType,
                            Name = GetSafeDbObjectName(fProperty.Key),
                            FriendlyName = fProperty.Key
                        });
                    }
                }
            }


            //Since reading GeoJSON, there is also a geometry column
            output.DataSource.Columns.Add(new Column
            {
                Type = ColumnDataType.Geom,
                Name = "geom",
                FriendlyName = "Geom"
            });

            //create object straight away, so when something goes wrong with import, etc. there is a chance for a cleanup bot
            //to pick it up and cleanup the orphaned data when necessary
            await output.CreateAsync(dbCtx);


            //once have the shp meta, can create a db table for the data
            var ensureSchemaSql = GetEnsureSchemaSql(output);

            var createTableSql = GetCreateTableSql(output);

            //geometry col index
            var geomIdxSql = GetCreateGeomIdxSql(output);


            using (var conn = new NpgsqlConnection(output.DataSource.DataSourceCredentials.GetConnectionString()))
            using (var cmd = new NpgsqlCommand())
            {
                await conn.OpenAsync();
                cmd.Connection = conn;

                cmd.CommandText = ensureSchemaSql;
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = createTableSql;
                await cmd.ExecuteNonQueryAsync();


                //table ready, so pump in the data
                //assume the geoms to be in EPSG:4326 (GeoJSON :)

                var batchSize = 25;
                var processed = 0;
                var insertData = new List<string>(batchSize);

                foreach (var f in featureCollection.Features)
                {
                    if (processed > 0 && processed % batchSize == 0)
                    {
                        await ExecuteInsertBatch(cmd, output, insertData);
                    }

                    processed += 1;


                    var data = new string[f.Properties.Count + 1];
                    var pIdx = 0;
                    foreach (var c in output.DataSource.Columns)
                    {
                        if (c.Type == ColumnDataType.Geom)
                            continue;

                        data[pIdx] = $"@r{processed}_p{pIdx}";

                        if (f.Properties.ContainsKey(c.FriendlyName))
                        {
                            if (f.Properties[c.FriendlyName] != null)
                            {
                                cmd.Parameters.AddWithValue(data[pIdx], f.Properties[c.FriendlyName]);
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
                    data[data.Length - 1] =
                        $"ST_Transform(ST_SetSRID(ST_GeomFromGeoJSON('{JsonConvert.SerializeObject(f.Geometry)}'),4326),3857)";
                    //assume geojson to be always in 4326 and transform it to spherical mercator!

                    insertData.Add($"SELECT {string.Join(",", data)}");
                }


                if (insertData.Count > 0)
                {
                    await ExecuteInsertBatch(cmd, output, insertData);
                }

                //when ready, index the geom col, so geom reads are quicker
                cmd.CommandText = geomIdxSql;
                await cmd.ExecuteNonQueryAsync();

                await CalculateBBox(cmd, output);

                //location col indexing as required
                await CreateLocationIndex(cmd, output);
            }


            return output;
        }

        /// <summary>
        /// Extracts a bbox of a GeoJSON feature collection object
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        protected static (double? minX, double? minY, double? maxX, double? maxY) ExtractGeoJsonBbox(FeatureCollection fc)
        {
            if (fc.BoundingBoxes != null)
                return (fc.BoundingBoxes[0], fc.BoundingBoxes[1], fc.BoundingBoxes[2], fc.BoundingBoxes[3]);

            double? minX = null;
            double? minY = null;
            double? maxX = null;
            double? maxY = null;
            fc.Features.ForEach(f =>
            {
                var bbox = f.BoundingBoxes;

                if (bbox == null)
                    switch (f.Geometry.Type)
                    {
                        case GeoJSONObjectType.Point:
                            bbox = ((GeoJSON.Net.Geometry.Point)f.Geometry).BoundingBoxes;
                            break;
                        case GeoJSONObjectType.MultiPoint:
                            bbox = ((GeoJSON.Net.Geometry.MultiPoint)f.Geometry).BoundingBoxes;
                            break;

                        case GeoJSONObjectType.LineString:
                            bbox = ((GeoJSON.Net.Geometry.LineString)f.Geometry).BoundingBoxes;
                            break;
                        case GeoJSONObjectType.MultiLineString:
                            bbox = ((GeoJSON.Net.Geometry.MultiLineString)f.Geometry).BoundingBoxes;
                            break;

                        case GeoJSONObjectType.Polygon:
                            bbox = ((GeoJSON.Net.Geometry.Polygon)f.Geometry).BoundingBoxes;
                            break;
                        case GeoJSONObjectType.MultiPolygon:
                            bbox = ((GeoJSON.Net.Geometry.MultiPolygon)f.Geometry).BoundingBoxes;
                            break;
                    }

                if (bbox == null)
                    return;

                minX = minX.HasValue ? Math.Min(minX.Value, bbox[0]) : bbox[0];
                minY = minY.HasValue ? Math.Min(minY.Value, bbox[1]) : bbox[1];
                maxX = maxX.HasValue ? Math.Max(maxX.Value, bbox[2]) : bbox[2];
                maxY = maxY.HasValue ? Math.Max(maxY.Value, bbox[3]) : bbox[3];
            });

            return (minX, minY, maxX, maxY);
        }
    }
}
