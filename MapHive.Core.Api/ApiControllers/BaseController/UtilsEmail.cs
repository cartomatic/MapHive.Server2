using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Email;
using MapHive.Core.DAL;
using Microsoft.Extensions.Configuration;

namespace MapHive.Core.Api.ApiControllers
{
    public abstract partial class BaseController
    {
        /// <summary>
        /// Extracts an email template and a configured email account based on the email template identifier and app identifier; if application name is not provided it defaults to "common" as such templates should always be provided by the maphive core; Tries to work out the lang to extract the translation for email dynamically
        /// </summary>
        /// <param name="emailIdentifier"></param>
        /// <param name="appName"></param>
        /// <param name="langCode"></param>
        /// <returns></returns>
        protected async Task<(EmailAccount emailAccount, EmailTemplate emailTemplate)> GetEmailStuffAsync(string emailIdentifier, string appName = null, string langCode = null)
        {
            EmailAccount ea = null;
            EmailTemplate et = null;

            var localisedEmail = await RestApiCall<Cartomatic.Utils.Email.EmailTemplate>(
                Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig()["Endpoints:Localization"],
                "emailtemplatelocalizations/emailtemplate",
                queryParams: new Dictionary<string, object>
                {
                    { "emailIdentifier" , emailIdentifier},
                    { "appIdentifier" , string.IsNullOrEmpty(appName) ? "common" : appName },
                    {
                        "langCode" , string.IsNullOrEmpty(langCode) ? GetRequestLangCode(Context) : langCode
                    }
                }
            );

            //silently read cfg
            try
            {
                var cfg = Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig();
                ea = cfg.GetSection("ServiceEmail").Get<EmailAccount>();
            }
            catch
            {
                //ignore
            }

            //sooo, if there is a localised email and such can prepare the actual template
            if (localisedEmail?.Output != null && ea != null)
            {
                et = new Cartomatic.Utils.Email.EmailTemplate
                {
                    Title = localisedEmail.Output?.Title,
                    Body = localisedEmail.Output?.Body,
                    IsBodyHtml = localisedEmail.Output.IsBodyHtml
                };
            }

            return (ea, et);
        }
        
    }
}
