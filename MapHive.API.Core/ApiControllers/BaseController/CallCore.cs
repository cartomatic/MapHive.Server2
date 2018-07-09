using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace MapHive.Api.Core.ApiControllers
{
    public abstract partial class BaseController
    {
        /// <summary>
        /// Calls a core API
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="route"></param>
        /// <param name="method"></param>
        /// <param name="queryParams"></param>
        /// <param name="data"></param>
        /// <param name="authToken"></param>
        /// <returns></returns>
        protected internal virtual async Task<ApiCallOutput<TOut>> CoreApiCall<TOut>(string route, Method method = Method.GET,
            Dictionary<string, object> queryParams = null, object data = null, string authToken = null, Dictionary<string, string> customHeaders = null)
        {
            return await RestApiCall<TOut>(
                Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig()["CoreApiEndpoint"],
                route,
                method,
                queryParams,
                data,
                authToken,
                customHeaders
            );
        }
    }
}
