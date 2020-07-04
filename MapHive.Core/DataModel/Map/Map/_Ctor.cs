using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MapHive.Core.DataModel;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel.Map
{
    /// <summary>
    /// Project exposes some data specified by a customer.
    /// </summary>
    public partial class Map: MapBase
    {
        static Map()
        {
            BaseObjectTypeIdentifierExtensions.RegisterTypeIdentifier(MethodInfo.GetCurrentMethod().DeclaringType, Guid.Parse("87f949bd-c9ed-4356-af66-51c1a067d674"));
        }

        public new List<Layer> Layers { get; set; }

        protected internal virtual async Task ReadLayersAsync(DbContext dbCtx)
        {
            Layers = await ReadLayersAsync<Layer>(dbCtx);
        }

        protected override async Task CleanUpLayers(DbContext dbCtx, Guid mapId)
        {
            await CleanUpLayers<Layer>(dbCtx, mapId);
        }

        protected override async Task HandleLayers(DbContext dbCtx, Guid mapId)
        {
            await HandleLayers<Layer>(dbCtx, mapId);
        }

        protected override List<T> GetLayers<T>()
        {
            return this.Layers.Select(l => (T)(Base)l).ToList();
        }
    }
}
