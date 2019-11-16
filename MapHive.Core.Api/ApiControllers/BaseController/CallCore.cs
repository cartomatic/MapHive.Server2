using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils;
using RestSharp;

namespace MapHive.Core.Api.ApiControllers
{
    public abstract partial class BaseController
    {
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
        /// <param name="transferAuthHdr"></param>
        /// <param name="transferMhHdrs"></param>
        /// <param name="transferRequestHdrs"></param>
        /// <returns></returns>
        protected internal virtual async Task<RestApi.ApiCallOutput<TOut>> CoreApiCall<TOut>(string route, Method method = Method.GET,
            Dictionary<string, object> queryParams = null, object data = null, string authToken = null, Dictionary<string, string> customHeaders = null, bool transferAuthHdr = true, bool transferMhHdrs = true, bool transferRequestHdrs = true)
        {
            return await RestApiCall<TOut>(
                Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig()["Endpoints:Core"],
                route,
                method,
                queryParams,
                data,
                authToken,
                customHeaders,
                transferAuthHdr: transferAuthHdr,
                transferMhHdrs: transferMhHdrs,
                transferRequestHdrs: transferRequestHdrs
            );
        }


        /// <summary>
        /// Calls a core api
        /// </summary>
        /// <param name="route"></param>
        /// <param name="method"></param>
        /// <param name="queryParams"></param>
        /// <param name="data"></param>
        /// <param name="authToken"></param>
        /// <param name="customHeaders"></param>
        /// <param name="transferAuthHdr">Whether or not auth header should be automatically transferred to outgoing request; when a custom auth header is provided it will always take precedence</param>
        /// <param name="transferMhHdrs">Whether or not should auto transfer maphive request headers such as request src, lng, etc</param>
        /// <param name="transferRequestHdrs">Whether or not should auto transfer request headers so they are sent out </param>
        /// <returns></returns>
        protected internal virtual async Task<IRestResponse> CoreApiCall(string route, Method method = Method.GET,
            Dictionary<string, object> queryParams = null, object data = null, string authToken = null, Dictionary<string, string> customHeaders = null, bool transferAuthHdr = true, bool transferMhHdrs = true, bool transferRequestHdrs = true)
        {
            return await RestApiCall(
                Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig()["Endpoints:Core"],
                route,
                method,
                queryParams,
                data,
                authToken,
                customHeaders,
                transferAuthHdr,
                transferMhHdrs,
                transferRequestHdrs
            );
        }
    }
}
