using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MapHive.Core.Api.ApiControllers
{
    public abstract partial class BaseController
    {
        /// <summary>
        /// Extracts auth token off a request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected internal string ExtractAuthorizationToken(HttpRequestMessage request)
        {
            //grab the auth token used by the requestee
            var authToken = string.Empty;

            if (request.Headers.Authorization != null)
            {
                authToken = $"{request.Headers.Authorization.Scheme} {request.Headers.Authorization.Parameter}";
            }

            return authToken;
        }
    }
}
