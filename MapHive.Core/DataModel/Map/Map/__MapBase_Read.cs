using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DAL;
using MapHive.Core.DataModel;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel.Map
{
    public partial class MapBase
    {
        protected internal override async Task<T> ReadAsync<T>(DbContext dbCtx, Guid uuid, bool detached = true)
        {
            var obj = (MapBase)(Base)await base.ReadAsync<T>(dbCtx, uuid, detached);

            if (obj != null)
            {
                await obj.ReadLayersAsync(dbCtx);
            }

            return (T)(Base)obj;
        }

        /// <summary>
        /// Reads layers for the map
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        protected internal virtual async Task ReadLayersAsync(DbContext dbCtx)
        {
            throw new NotImplementedException("Override in order to assign proper layer types via ReadLayersAsync");
        }

        /// <summary>
        /// Reads layers for the map
        /// </summary>
        /// <typeparam name="TLayer"></typeparam>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        protected internal virtual async Task<List<TLayer>> ReadLayersAsync<TLayer>(DbContext dbCtx)
            where TLayer : LayerBase
        {
            var layers = new List<TLayer>();

            if (dbCtx is IMapDbContext iMapDbCtx)
            {
                layers = await iMapDbCtx.GetLayersDbSet<TLayer>().AsNoTracking()
                    .Where(l => l.MapId == Uuid)
                    .OrderBy(l => l.Order)
                    .ToListAsync();

                foreach (var t in layers)
                {
                    await t.ReadSourceLayer<TLayer>(dbCtx);
                    t.CleanSensitiveData();
                }
            }

            return layers;
        }
    }
}
