using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Cartomatic.Utils;
using Cartomatic.Utils.Cache;
using MapHive.Core.Api.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using RestSharp;

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
