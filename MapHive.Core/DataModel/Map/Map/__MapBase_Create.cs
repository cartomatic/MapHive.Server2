using System;
using System.Reflection;
using System.Threading.Tasks;
using MapHive.Core.DataModel;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel.Map
{
    public partial class MapBase
    {
        protected internal override async Task<T> CreateAsync<T>(DbContext dbCtx)
        {
            //need permalink for a new project
            Permalink = Guid.NewGuid();

            var obj = await base.CreateAsync<T>(dbCtx);

            //need to perform update prior to calling layers handler, so can have a proper project uuid context for links handling
            await HandleLayers(dbCtx, obj.Uuid);

            return obj;
        }
    }
}
