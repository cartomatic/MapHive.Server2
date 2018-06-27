using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DAL;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using static MapHive.Core.DataModel.AppLocalization;

#if NETFULL
using System.Data.Entity;
#endif
#if NETSTANDARD
using Microsoft.EntityFrameworkCore;
#endif

namespace MapHive.Core.DataModel
{
    public partial class LocalizationClass
    {
        /// <summary>
        /// Destroys an object; returns destroyed object or null in a case it has not been found
        /// invalidates app localizations cache, so client localizations will be regenerated on the next read
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCtx"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected internal override async Task<T> DestroyAsync<T>(DbContext dbCtx, Guid uuid)
        {
            InvalidateAppLocalizationsCache(await GetLocalizationClassAppNameAsync(dbCtx, uuid), await GetLocalizationClassClassNameAsync(dbCtx, uuid));

            //need to destroy all the translation keys too...
            var localisedDbCtx = (ILocalizedDbContext) dbCtx;
            localisedDbCtx.TranslationKeys.RemoveRange(
                localisedDbCtx.TranslationKeys.Where(tk => tk.LocalizationClassUuid == uuid));

            await dbCtx.SaveChangesAsync();

            return await base.DestroyAsync<T>(dbCtx, uuid);    
        }
    }
}
