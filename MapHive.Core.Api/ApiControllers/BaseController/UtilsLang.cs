using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MapHive.Core.Configuration;
using MapHive.Core.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.Api.ApiControllers
{
    public abstract partial class BaseController
    {
        /// <summary>
        /// Tries to obtain a language of a request
        /// </summary>
        /// <returns></returns>
        protected string GetRequestLangCode(HttpContext context)
        {
            var request = context.Request;

            //first headers
            var lng = request.Headers[WebClientConfiguration.HeaderLang].ToString();

            //url test
            if (string.IsNullOrEmpty(lng))
            {
                lng = request.Query[WebClientConfiguration.LangParam];
            }

            //now it's the cookie time...
            //Note: no point in reading cookies here really, as by default the api is accessed via CORS, hence the default mh cookie is not likely to be accessible.
            //This is pretty much only going to be used if the client starts from the same endpoint as the api itself 
            if (string.IsNullOrEmpty(lng))
            {
                request.Cookies.TryGetValue(WebClientConfiguration.MhCookie, out var mhCookieStr);
                if (!string.IsNullOrEmpty(mhCookieStr))
                {
                    try
                    {
                        //assume the cooie to be json. if it does not deserialize to json, then it's a valid cookie
                        var mhCookie =

                            Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(mhCookieStr);

                        if (mhCookie.ContainsKey(WebClientConfiguration.LangParam))
                            lng = mhCookie[WebClientConfiguration.LangParam];
                    }
                    catch
                    {
                        //ignore
                    }

                }
            }

            //uhuh, no lang detected so far. inspect the request and the client langs
            if (string.IsNullOrEmpty(lng))
            {
                //based on https://stackoverflow.com/questions/49381843/get-browser-language-in-aspnetcore2-0
                var culture = context.Features.Get<IRequestCultureFeature>();
                lng = culture?.RequestCulture?.UICulture?.TwoLetterISOLanguageName.ToLower();
            }

            return lng;
        }

        /// <summary>
        /// grabs a default language
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        protected async Task<string> GetDefaultLangCodeAsync(MapHiveDbContext dbCtx)
        {
            return (await dbCtx.Langs.FirstOrDefaultAsync(l => l.IsDefault))?.LangCode;
        }
    }
}
