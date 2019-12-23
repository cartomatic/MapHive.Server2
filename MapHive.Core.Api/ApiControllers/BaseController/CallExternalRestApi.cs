using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils;
using MapHive.Core.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using RestSharp;

namespace MapHive.Core.Api.ApiControllers
{
    public abstract partial class BaseController
    {

        /// <summary>
        /// returns a resp message based on the response
        /// </summary>
        /// <param name="apiResponse"></param>
        /// <returns></returns>
        protected virtual IActionResult ApiCallPassThrough(IRestResponse apiResponse)
        {
            return Cartomatic.Utils.RestApi.ApiCallPassThrough(apiResponse);
        }

        /// <summary>
        /// Calls a rest API
        /// </summary>
        /// <param name="url"></param>
        /// <param name="route"></param>
        /// <param name="method"></param>
        /// <param name="queryParams"></param>
        /// <param name="data"></param>
        /// <param name="authToken">Allows for performing authorized calls against MapHive apis</param>
        /// <param name="customHeaders"></param>
        /// <param name="transferAuthHdr">Whether or not auth header should be automatically transferred to outgoing request; when a custom auth header is provided it will always take precedence</param>
        /// <param name="transferMhHdrs">Whether or not should auto transfer maphive request headers such as request src, lng, etc</param>
        /// <param name="transferRequestHdrs">Whether or not should auto transfer request headers so they are sent out </param>
        /// <returns></returns>
        protected internal virtual async Task<IRestResponse> RestApiCall(string url, string route, Method method = Method.GET,
            Dictionary<string, object> queryParams = null, object data = null, string authToken = null, Dictionary<string, string> customHeaders = null, bool transferAuthHdr = true, bool transferMhHdrs = true, bool transferRequestHdrs = true)
        {
            if (transferMhHdrs)
            {
                customHeaders ??= new Dictionary<string, string>();
                foreach (var mhHdr in WebClientConfiguration.GetMhHeaders())
                {
                    if (Request.Headers.ContainsKey(mhHdr))
                    {
                        if (!customHeaders.ContainsKey(mhHdr))
                            customHeaders.Add(mhHdr, Request.Headers[mhHdr]);
                    }
                }
            }

            return await Cartomatic.Utils.RestApi.RestApiCall(Request, url, route, method, queryParams, data, authToken,
                customHeaders, null, transferAuthHdr,
                transferRequestHdrs);
        }

        /// <summary>
        /// Calls a REST API, auto deserializes output
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="url"></param>
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
        protected internal virtual async Task<RestApi.ApiCallOutput<TOut>> RestApiCall<TOut>(string url, string route,
            Method method = Method.GET,
            Dictionary<string, object> queryParams = null, object data = null, string authToken = null,
            Dictionary<string, string> customHeaders = null, bool transferAuthHdr = true, bool transferMhHdrs = true,
            bool transferRequestHdrs = true)
        {
            return await Cartomatic.Utils.RestApi.RestApiCall<TOut>(Request, url, route, method, queryParams, data, authToken,
                customHeaders, transferMhHdrs ? WebClientConfiguration.GetMhHeaders() : null, transferAuthHdr,
                transferRequestHdrs);
        }
    }
}
