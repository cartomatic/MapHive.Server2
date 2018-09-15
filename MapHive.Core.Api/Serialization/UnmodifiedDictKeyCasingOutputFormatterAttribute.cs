using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Use to prevent changing dictionary keys changing
    /// </summary>
    public class UnmodifiedDictKeyCasingOutputFormatterAttribute : Attribute, IAsyncResultFilter
    {
        /// <inheritdoc />
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var originResult = context.Result as JsonResult;

            context.Result = new JsonResult(originResult.Value, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            await next();
        }
    }

    /// <summary>
    /// Custom dict resolver that does not change the dict keys
    /// </summary>
    internal class CamelCaseExceptDictionaryKeysResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
        {
            var contract = base.CreateDictionaryContract(objectType);

            //make sure to not change dict keys!
            contract.DictionaryKeyResolver = propertyName => propertyName;

            return contract;
        }
    }
}
