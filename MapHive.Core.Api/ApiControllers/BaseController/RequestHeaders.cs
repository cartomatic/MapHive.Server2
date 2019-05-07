using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.Configuration;
using Microsoft.AspNetCore.Http;
using RestSharp;

namespace MapHive.Core.Api.ApiControllers
{
    public abstract partial class BaseController
    {
        /// <summary>
        /// Extracts auth token off a request
        /// </summary>
        /// <returns></returns>
        protected internal string ExtractAuthorizationHeader()
        {
            //grab the auth token used by the requestee
            var authToken = string.Empty;

            if (Request.Headers.ContainsKey("Authorization"))
            {
                authToken = Request.Headers["Authorization"].First();
            }

            return authToken;
        }


        private string[] _headersToIgnoreWhenTransferring = new[]
        {
            //ignore auth; it is transferred separately
            "authorization",
            //always avoid passing host header as it means when DNS resolves url to ip and request hits the sever it will resolve back to the domain thie request has originated from - the initial api
            //it would be pretty much ok, if the endpoints used different web servers or machines, but will casue problems when deployed from a single IIS!
            "host",

            //ignore content type as it may have been compressed on input and this call will send just json serialized data
            "content-type",

            //ignore content length! After all when new content is added for POST/PUT it will be worked out appropriately during data serialization and / or compression!
            //also for scenarios, when original request that initiated another request being performed in the background and being of different type (be it whatever, GET, PUT) and different length than the initiator, passing an invalid content length will make RestSharp request take ages to complete (likely because either client or server will be waiting for data me thinks ;)
            "content-length"
        };

        /// <summary>
        /// Transfers request headers
        /// </summary>
        /// <param name="restRequest"></param>
        /// <param name="customHdrs"></param>
        /// <param name="tranferMhHdrs"></param>
        /// <param name="transferHdrs"></param>
        protected internal void TransferRequestHeaders(RestRequest restRequest, Dictionary<string, string> customHdrs, bool tranferMhHdrs, bool transferHdrs)
        {
            foreach (var hdr in Request.Headers)
            {
                if (_headersToIgnoreWhenTransferring.Contains(hdr.Key.ToLower()))
                    continue;

                //assume that custom headers overwrite whatever are the incoming headers
                //this is done outside of this method
                if (customHdrs?.ContainsKey(hdr.Key) == true)
                    continue;

                //should mh headers be transferred?
                var mhHdrs = WebClientConfiguration.GetMhHeaders();
                if(mhHdrs.Contains(hdr.Key) && !tranferMhHdrs)
                    continue;

                //finally check if remaining headers should be transferred
                if(!transferHdrs)
                    continue;


                foreach (var hdrValue in hdr.Value)
                {
                    restRequest.AddHeader(hdr.Key, hdrValue);
                }
            }
        }

    }
}
