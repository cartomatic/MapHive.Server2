using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel.Map
{
    public partial class MapBase
    {
        /// <summary>
        /// Customized map delete procedure. Handles cleanup of all the related resources too.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCtx"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected internal override async Task<T> DestroyAsync<T>(DbContext dbCtx, Guid uuid)
        {
            await CleanUpLayers(dbCtx, uuid);

            return await base.DestroyAsync<T>(dbCtx, uuid);
        }
    }
}
