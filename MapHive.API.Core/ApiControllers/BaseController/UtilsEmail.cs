﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Email;
using MapHive.Core.DAL;

namespace MapHive.Api.Core.ApiControllers
{
    public abstract partial class BaseController
    {
        /// <summary>
        /// Extracts an email template and a configured email account based on the email identifier. Since application name is not provided it only matches email identifiers for templates that do not have the app name set. Tries to work out the lang to extract the translation for email dynamically
        /// </summary>
        /// <param name="emailIdentifier"></param>
        /// <returns></returns>
        protected async Task<(EmailAccount emailAccount, EmailTemplate emailTemplate)> GetEmailStuffAsync(string emailIdentifier)
        {
            return await this.GetEmailStuffAsync(emailIdentifier, "common");
        }

        /// <summary>
        /// Extracts an email template and a configured email account based on the email template identifier and app identifier; Tries to work out the lang to extract the translation for email dynamically
        /// </summary>
        /// <param name="emailIdentifier"></param>
        /// <param name="appName"></param>
        /// <param name="langCode"></param>
        /// <returns></returns>
        protected async Task<(EmailAccount emailAccount, EmailTemplate emailTemplate)> GetEmailStuffAsync(string emailIdentifier, string appName, string langCode = null)
        {
            EmailAccount ea = null;
            EmailTemplate et = null;

            var localisedEmail = await RestApiCall<Cartomatic.Utils.Email.EmailTemplate>(
                Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig()["LocalizationApiEndpoint"],
                "emailtemplates",
                queryParams: new Dictionary<string, object>
                {
                    { "emailIdentifier" , emailIdentifier},
                    { "appName" , appName},
                    {
                        "langCode" , string.IsNullOrEmpty(langCode) ? GetRequestLangCode(Context) : langCode
                    }
                }
            );

            //silently read cfg
            try
            {
                ea = EmailAccount.FromJson(ConfigurationManager.AppSettings["EmailSender"]);
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