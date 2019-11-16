using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DAL;
using MapHive.Core.DataModel;
using Microsoft.EntityFrameworkCore;


namespace MapHive.Core.DataModel.Map
{
    public abstract partial class LayerBase
    {
        /// <summary>
        /// Customized layer delete that takes sure of cleaning up all the child layers too.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCtx"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected internal override async Task<T> DestroyAsync<T>(DbContext dbCtx, Guid uuid)
        {
            if(!(dbCtx is IMapDbContext))
                throw new ArgumentException($"DbCtx is expected to implement {nameof(IMapDbContext)}");

            var mapDbCtx = (IMapDbContext) dbCtx;

            //grab self - need to prevent deleting default layers!
            var self = await mapDbCtx.Layers.AsNoTracking().FirstOrDefaultAsync(l => l.Uuid == uuid);
            if (self.IsDefault == true)
                throw MapHive.Core.DataModel.Validation.Utils.GenerateValidationFailedException(
                    $"{nameof(Metadata)}.{nameof(IsDefault)}", "is_default_delete", "Cannot delete a default layer"
                );

            mapDbCtx.Layers.RemoveRange(
                mapDbCtx.Layers.Where(l => l.SourceLayerId == uuid)
            );

            if (DataStoreId.HasValue)
            {
                var dataStore = await mapDbCtx.DataStores.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Uuid == DataStoreId);

                if (dataStore != null)
                {
                    dataStore.InUse = false;
                    await dataStore.UpdateAsync(dbCtx);

                    //also check linked data stores
                    if (dataStore.LinkedDataStoreIds != null)
                    {
                        foreach (var linkedDataStoreId in dataStore.LinkedDataStoreIds)
                        {
                            var linkedDataStore = await mapDbCtx.DataStores.AsNoTracking()
                                .FirstOrDefaultAsync(x => x.Uuid == linkedDataStoreId);
                            if (linkedDataStore != null)
                            {
                                linkedDataStore.InUse = false;
                                await linkedDataStore.UpdateAsync(dbCtx);
                            }
                        }
                    }
                }
            }

            await dbCtx.SaveChangesAsync();

            return await base.DestroyAsync<T>(dbCtx, uuid);
        }
    }
}
