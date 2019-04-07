using MapHive.Core.Configuration;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace MapHive.Core.Api.ApiControllers
{
    public abstract partial class BaseController
    {
        /// <summary>
        /// Extracts a source header off a request. Source header is used by the MH env to pass a full request source including hash, because hash is never sent to the client
        /// </summary>
        /// <returns></returns>
        protected string GetRequestSource(HttpContext context)
        {
            return context.Request.Headers[WebClientConfiguration.HeaderSource];
        }

        /// <summary>
        /// extracts app identifier off the client url
        /// </summary>
        /// <returns></returns>
        protected string GetRequestAppIdentifier()
        {
            return ExtractRequesToken(UrlAppTokenDelimiter);
        }

        /// <summary>
        /// extracts org identifier off the client url
        /// </summary>
        /// <returns></returns>
        protected string GetRequestOrgIdentifier()
        {
            return ExtractRequesToken(UrlOrgTokenDelimiter);
        }

        /// <summary>
        /// app url token delimiter
        /// </summary>
        protected const string UrlAppTokenDelimiter = "!";

        /// <summary>
        /// org url token delimiter
        /// </summary>
        protected const string UrlOrgTokenDelimiter = "@";

        /// <summary>
        /// extracts a given doken off the url based on the token delimiter
        /// </summary>
        /// <param name="tokenDelimiter"></param>
        /// <returns></returns>
        protected string ExtractRequesToken(string tokenDelimiter)
        {
            var token = string.Empty;
            foreach (var urlPart in GetRequestSource(HttpContext).Split('#')[0].Split('/'))
            {
                if (urlPart.StartsWith(tokenDelimiter))
                {
                    token = urlPart.Replace(tokenDelimiter, "");
                    break;
                }
            }
            return token;
        }

        /// <summary>
        /// Extracts an application Url from the Src header appended by the client
        /// </summary>
        /// <returns></returns>
        protected string GetAppUrl()
        {
            //app url is pretty much the full path but without the hash and params
            //need to remove the app / org tokens too
            var urlParts = GetRequestSource(HttpContext).Split('#')[0].Split('?')[0].Split('/');

            return string.Join("/", urlParts.Where(str => !(str.StartsWith(UrlAppTokenDelimiter) || str.StartsWith(UrlOrgTokenDelimiter))));
        }
    }
}
