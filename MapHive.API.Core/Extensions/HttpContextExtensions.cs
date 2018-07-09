﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapHive.Core.Configuration;
using Microsoft.AspNetCore.Http;

namespace MapHive.Api.Core.Extensions
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

        /// <summary>
        /// Extracts authorization header off http context
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static (string scheme, string parameter)? ExtractAuthHeader(this HttpContext context)
        {
            context.Request.Headers.TryGetValue("Authorization", out var authHeader);

            if (authHeader.Count != 1)
                return null;

            var headerParts = authHeader[0].Split(' ').Where(str => !string.IsNullOrEmpty(str)).ToArray();
            if (headerParts.Length == 2)
            {
                return (headerParts[0], headerParts[1]);
            }

            return null;
        }


        /// <summary>
        /// Extracts referrer header value
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Uri ExtractReferrerHeader(this HttpContext context)
        {
            context.Request.Headers.TryGetValue("Referrer", out var referrerValues);

            return referrerValues.Count == 1 ? new Uri(referrerValues[0]) : null;
        }
    }
}
