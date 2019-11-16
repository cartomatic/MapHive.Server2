using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace MapHive.Core.Api.Extensions
{

    public static class IActionResultExtensions
    {
        /// <summary>
        /// Extracts content off a OK IActionResult
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="r"></param>
        /// <returns></returns>
        public static T GetContent<T>(this IActionResult r)
            where T : class
        {
            return (r as OkObjectResult)?.Value as T;
        }
    }

}
