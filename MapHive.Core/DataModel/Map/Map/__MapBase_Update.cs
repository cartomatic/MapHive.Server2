using System;
using System.Reflection;
using System.Threading.Tasks;
using MapHive.Core.DataModel;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel.Map
{
    public partial class MapBase
    {
        protected internal override async Task<T> UpdateAsync<T>(DbContext dbCtx, Guid uuid)
        {
            //need permalink for a map - this is a 'self fix' for recs that do not have one
            if (!Permalink.HasValue)
                Permalink = Guid.NewGuid();

            var obj = await base.UpdateAsync<T>(dbCtx, uuid);

            await HandleLayers(dbCtx, uuid);

            return obj;
        }
    }
}
