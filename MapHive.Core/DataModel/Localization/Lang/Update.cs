using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel
{
    public partial class Lang
    {
        /// <summary>
        /// Updates an object; returns an updated object or null if the object does not exist
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCtx"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected internal override async Task<T> UpdateAsync<T>(DbContext dbCtx, Guid uuid)
        {
            await ResetCurrentDefaultLangAsync(dbCtx);
            return await base.UpdateAsync<T>(dbCtx, uuid);
        }
    }
}
