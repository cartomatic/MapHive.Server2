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
            var lng = request.Headers[WebClientConfiguration.HeaderLang];

            //url test
            if (string.IsNullOrEmpty(lng))
            {
                lng = request.Query[WebClientConfiguration.LangParam];
            }
           
            //now it's the cookie time
            if (string.IsNullOrEmpty(lng))
            {
                //TODO!
                //FIXME - need to check this, as not sure how the behavior changed in aspnetcore
                var cookies = request.Cookies[WebClientConfiguration.MhCookie];
                //lng = cookie //[WebClientConfiguration.LangParam];
            }

            //uhuh, no lang detected so far. inspect the request and the client langs
            if (string.IsNullOrEmpty(lng))
            {
                //based on https://stackoverflow.com/questions/49381843/get-browser-language-in-aspnetcore2-0
                //TODO - need to add middlewar in unified API setuper!!!

                context.Features.Get<IRequestCultureFeature>();
                //if (request.User. .UserLanguages?.Length > 0)
                //{
                //    lng = request.UserLanguages[0].Substring(0, 2).ToLower();
                //}
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
