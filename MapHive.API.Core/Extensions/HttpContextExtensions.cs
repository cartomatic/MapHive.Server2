using System;
using System.Collections.Generic;
using System.Text;
using MapHive.Core.Configuration;
using Microsoft.AspNetCore.Http;

namespace MapHive.API.Core.Extensions
{
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Appends "total" header to response object of the context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="total"></param>
        public static void AppendTotalHeader(this HttpContext context, int total)
        {
            context.Response.Headers.Append(WebClientConfiguration.HeaderTotal, $"{total}");
            context.Response.Headers.Append("Access-Control-Expose-Headers", WebClientConfiguration.HeaderTotal);
        }
    }
}
