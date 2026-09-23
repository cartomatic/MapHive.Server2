using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Cartomatic.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace MapHive.Core.Api.Authorize
{

    public static class InputGzipEncodingMiddlewareExtensions
    {
        /// <summary>
        /// Activates token authorize middleware to handle api token auth properly;
        /// <para/>this middleware should only be activated on demand
        /// </summary>
        /// <param name="app"></param>
        public static void UseInputGzipEncodingMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<InputGzipEncodingMiddleware>();
        }
    }

    /// <summary>
    /// When plugged into pipeline, decodes gzip body content when encountered
    /// </summary>
    public class InputGzipEncodingMiddleware
    {
        private readonly RequestDelegate _next;

        public InputGzipEncodingMiddleware(RequestDelegate next)
        {
            this._next = next;
        }


        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers["Encoding"].FirstOrDefault() == "gzip")
            {
                try
                {
                    context.Request.Body = await DecompressToStream(context.Request.Body);
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync(ex.Message);
                    return;
                }
            }

            //let all the middleware do the job
            await _next.Invoke(context);
        }

        private async Task<Stream> DecompressToStream(Stream dataStr)
        {
            using (var gzipStream = new System.IO.Compression.GZipStream(dataStr, CompressionMode.Decompress))
            {
                return await gzipStream.CopyStreamAsync();
            }
        }
    }

}
