using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Configuration;

namespace MapHive.Core.Api
{
    /// <summary>
    /// utils that simplify file upload
    /// </summary>
    public class FileUploadHelpers
    {
        /// <summary>
        /// Gets file upload configurations
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, FileUploadConfiguration> GetFileUploadConfigurations()
        {
            var cfg = Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig();
            return cfg.GetSection("FileUploadConfiguration")
                .Get<Dictionary<string, FileUploadConfiguration>>();
        }

        /// <summary>
        /// Returns a file upload configuration by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static FileUploadConfiguration GetFileUploadConfiguration(string key)
        {
            var uploadConfig = GetFileUploadConfigurations();
            return uploadConfig?[key];
        }

        /// <summary>
        /// Ensures dir presence
        /// </summary>
        /// <param name="cfg"></param>
        public static void EnsureUpload(FileUploadConfiguration cfg)
        {
            var dir = cfg.Path.SolvePath();

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }


        /// <summary>
        /// Downloads a given web resource to a file
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileName"></param>
        /// <param name="extension"></param>
        /// <param name="fileUploadConfigKey"></param>
        /// <returns></returns>
        public static async Task<Guid> DownloadResource(string url, string fileName, string extension, string fileUploadConfigKey)
        {
            var uploadConfig = GetFileUploadConfigurations();

            if (uploadConfig == null)
                throw new InvalidOperationException("FileUploadConfiguration has not been found.");

            if (!uploadConfig.ContainsKey(fileUploadConfigKey))
                throw new ArgumentException($"Could not find the {fileUploadConfigKey} key in FileUploadConfiguration.");


            var saveCfg = uploadConfig[fileUploadConfigKey];

            var uploadId = Guid.NewGuid();
            var uploadDir = Path.Combine(saveCfg.Path.SolvePath(), uploadId.ToString());

            Directory.CreateDirectory(uploadDir);

            try
            {
                using (var client = new WebClient())
                {
                    client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.33 Safari/537.36");
                    client.DownloadFile(new Uri(url), Path.Combine(uploadDir, fileName ?? uploadId.ToString()));
                }
            }
            catch (Exception ex)
            {
                throw MapHive.Core.DataModel.Validation.Utils.GenerateValidationFailedException("RemoteResource", "remote_download_failure",
                    $"Failed to download a remote resource to a file: {url}");
            }
            

            FileCleanup(saveCfg);

            return uploadId;
        }



        /// <summary>
        /// Saves files passed as multi part upload to the folder specified by the key in the FileUploadConfiguration
        /// </summary>
        /// <param name="request"></param>
        /// <param name="fileUploadConfigKey"></param>
        /// <returns></returns>
        public static async Task<(Guid uploadId, Dictionary<string, string> formData)> SaveFiles(HttpRequest request,
            string fileUploadConfigKey)
        {
            var uploadConfig = GetFileUploadConfigurations();

            if (uploadConfig == null)
                throw new InvalidOperationException("FileUploadConfiguration has not been found.");

            if (!uploadConfig.ContainsKey(fileUploadConfigKey))
                throw new ArgumentException($"Could not find the {fileUploadConfigKey} key in FileUploadConfiguration.");

            return await SaveFiles(request, uploadConfig[fileUploadConfigKey]);
        }

        /// <summary>
        /// Saves files passed as multi part upload to the folder specified in the provided FileUploadConfiguration
        /// </summary>
        /// <param name="request"></param>
        /// <param name="fileUploadConfig"></param>
        /// <returns></returns>
        public static async Task<(Guid uploadId, Dictionary<string, string> formData)> SaveFiles(HttpRequest request,
            FileUploadConfiguration fileUploadConfig)
        {
            var saveFileMeta = await SaveMultiPartData(request, fileUploadConfig.Path);

            FileCleanup(fileUploadConfig);

            return saveFileMeta;
        }

