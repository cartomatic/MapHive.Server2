using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MapHive.Core.Configuration;
using Microsoft.AspNetCore.Http;

namespace MapHive.Api.Core.ApiControllers
{
    public abstract partial class BaseController
    {
        /// <summary>
        /// Extracts a source header off a request. Source header is used by the MH env to pass a full request source including hash, because hash is never sent to the client
        /// </summary>
        /// <returns></returns>
        public string GetRequestSource(HttpContext context)
        {
            return context.Request.Headers[WebClientConfiguration.HeaderSource];
        }
    }
}
