using GeoJSON.Net;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cartomatic.Utils.Data;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using FeatureCollection = GeoJSON.Net.Feature.FeatureCollection;

namespace MapHive.Core.DataModel.Map
{
    public partial class DataStore
    {

        protected const string HasImgsColname = "has_imgs";

        public static async Task<bool> EnsureImagesCol(DbContext dbCtx, DataStore ds)
        {
            if (ds == null)
                return false;

            var addCol = false;


            if (ds.DataSource.Columns.All(c => c.Name != HasImgsColname))
            {
                addCol = true;

                ds.DataSource.Columns.Add(new Column
                {
                    Name = HasImgsColname,
                    FriendlyName = HasImgsColname,
                    Styleable = true,
                    Type = ColumnDataType.Bool
                });

                await ds.UpdateAsync(dbCtx);
            }

            return addCol;
        }

        public static async Task ProcessImages(DataStore ds, string destPath, string uploadPath, string matchingCol, bool addImgsCol)
        {
            if(ds.DataSource.Columns.All(c => c.Name != matchingCol))
                throw MapHive.Core.DataModel.Validation.Utils.GenerateValidationFailedException(nameof(matchingCol),
                "unknown_matching_col",
                "Matching column not found in a layer's dataset");

            //assuming a single zip can only be present in a directory, as uploading data for a single layer

            //if there is a zip archive, need to extract it
            ExtractZip(uploadPath);


            //drill 2 lvls down if required - there may be a root folder for the img upload, though there may not have be...
            var imgDirs = new Dictionary<string, string>();
            foreach (var dir in Directory.GetDirectories(uploadPath))
            {
                //if dir has subdirs, assume subdirs should be proccessed, otherwise treat a dir as an img container
                var subDirs = Directory.GetDirectories(dir);
                if (subDirs.Any())
                {
                    foreach (var subDir in subDirs)
                    {
                        imgDirs.Add(subDir.Split('\\').Last(), subDir);
                    }
                }
                else
                {
                    imgDirs.Add(Path.GetDirectoryName(dir), dir);
                }
            }

            //spin through all the objects to be able to match images
            using (var conn = new NpgsqlConnection(ds.DataSource.DataSourceCredentials.GetConnectionString()))
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                await conn.OpenAsync();


                if (addImgsCol)
                {
                    cmd.CommandText = $"ALTER TABLE {ds.DataSource.Schema}.{ds.DataSource.Table} ADD COLUMN {HasImgsColname} boolean;";
                    await cmd.ExecuteNonQueryAsync();
                }


                cmd.CommandText =
                    $"select {MapHive.Core.DataModel.Map.DataStore.IdCol}, {matchingCol} from {ds.DataSource.Schema}.{ds.DataSource.Table};";

                var hasImgsIds = new List<int>();

                using (var rdr = await cmd.ExecuteReaderAsync())
                {
                    while (rdr.HasRows && await rdr.ReadAsync())
                    {
                        var matchingValue = rdr.GetValue(1).ToString();
                        if (imgDirs.ContainsKey(matchingValue))
                        {
                            var mhId = rdr.GetInt32(0); //MapHive.Core.DataModel.Map.DataStore.IdCol
                            var destFolder = Path.Combine(destPath, mhId.ToString());
                            if (!Directory.Exists(destFolder))
                                Directory.CreateDirectory(destFolder);

                            var imageFiles = Directory.GetFiles(imgDirs[matchingValue])
                                .Where(s => ImgFileExtensions.Any(ext => ext == s.Split('.').Last().ToLower()));

                            if (imageFiles.Any())
                            {
                                hasImgsIds.Add(mhId);

                                foreach (var file in imageFiles)
                                {
                                    File.Copy(file, Path.Combine(destFolder, Path.GetFileName(file)), true);
                                }
                            }
                        }
                    }
                }

                //finally mark objects as having images!
                cmd.CommandText =
                    $"update {ds.DataSource.Schema}.{ds.DataSource.Table} set {HasImgsColname}=true where {MapHive.Core.DataModel.Map.DataStore.IdCol} in ({string.Join(",", hasImgsIds)})";
                await cmd.ExecuteNonQueryAsync();
            }

            //once copied, can clean up the incoming folder
            Directory.Delete(uploadPath, true);
        }

        private static string[] ImgFileExtensions = new[]
        {
            "jpg", "jpeg", "gif", "png"
        };

    }
}
