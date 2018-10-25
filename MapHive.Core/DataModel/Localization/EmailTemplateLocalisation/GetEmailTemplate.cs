using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DAL;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel
{
    public partial class EmailTemplateLocalization
    {
        /// <summary>
        /// Gets app specific email template in given language;
        /// when template for the language specified is not found it defaults to a template in a default enf language and if it is not defined a first template translation (if any!) is returned
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <param name="emailIdentifier"></param>
        /// <param name="appIdentifier"></param>
        /// <param name="langCode"></param>
        /// <returns></returns>
        public static async Task<Cartomatic.Utils.Email.EmailTemplate> GetEmailTemplate(ILocalizedDbContext dbCtx, string emailIdentifier,
            string appIdentifier, string langCode)
        {
            //ignore when there is no email identifier as it would be a bit pointless really...
            if (string.IsNullOrWhiteSpace(emailIdentifier))
                return null;


            //if app has not been specified, use a 'common' identifier for email template - maphive specific rather than app specific
            if (string.IsNullOrWhiteSpace(appIdentifier))
                appIdentifier = "common";

            var emailTemplateLocalization = await dbCtx.EmailTemplates.FirstOrDefaultAsync(etl =>
                etl.Identifier == emailIdentifier && etl.ApplicationIdentifier == appIdentifier);

            if (emailTemplateLocalization == null)
                return null;

            //default lang code so can try email localization for default lang if not present for the requested lang
            var defaultLangCode = await dbCtx.Langs.FirstOrDefaultAsync(l => l.IsDefault);


            EmailTemplate et = null;
            if (emailTemplateLocalization.Translations.ContainsKey(langCode))
            {
                et = emailTemplateLocalization.Translations[langCode];
            }
            else if (defaultLangCode != null && emailTemplateLocalization.Translations.ContainsKey(defaultLangCode.LangCode))
            {
                et = emailTemplateLocalization.Translations[defaultLangCode.LangCode];
            }
            else
            {
                //no email template for neither requested lang nor default lang
                //is there an email template at all???
                if (emailTemplateLocalization.Translations.Count > 0)
                {
                    et = emailTemplateLocalization.Translations.First().Value;
                }
            }

            return new Cartomatic.Utils.Email.EmailTemplate
            {
                Title = et.Title,
                Body = et.Body,
                IsBodyHtml = emailTemplateLocalization.IsBodyHtml
            };
        }
    }
}
