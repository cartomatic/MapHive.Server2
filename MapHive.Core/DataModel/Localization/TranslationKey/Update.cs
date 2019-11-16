using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DAL;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using static MapHive.Core.DataModel.AppLocalization;

using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel
{
    public partial class TranslationKey
    {
        /// <summary>
        /// Updates an object; returns an updated object or null if the object does not exist
        /// invalidates app localizations cache, so client localizations will be regenerated on the next read
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCtx"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected internal override async Task<T> UpdateAsync<T>(DbContext dbCtx, Guid uuid)
        {
            InvalidateAppLocalizationsCache(await GetLocalizationClassAppNameAsync(dbCtx, LocalizationClassUuid), await GetLocalizationClassClassNameAsync(dbCtx, LocalizationClassUuid));

            var lc = await base.UpdateAsync<T>(dbCtx, uuid);

            //if not marked as overwriting, try to obtain inherited value
            if (Overwrites != true)
            {
                await ObtainInheritedTranslations(dbCtx, this);
            }

            //always propagate up the inheritance stack
            await PropagateChangeUpTheInheritanceStack(dbCtx, this);

            return lc;
        }

        /// <summary>
        /// Silently updates translation key without triggering the inheritance chain propagations 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCtx"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected internal async Task<T> SilentUpdateAsync<T>(DbContext dbCtx)
            where T : Base
        {
            return await base.UpdateAsync<T>(dbCtx, this.Uuid);
        }

        /// <summary>
        /// Propagates change up the inheritance stack, so translations in inheriting classes are updated;
        /// propagation stops when an inheriting translation overwrites inherited value
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <param name="tk"></param>
        /// <returns></returns>
        protected internal static async Task PropagateChangeUpTheInheritanceStack(DbContext dbCtx, TranslationKey tk)
        {
            var appName = await GetLocalizationClassAppNameAsync(dbCtx, tk.LocalizationClassUuid);
            var className = await GetLocalizationClassClassNameAsync(dbCtx, tk.LocalizationClassUuid);

            var localizedDbCtx = dbCtx as ILocalizedDbContext;
            var inheritingClasses = await (localizedDbCtx).LocalizationClasses.AsNoTracking()
                .Where(lc => lc.InheritedClassName == $"{appName}.{className}Localization").ToListAsync();

            foreach (var inheritingClass in inheritingClasses)
            {
                //get inheriting tk
                var inheritingTk = await localizedDbCtx.TranslationKeys.FirstOrDefaultAsync(x =>
                    x.LocalizationClassUuid == inheritingClass.Uuid && x.Key == tk.Key);

                //no key, or key overwrites - stop, as no point, as this breaks inheritance chain
                if(inheritingTk == null || inheritingTk.Overwrites == true)
                    continue;

                inheritingTk.Translations = tk.Translations;
                await inheritingTk.SilentUpdateAsync<TranslationKey>(dbCtx);

                await PropagateChangeUpTheInheritanceStack(dbCtx, inheritingTk);
            }
        }

        /// <summary>
        /// Obtains inherited translations
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        protected internal static async Task ObtainInheritedTranslations(DbContext dbCtx, TranslationKey tk)
        {
            var localizedDbCtx = dbCtx as ILocalizedDbContext;

            var localizationClass = await localizedDbCtx.LocalizationClasses.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Uuid == tk.LocalizationClassUuid);


            //drill one level down as assuming that it should be actual
            if (localizationClass != null && !string.IsNullOrWhiteSpace(localizationClass.InheritedClassName))
            {
                var inheritedLocalizationClass = await localizedDbCtx.LocalizationClasses.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ApplicationName + x.ClassName + "Localization" == localizationClass.InheritedClassName);

                if (inheritedLocalizationClass == null)
                    return;

                var inheritedTk = await localizedDbCtx.TranslationKeys.FirstOrDefaultAsync(x =>
                    x.LocalizationClassUuid == inheritedLocalizationClass.Uuid && x.Key == tk.Key);

                if (inheritedTk == null)
                    return;

                tk.Translations = inheritedTk.Translations;

                await tk.SilentUpdateAsync<TranslationKey>(dbCtx);
            }

        }
    }
}
