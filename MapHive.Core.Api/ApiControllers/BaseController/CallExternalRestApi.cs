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
using BrotliSharpLib;
using Cartomatic.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
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
            /// <summary>
            /// deserialized api call output
            /// </summary>
            public TOut Output { get; set; }

            /// <summary>
            /// raw IRestResponse
            /// </summary>
            public IRestResponse Response { get; set; }
        }

        /// <summary>
        /// returns a resp message based on the response
        /// </summary>
        /// <param name="apiResponse"></param>
        /// <returns></returns>
        protected virtual IActionResult ApiCallPassThrough(IRestResponse apiResponse)
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
            //this may be because not content has actually been sent out...
            if (string.IsNullOrWhiteSpace(contentType))
            {
                //for such scenario pick the first content type accepted by the client.
                //otherwise client (or kestrel?) will return 406 code as the accepted return content types do not match the one actually returned
                contentType = apiResponse.Request.Parameters.FirstOrDefault(p => p.Name == "Accept")?.Value?.ToString().Split(',').FirstOrDefault();

                //if the above did not work, default to octet-stream
                if (string.IsNullOrEmpty(contentType))
                {
                    contentType = "application/octet-stream";
                }
            }

            //return new ByteArrayContent();
            var content = ExtractResponseContentAsString(apiResponse);
            return new ObjectResult(
                
                string.IsNullOrEmpty(content)
                    ? (object)(apiResponse.RawBytes ?? new byte[0])
                    : contentType == "application/json"
                        ? !string.IsNullOrEmpty(content) ? JsonConvert.DeserializeObject(content) : null //so nicely serialize object is returned
                        : contentType
            )
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
        /// <param name="transferAuthHdr">Whether or not auth header should be automatically transferred to outgoing request; when a custom auth header is provided it will always take precedence</param>
        /// <param name="transferMhHdrs">Whether or not should auto transfer maphive request headers such as request src, lng, etc</param>
        /// <param name="transferRequestHdrs">Whether or not should auto transfer request headers so they are sent out </param>
        /// <returns></returns>
        protected internal virtual async Task<IRestResponse> RestApiCall(string url, string route, Method method = Method.GET,
            Dictionary<string, object> queryParams = null, object data = null, string authToken = null, Dictionary<string, string> customHeaders = null, bool transferAuthHdr = true, bool transferMhHdrs = true, bool transferRequestHdrs = true)
        {
            var client = new RestClient($"{url}{(url.EndsWith("/") ? "" : "/")}{route}");
            var request = new RestRequest(method);

            //assuming here only json ap input is supported.
            request.AddHeader("Content-Type", "application/json");

            if (customHeaders != null)
            {
                foreach (var headerKey in customHeaders.Keys)
                {
                    request.AddHeader(headerKey, customHeaders[headerKey]);
                }
            }

            //since the api call is done in scope of a maphive controller try to attach the default custom maphive headers
            TransferRequestHeaders(request, customHeaders, transferMhHdrs, transferRequestHdrs);



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
                request.AddJsonBody(data);
            }


            //when auth token not provided try to obtain it off the request
            if (transferAuthHdr && string.IsNullOrEmpty(authToken))
            {
                authToken = ExtractAuthorizationHeader();
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
        /// <param name="transferAuthHdr">Whether or not auth header should be automatically transferred to outgoing request; when a custom auth header is provided it will always take precedence</param>
        /// <param name="transferMhHdrs">Whether or not should auto transfer maphive request headers such as request src, lng, etc</param>
        /// <param name="transferRequestHdrs">Whether or not should auto transfer request headers so they are sent out </param>
        /// <returns></returns>
        protected internal virtual async Task<ApiCallOutput<TOut>> RestApiCall<TOut>(string url, string route, Method method = Method.GET,
            Dictionary<string, object> queryParams = null, object data = null, string authToken = null, Dictionary<string, string> customHeaders = null, bool transferAuthHdr = true, bool transferMhHdrs = true, bool transferRequestHdrs = true)
        {
            //because of some reason RestSharp is bitching around when deserializing the arr / list output...
            //using Newtonsoft.Json instead

            var output = default(TOut);

            var resp = await RestApiCall(url, route, method, queryParams, data, authToken, customHeaders, transferAuthHdr, transferMhHdrs, transferRequestHdrs);
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                output = (TOut)Newtonsoft.Json.JsonConvert.DeserializeObject(ExtractResponseContentAsString(resp), typeof(TOut));
            }

            return new ApiCallOutput<TOut>
            {
                Output = output,
                Response = resp
            };
        }

        /// <summary>
        /// Extracts response content as string
        /// </summary>
        /// <param name="resp"></param>
        /// <returns></returns>
        protected internal virtual string ExtractResponseContentAsString(IRestResponse resp)
        {
            var content = string.Empty;

            if (resp.ContentEncoding == "br")
            {
                using (var ms = new MemoryStream(resp.RawBytes))
                using (var bs = new BrotliStream(ms, CompressionMode.Decompress))
                using (var sr = new StreamReader(bs))
                {
                    content = sr.ReadToEnd();
                }
            }
            else
            {
                content = resp.Content;
            }

            return content;
        }
    }
}
