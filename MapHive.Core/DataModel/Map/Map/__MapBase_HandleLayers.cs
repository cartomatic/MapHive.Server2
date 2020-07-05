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
    public partial class MapBase
    {
        /// <summary>
        /// returns a layers collection with the type specified
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected virtual List<T> GetLayers<T>()
            where T : LayerBase
        {
            throw new NotImplementedException("Need to override to obtain layers from a proper type! Layers property is hidden when using customized laeyer types.");
        }

        /// <summary>
        /// Handles proper management of map layers; called on create /update
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <param name="mapId"></param>
        /// <returns></returns>
        protected virtual async Task HandleLayers(DbContext dbCtx, Guid mapId)
        {
            throw new NotImplementedException("Need to override to call a generic HandleLayers with a proper type!");
        }

        /// <summary>
        /// Handles proper management of map layers; called on create /update
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <param name="mapId"></param>
        /// <returns></returns>
        protected virtual async Task HandleLayers<T>(DbContext dbCtx, Guid mapId)
            where T : LayerBase
        {
            //Need to:
            //work out layers to delete, update, create and do so...

            if (!(dbCtx is IMapDbContext iMapDbContext))
                return;

            var layers = GetLayers<T>();

            var currentLayers = await iMapDbContext.GetLayersDbSet<T>().AsNoTracking().Where(l => l.MapId == mapId).ToListAsync();
            var layersToDestroy = currentLayers.Where(l => layers.All(ll => ll.Uuid != l.Uuid)).ToList();
            
            var layersToUpdate = layers.Where(l => currentLayers.Any(ll => ll.Uuid == l.Uuid)).ToList();
            var layersToCreate = layers.Where(l => currentLayers.All(ll => ll.Uuid != l.Uuid));

            //enforce some layer defaults
            for (var l = 0; l < layers.Count; l++)
            {
                //assume whatever order has been sent from the clientside should be maintained
                layers[l].Order = l;

                //apply map id as when a new project is created a client is not able to determine the id
                layers[l].MapId = mapId;
            }

            foreach (var l in layersToDestroy)
            {
                await l.DestroyAsync(dbCtx);
            }

            foreach (var l in layersToUpdate)
            {
                await l.UpdateAsync(dbCtx);
            }

            foreach (var l in layersToCreate)
            {
                await l.CreateAsync(dbCtx);
            }
        }

        /// <summary>
        /// Cleans up all the layers for given map
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <param name="mapId"></param>
        /// <returns></returns>
        protected virtual async Task CleanUpLayers(DbContext dbCtx, Guid mapId)
        {
            throw new NotImplementedException("Need to override to call a generic CleanUpLayers with a proper type!");
        }

        /// <summary>
        /// Cleans up all the layers for given map
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <param name="mapId"></param>
        /// <returns></returns>
        protected async Task CleanUpLayers<T>(DbContext dbCtx, Guid mapId)
            where T : LayerBase
        {
            if (dbCtx is IMapDbContext iMapDbContext)
            {
                iMapDbContext.GetLayersDbSet<T>().RemoveRange(
                    iMapDbContext.GetLayersDbSet<T>().Where(l => l.MapId == mapId)
                );

                await dbCtx.SaveChangesAsync();
            }
        }


    }
}
