using System;
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
    public partial class TranslationKey
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
            //need to read self first
            var translationKey = await (dbCtx as ILocalized).TranslationKeys.FirstOrDefaultAsync(tk => tk.Uuid == uuid);
            InvalidateAppLocalizationsCache(await GetLocalizationClassAppNameAsync(dbCtx, translationKey.LocalizationClassUuid), await GetLocalizationClassClassNameAsync(dbCtx, translationKey.LocalizationClassUuid));
            return await base.DestroyAsync<T>(dbCtx, uuid);    
        }
    }
}
