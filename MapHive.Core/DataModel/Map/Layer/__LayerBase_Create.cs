using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DAL;
using MapHive.Core.DataModel;
using Microsoft.EntityFrameworkCore;
using MimeKit.Encodings;

namespace MapHive.Core.DataModel.Map
{
    public abstract partial class LayerBase
    {
        protected internal override async Task<T> CreateAsync<T>(DbContext dbCtx)
        {
            throw new Exception("Override or Use the CreateAsync<T, TDataStore> method instead!");
        }

        protected internal virtual async Task<TLayer> CreateAsync<TLayer, TDataStore>(DbContext dbCtx)
            where TLayer : LayerBase
            where TDataStore : DataStore
        {

            //if the layer has its data store, make sure to mark the data store as in use, so it does not get deleted
            if (DataStoreId.HasValue && dbCtx is IMapDbContext mapDbCtx)
            {
                var dataStoresDbSet = mapDbCtx.GetDataStoresDbSet<TDataStore>().AsNoTracking();
                var dataStore = await dataStoresDbSet.FirstOrDefaultAsync(x => x.Uuid == DataStoreId);

                if (dataStore != null)
                {
                    dataStore.InUse = true;
                    await dataStore.UpdateAsync(dbCtx);

                    //also check out the liked data stores
                    if (dataStore.LinkedDataStoreIds != null)
                    {
                        foreach (var linkedDataStoreId in dataStore.LinkedDataStoreIds)
                        {
                            var linkedDataStore = await dataStoresDbSet.FirstOrDefaultAsync(x => x.Uuid == linkedDataStoreId);
                            if (linkedDataStore != null)
                            {
                                linkedDataStore.InUse = true;
                                await linkedDataStore.UpdateAsync(dbCtx);
                            }
                        }
                    }
                }
            }

            return await base.CreateAsync<TLayer>(dbCtx);
        }
    }
}
