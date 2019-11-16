using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DAL;
using MapHive.Core.DataModel;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel.Map
{
    public abstract partial class LayerBase
    {
        protected internal override async Task<T> CreateAsync<T>(DbContext dbCtx)
        {
            if (!(dbCtx is IMapDbContext))
                throw new ArgumentException($"DbCtx is expected to implement {nameof(IMapDbContext)}");

            var mapDbCtx = (IMapDbContext)dbCtx;

            //if the layer has its data store, make sure to mark the data store as in use, so it does not get deleted
            if (DataStoreId.HasValue)
            {
                var dataStore = await mapDbCtx.DataStores.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Uuid == DataStoreId);

                if (dataStore != null)
                {
                    dataStore.InUse = true;
                    await dataStore.UpdateAsync(dbCtx);

                    //also check out the liked data stores
                    if (dataStore.LinkedDataStoreIds != null)
                    {
                        foreach (var linkedDataStoreId in dataStore.LinkedDataStoreIds)
                        {
                            var linkedDataStore = await mapDbCtx.DataStores.AsNoTracking()
                                .FirstOrDefaultAsync(x => x.Uuid == linkedDataStoreId);
                            if (linkedDataStore != null)
                            {
                                linkedDataStore.InUse = true;
                                await linkedDataStore.UpdateAsync(dbCtx);
                            }
                        }
                    }
                }
            }

            return (T)(Base)(await base.CreateAsync<Layer>(dbCtx));
        }
    }
}
