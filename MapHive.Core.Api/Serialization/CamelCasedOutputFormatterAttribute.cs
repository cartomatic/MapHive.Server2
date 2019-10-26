using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MapHive.Core.Api.Serialization
{


    /// <summary>
    /// Enforces formatting output in CamelCase
    /// </summary>
    public class CamelCasedOutputFormatterAttribute : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(
            ResultExecutingContext context,
            ResultExecutionDelegate next)
        {
            var originResult = context.Result as JsonResult;

            context.Result = new JsonResult(originResult.Value, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            await next();
        }
    }

}
