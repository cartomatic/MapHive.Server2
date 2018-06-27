using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;

#if NETFULL
using System.Data.Entity;
#endif
#if NETSTANDARD
using Microsoft.EntityFrameworkCore;
#endif

namespace MapHive.Core.DataModel
{
    public static partial class AppLocalization
    {
        /// <summary>
        /// Gets translations for the specified apps
        /// </summary>
        /// <typeparam name="TDbCtx"></typeparam>
        /// <param name="dbCtx"></param>
        /// <param name="langCode"></param>
        /// <param name="appNames"></param>
        /// <returns></returns>
        public static async Task<Dictionary<string, Dictionary<string, Dictionary<string, string>>>> GetAppLocalizationsAsync<TDbCtx>(TDbCtx dbCtx, string langCode, params string[] appNames)
            where TDbCtx : DbContext, ILocalized
        {
            return await GetAppLocalizationsAsync(dbCtx, new[] { langCode }, appNames);
        }

        /// <summary>
        /// Gets localizations for specified lang codes and apps
        /// </summary>
        /// <typeparam name="TDbCtx"></typeparam>
        /// <param name="dbCtx"></param>
        /// <param name="langCodes"></param>
        /// <param name="appNames"></param>
        /// <returns></returns>
        public static async Task<Dictionary<string, Dictionary<string, Dictionary<string, string>>>> GetAppLocalizationsAsync<TDbCtx>(TDbCtx dbCtx, IEnumerable<string> langCodes, IEnumerable<string> appNames)
            where TDbCtx : DbContext, ILocalized
        {
            var ret = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

            if (appNames == null || !appNames.Any())
                return ret;

            //grab the default lng
            var defaultLang = await Lang.GetDefaultLangAsync(dbCtx);

            if (langCodes == null || !langCodes.Any())
            {
                if (defaultLang == null)
                {
                    return ret;
                }

                //no langs provided, so the calling client may not be aware of the lang yet.
                //in this scenario just lookup the default lang and get the localization fot the default lng
                langCodes = new[] {defaultLang.LangCode};
            }


            //see if there is cache for the current combination already
            var cacheKey = GetClientLocalizationsCacheKey(langCodes, appNames);
            if (ClientLocalizationsCache.ContainsKey(cacheKey))
            {
                return (Dictionary<string, Dictionary<string, Dictionary<string, string>>>)ClientLocalizationsCache[cacheKey];
            }

            //fetch localizations if needed
            foreach (var appName in appNames)
            {
                if (!AppLocalizationsCache.ContainsKey(appName) || AppLocalizationsCache[appName] == null)
                {
                    //read all the Localization classes for the given AppName
                    var localizationClasses = await
                        dbCtx.LocalizationClasses.Where(lc => lc.ApplicationName == appName).ToListAsync();

                    //now grab the identifiers - need them in order to request translation keys. when using IQueryable, the range MUST BE static, simple types
                    //so even though compiler will not complain if a range is passes as localizationClasses.Select(lc => lc.Uuid) it will fail in the runtime
                    //saving this to a variable solves the issue!
                    var localizationClassesIdentifiers = localizationClasses.Select(lc => lc.Uuid);

                    var translationKeys = await
                        dbCtx.TranslationKeys.Where(
                            tk => localizationClassesIdentifiers.Contains(tk.LocalizationClassUuid)).ToListAsync();

                    AppLocalizationsCache[appName] = localizationClasses.GroupJoin(
                        translationKeys,
                        lc => lc.Uuid,
                        tk => tk.LocalizationClassUuid,
                        (lc, tk) => new LocalizationClass()
                        {
                            ApplicationName = lc.ApplicationName,
                            ClassName = lc.ClassName,
                            InheritedClassName = lc.InheritedClassName,
                            TranslationKeys = tk
                        }
                    );
                }

                var appLocalizations = AppLocalizationsCache[appName];

                foreach (var appL in appLocalizations)
                {
                    var key = $"{appL.ApplicationName}.{appL.ClassName}";
                    if (!ret.ContainsKey(key))
                    {
                        ret[key] = new Dictionary<string, Dictionary<string, string>>();
                    }
                    var classTranslations = ret[key];


                    foreach (var tk in appL.TranslationKeys)
                    {
                        if (!classTranslations.ContainsKey(tk.Key))
                        {
                            classTranslations[tk.Key] = new Dictionary<string, string>();
                        }

                        foreach (var translation in tk.Translations.Where(t => t.Key == defaultLang.LangCode || langCodes.Contains(t.Key)))
                        {
                            classTranslations[tk.Key].Add(translation.Key, translation.Value);
                        }
                    }
                }
            }

            //cahce the output
            ClientLocalizationsCache[cacheKey] = ret;

            return ret;
        }
    }
}
