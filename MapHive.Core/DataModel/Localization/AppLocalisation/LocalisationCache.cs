using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// App localizations records cache. invalidated on app localization create, update, destroy
        /// </summary>
        private static Dictionary<string, IEnumerable<LocalizationClass>> AppLocalizationsCache { get; set; } = new Dictionary<string, IEnumerable<LocalizationClass>>();


        /// <summary>
        /// Client localizations cache - cache of the localizations prepared for the client output
        /// </summary>
        private static Dictionary<string, object> ClientLocalizationsCache { get; set; } = new Dictionary<string, object>();
        //Q: how will this behave? Will the static live in app domain, so will be shared even when there are multiple worker processes?

        /// <summary>
        /// Caches guids => class names of the localization classes
        /// </summary>
        private static Dictionary<Guid, string> LocalizationClassClassNamesCache { get; set; } = new Dictionary<Guid, string>();

        /// <summary>
        /// Caches guids => app names of the localization classes
        /// </summary>
        private static Dictionary<Guid, string> LocalizationClassAppNamesCache { get; set; } = new Dictionary<Guid, string>();

        /// <summary>
        /// Caches guids => fully qualified class names of the localization classes
        /// </summary>
        private static Dictionary<Guid, string> LocalizationClassFullClassNamesCache { get; set; } = new Dictionary<Guid, string>();

        /// <summary>
        /// Ivalidates app localizations cache
        /// </summary>
        /// <param name="appName"></param>
        public static void InvalidateAppLocalizationsCache(string appName)
        {
            InvalidateAppLocalizationsCache(appName, string.Empty);
        }

        /// <summary>
        /// Ivalidates app localizations cache
        /// </summary>
        /// <param name="appName"></param>
        public static void InvalidateAppLocalizationsCache(string appName, string className)
        {
            //app localizations cache first

            if (AppLocalizationsCache.ContainsKey(appName) && string.IsNullOrEmpty(className))
            {
                AppLocalizationsCache.Remove(appName);
            }
            else if (AppLocalizationsCache.ContainsKey(appName))
            {
                AppLocalizationsCache.Remove(appName);

                //FIXME - this requires better reading - need to read from db to 'top up' the cache

                //var localizations = AppLocalizationsCache[appName].ToList();

                //var toBeRemoved = localizations.FirstOrDefault(l => l.ClassName == className);
                //if(toBeRemoved != null)
                //    localizations.Remove(toBeRemoved);

                //AppLocalizationsCache[appName] = localizations;
            }

            //clients cache too...
            var keysToInvalidate = ClientLocalizationsCache.Keys.Where(k => k.IndexOf(appName) > -1).ToList();
            foreach (var key in keysToInvalidate)
            {
                ClientLocalizationsCache.Remove(key);
            }
        }

        /// <summary>
        /// Generates a cache key for the client localizations
        /// </summary>
        /// <param name="langCodes"></param>
        /// <param name="appNames"></param>
        /// <returns></returns>
        public static string GetClientLocalizationsCacheKey(IEnumerable<string> langCodes, IEnumerable<string> appNames)
        {
            return $"{string.Join("_", appNames.OrderBy(s => s))}___{string.Join("_", langCodes.OrderBy(s => s))}";
        }


        /// <summary>
        /// Gets a LocalizationClass ClassName by the object uuid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCtx"></param>
        /// <param name="localizationClassIdentifier"></param>
        /// <returns></returns>
        public static async Task<string> GetLocalizationClassClassNameAsync<T>(T dbCtx, Guid localizationClassIdentifier)
            where T : DbContext
        {
            if (LocalizationClassClassNamesCache.ContainsKey(localizationClassIdentifier))
            {
                return LocalizationClassClassNamesCache[localizationClassIdentifier];
            }

            var localisedDbCtx = (ILocalized)dbCtx;

            var className =
                (await localisedDbCtx.LocalizationClasses.FirstOrDefaultAsync(lc => lc.Uuid == localizationClassIdentifier))?
                    .ClassName;

            //cache the output
            if(!string.IsNullOrEmpty(className))
                LocalizationClassClassNamesCache[localizationClassIdentifier] = className;

            return className;
        }

        /// <summary>
        /// Gets a LocalizationClass ClassName by the object uuid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCtx"></param>
        /// <param name="localizationClassIdentifier"></param>
        /// <returns></returns>
        public static async Task<string> GetLocalizationClassAppNameAsync<T>(T dbCtx, Guid localizationClassIdentifier)
            where T : DbContext
        {
            if (LocalizationClassAppNamesCache.ContainsKey(localizationClassIdentifier))
            {
                return LocalizationClassAppNamesCache[localizationClassIdentifier];
            }

            var localisedDbCtx = (ILocalized)dbCtx;

            var appName =
                (await localisedDbCtx.LocalizationClasses.FirstOrDefaultAsync(lc => lc.Uuid == localizationClassIdentifier))?
                .ApplicationName;

            //cache the output
            if (!string.IsNullOrEmpty(appName))
                LocalizationClassAppNamesCache[localizationClassIdentifier] = appName;

            return appName;
        }


        /// <summary>
        /// Gets a LocalizationClass full ClassName by the object uuid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCtx"></param>
        /// <param name="localizationClassIdentifier"></param>
        /// <returns></returns>
        public static async Task<string> GetLocalizationClassFullClassNameAsync<T>(T dbCtx, Guid localizationClassIdentifier)
            where T : DbContext
        {
            if (LocalizationClassFullClassNamesCache.ContainsKey(localizationClassIdentifier))
            {
                return LocalizationClassFullClassNamesCache[localizationClassIdentifier];
            }

            var localizedDbCtx = (ILocalized)dbCtx;
            var localizationClass = await localizedDbCtx.LocalizationClasses.FirstOrDefaultAsync(
                lc => lc.Uuid == localizationClassIdentifier);
            var fullClassName = $"{localizationClass?.ApplicationName}.{localizationClass?.ClassName}";

            //cache the output
            if (!string.IsNullOrEmpty(fullClassName))
                LocalizationClassFullClassNamesCache[localizationClassIdentifier] = fullClassName;

            return fullClassName;
        }
    }
}