        /// <summary>
        /// Cleans up upload dir as specified in the upload config
        /// </summary>
        /// <param name="fileUploadConfig"></param>
        public static void FileCleanup(FileUploadConfiguration fileUploadConfig)
        {
            FileCleanup(fileUploadConfig.Path, fileUploadConfig.FileRetentionInMinutes);
        }

        /// <summary>
        /// cleans up given directory
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="fileRetentionMinutes"></param>
        /// <returns></returns>
        public static void FileCleanup(string directory, int? fileRetentionMinutes = 60)
        {
            directory = directory.SolvePath();

            if (!Directory.Exists(directory))
                return;

            //delete files
            foreach (var file in Directory.GetFiles(directory))
            {
                try
                {
                    var fi = new FileInfo(file);
                    if (fi.Exists && new TimeSpan(DateTime.Now.Ticks - fi.CreationTime.Ticks).TotalMinutes >
                        fileRetentionMinutes)
                    {
                        File.Delete(file);
                    }
                }
                catch
                {
                    //ignore
                }
            }

            //delete directories
            foreach (var dir in Directory.GetDirectories(directory))
            {
                try
                {
                    var di = new DirectoryInfo(dir);
                    if (di.Exists && new TimeSpan(DateTime.Now.Ticks - di.CreationTime.Ticks).TotalMinutes >
                        fileRetentionMinutes)
                    {
                        Directory.Delete(dir, true);
                    }
                }
                catch
                {
                    //ignore
                }
            }
        }

        /// <summary>
        /// Reads multipart data and returns a Guid of a folder it has been saved to as well as the extracted form data if any
        /// </summary>
        /// <param name="request"></param>
        /// <param name="saveDir"></param>
        /// <returns></returns>
        public static async Task<(Guid uploadId, Dictionary<string, string> formData)> SaveMultiPartData(HttpRequest request, string saveDir)
        {
            //make the dir absolute
            saveDir = saveDir.SolvePath();

            if (!Directory.Exists(saveDir))
                Directory.CreateDirectory(saveDir);

            var uploadId = Guid.NewGuid();
            var uploadDir = Path.Combine(saveDir, uploadId.ToString());

            Directory.CreateDirectory(uploadDir);


            var boundary = GetBoundary(request.ContentType);
            var reader = new MultipartReader(boundary, request.Body, 80 * 1024);

            var valuesByKey = new Dictionary<string, string>();
            MultipartSection section;

            while ((section = await reader.ReadNextSectionAsync()) != null)
            {
                var contentDispo = section.GetContentDispositionHeader();

                if (contentDispo.IsFileDisposition())
                {
                    var fileSection = section.AsFileSection();

                    using (var fs = new FileStream(Path.Combine(uploadDir, fileSection.FileName), FileMode.Create))
                    {
                        await fileSection.FileStream.CopyToAsync(fs);
                    }
                }
                else if (contentDispo.IsFormDisposition())
                {
                    var formSection = section.AsFormDataSection();
                    var value = await formSection.GetValueAsync();
                    valuesByKey.Add(formSection.Name, value);
                }
            }

            return (uploadId, valuesByKey);
        }

        /// <summary>
        /// gets boundary info
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        private static string GetBoundary(string contentType)
        {
            if (contentType == null)
                throw new ArgumentNullException(nameof(contentType));

            var elements = contentType.Split(' ');
            var element = elements.First(entry => entry.StartsWith("boundary="));
            var boundary = element.Substring("boundary=".Length);

            boundary = HeaderUtilities.RemoveQuotes(boundary).ToString();

            return boundary;
        }

        /// <summary>
        /// Reads stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="bufferSize"></param>
        /// <returns></returns>
        private static async Task<int> ReadStream(Stream stream, int bufferSize)
        {
            var buffer = new byte[bufferSize];

            int bytesRead;
            var totalBytes = 0;

            do
            {
                bytesRead = await stream.ReadAsync(buffer, 0, bufferSize);
                totalBytes += bytesRead;
            } while (bytesRead > 0);
            return totalBytes;
        }
        
    }
}
