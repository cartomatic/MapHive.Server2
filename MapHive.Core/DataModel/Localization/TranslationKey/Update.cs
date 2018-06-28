using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            return await base.UpdateAsync<T>(dbCtx, uuid);
        }
    }
}
