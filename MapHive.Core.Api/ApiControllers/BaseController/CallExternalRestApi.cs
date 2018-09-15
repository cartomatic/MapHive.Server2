using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using RestSharp;

namespace MapHive.Core.Api.ApiControllers
{
    public abstract partial class BaseController
    {
        /// <summary>
        /// api call output - encapsulates the actual api output and the response itself for further investigation in a case it's required
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        public class ApiCallOutput<TOut>
        {
            public TOut Output { get; set; }

            public IRestResponse Response { get; set; }
        }

        /// <summary>
        /// returns a resp message based on the response
        /// </summary>
        /// <param name="apiResponse"></param>
        /// <returns></returns>
        public virtual IActionResult ApiCallPassThrough(IRestResponse apiResponse)
        {
            //looks like there is a little problem with content type such as application/json; charset=utf-8
            var contentType = apiResponse.ContentType;
            if (apiResponse.ContentType.StartsWith("application/json", StringComparison.Ordinal))
            {
                contentType = "application/json";
            }
            else if (apiResponse.ContentType.StartsWith("text/xml", StringComparison.Ordinal))
            {
                contentType = "text/xml";
            }

            //looks like no content info was returned from the backend api
            if (!string.IsNullOrWhiteSpace(contentType))
            {
                contentType = "application/octet-stream";
            }
            
            return new ObjectResult(apiResponse.RawBytes ?? new byte[0])
            {
                StatusCode = (int) apiResponse.StatusCode, //note: this cast should be ok, the enum uses proper values
                ContentTypes = new MediaTypeCollection()
                {
                    contentType
                }
            };
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
        /// <returns></returns>
        protected internal virtual async Task<IRestResponse> RestApiCall(string url, string route, Method method = Method.GET,
            Dictionary<string, object> queryParams = null, object data = null, string authToken = null, Dictionary<string, string> customHeaders = null)
        {
            var client = new RestClient($"{url}{(url.EndsWith("/") ? "" : "/")}{route}");
            var request = new RestRequest(method);
            request.AddHeader("Content-Type", "application/json");

            if (customHeaders != null)
            {
                foreach (var headerKey in customHeaders.Keys)
                {
                    request.AddHeader(headerKey, customHeaders[headerKey]);
                }
            }

            //add params if any
            if (queryParams != null && queryParams.Keys.Count > 0)
            {
                foreach (var key in queryParams.Keys)
                {
                    request.AddParameter(key, queryParams[key], ParameterType.QueryString);
                }
            }

            if ((method == Method.POST || method == Method.PUT) && data != null)
            {
                //use custom serializer on output! This is important as the newtonsoft's json stuff is used for the object serialization!
                request.RequestFormat = DataFormat.Json;
                request.JsonSerializer = new Cartomatic.Utils.RestSharpSerializers.NewtonSoftJsonSerializer();
                request.AddBody(data);
            }


            //add auth if need to perform an authorized call
            if (!string.IsNullOrEmpty(authToken))
            {
                request.AddHeader("Authorization", authToken);
            }

#if DEBUG
            var debug = client.BuildUri(request).AbsoluteUri;
#endif

            return await client.ExecuteTaskAsync(request);
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
        /// <returns></returns>
        protected internal virtual async Task<ApiCallOutput<TOut>> RestApiCall<TOut>(string url, string route, Method method = Method.GET,
            Dictionary<string, object> queryParams = null, object data = null, string authToken = null, Dictionary<string, string> customHeaders = null)
        {
            //because of some reason RestSharp is bitching around when deserializing the arr / list output...
            //using Newtonsoft.Json instead

            var output = default(TOut);

            var resp = await RestApiCall(url, route, method, queryParams, data, authToken, customHeaders);
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                output = (TOut)Newtonsoft.Json.JsonConvert.DeserializeObject(resp.Content, typeof(TOut));
            }

            return new ApiCallOutput<TOut>
            {
                Output = output,
                Response = resp
            };
        }
    }
}
