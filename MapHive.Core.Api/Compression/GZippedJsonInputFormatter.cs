using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cartomatic.Utils;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace MapHive.Core.Api.Compression
{
    /// <summary>
    /// Custom media formatter for reading data passed as gzip/json
    /// </summary>
    class GZippedJsonInputFormatter : InputFormatter
    {
        public GZippedJsonInputFormatter()
        {
            // Add the supported media type.
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("gzip/json"));
        }

        /// <inheritdoc />
        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            try
            {
                using (var decompressedStream = DecompressToStream(context.HttpContext.Request.Body))
                using (var sr = new StreamReader(decompressedStream))
                {
                    decompressedStream.Position = 0;
                    var jsonData = sr.ReadToEnd();

                    return InputFormatterResult.Success(!string.IsNullOrEmpty(jsonData)
                        ? Newtonsoft.Json.JsonConvert.DeserializeObject(jsonData, context.ModelType)
                        : null);
                }
            }
            catch
            {
                return InputFormatterResult.Failure();
            }
        }

        /// <inheritdoc />
        protected override bool CanReadType(Type type)
        {
            return true;
        }


        ///// <inheritdoc />
        //public override async Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger,
        //    CancellationToken cancellationToken)
        //{
        //    using (var decompressedStream = DecompressToStream(await content.ReadAsStreamAsync()))
        //    using (var sr = new StreamReader(decompressedStream))
        //    {
        //        decompressedStream.Position = 0;
        //        var jsonData = sr.ReadToEnd();

        //        return !string.IsNullOrEmpty(jsonData) ? Newtonsoft.Json.JsonConvert.DeserializeObject(jsonData, type) : null;
        //    }
        //}

        /// <summary>
        /// Decompresses gzip stream to data stream
        /// </summary>
        /// <param name="dataStr"></param>
        /// <returns></returns>
        private Stream DecompressToStream(Stream dataStr)
        {
            using (var gzipStream = new Ionic.Zlib.GZipStream(dataStr, Ionic.Zlib.CompressionMode.Decompress))
            {
                return gzipStream.CopyStream();
            }
        }

    }
}
