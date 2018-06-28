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
    public partial class LocalizationClass
    {
        /// <summary>
        /// Creates an object; returns a created object or null if it was not possible to create it due to the fact a uuid is already reserved
        /// invalidates app localizations cache, so client localizations will be regenerated on the next read
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        protected internal override async Task<T> CreateAsync<T>(DbContext dbCtx)
        {
            InvalidateAppLocalizationsCache(ApplicationName);
            return await base.CreateAsync<T>(dbCtx);
        }
    }
}
