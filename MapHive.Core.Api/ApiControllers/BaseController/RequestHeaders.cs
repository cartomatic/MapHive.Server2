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

            if (Request.Headers.Authorization != null)
            {
                authToken = $"{Request.Headers.Authorization.Scheme} {Request.Headers.Authorization.Parameter}";
            }

            return authToken;
        }

     
        

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
                //ignore auth; it is transferred separately
                if(hdr.Key.ToLower() == "authorization")
                    continue;

                //assume that custom headers overwrite whatever are the incoming headers
                //this is done outside of this method
                if(customHdrs?.ContainsKey(hdr.Key) == true)
                    continue;

                //should mh headers be transferred
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
