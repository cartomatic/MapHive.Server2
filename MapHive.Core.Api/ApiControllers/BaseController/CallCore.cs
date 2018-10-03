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

namespace MapHive.Core.Api.ApiControllers
{
    public abstract partial class BaseController
    {
        /// <summary>
        /// Calls a core API and auto deserializes output; extracts authorization header off the request
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="request"></param>
        /// <param name="route"></param>
        /// <param name="method"></param>
        /// <param name="queryParams"></param>
        /// <param name="data"></param>
        /// <param name="customHeaders"></param>
        /// <returns></returns>
        protected internal virtual async Task<ApiCallOutput<TOut>> CoreApiCall<TOut>(HttpRequestMessage request, string route, Method method = Method.GET,
            Dictionary<string, object> queryParams = null, object data = null, Dictionary<string, string> customHeaders = null)
        {
            return await CoreApiCall<TOut>(
                route,
                method,
                queryParams,
                data,
                ExtractAuthorizationToken(request),
                customHeaders
            );
        }

        /// <summary>
        /// Calls a core api and auto deserializes output
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="route"></param>
        /// <param name="method"></param>
        /// <param name="queryParams"></param>
        /// <param name="data"></param>
        /// <param name="authToken"></param>
        /// <param name="customHeaders"></param>
        /// <returns></returns>
        protected internal virtual async Task<ApiCallOutput<TOut>> CoreApiCall<TOut>(string route, Method method = Method.GET,
            Dictionary<string, object> queryParams = null, object data = null, string authToken = null, Dictionary<string, string> customHeaders = null)
        {
            return await RestApiCall<TOut>(
                Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig()["Endpoints:Core"],
                route,
                method,
                queryParams,
                data,
                authToken,
                customHeaders
            );
        }

        /// <summary>
        /// Calls a core api; extracts authorization header off a request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="route"></param>
        /// <param name="method"></param>
        /// <param name="queryParams"></param>
        /// <param name="data"></param>
        /// <param name="customHeaders"></param>
        /// <returns></returns>
        protected internal virtual async Task<IRestResponse> CoreApiCall(HttpRequestMessage request, string route, Method method = Method.GET,
            Dictionary<string, object> queryParams = null, object data = null, Dictionary<string, string> customHeaders = null)
        {
            return await CoreApiCall(
                route,
                method,
                queryParams,
                data,
                ExtractAuthorizationToken(request),
                customHeaders
            );
        }

        /// <summary>
        /// Calls a core api
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="route"></param>
        /// <param name="method"></param>
        /// <param name="queryParams"></param>
        /// <param name="data"></param>
        /// <param name="authToken"></param>
        /// <param name="customHeaders"></param>
        /// <returns></returns>
        protected internal virtual async Task<IRestResponse> CoreApiCall(string route, Method method = Method.GET,
            Dictionary<string, object> queryParams = null, object data = null, string authToken = null, Dictionary<string, string> customHeaders = null)
        {
            return await RestApiCall(
                Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig()["Endpoints:Core"],
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
