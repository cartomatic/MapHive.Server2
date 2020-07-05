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

        /// <summary>
        /// Need a new Layers property in order to specify the actually used type for layers collection
        /// </summary>
        public new List<Layer> Layers { get; set; }

        /// <inheritdoc cref="MapBase.GetLayers{T}"/>
        protected override List<T> GetLayers<T>()
        {
            return (this.Layers ?? new List<Layer>()).Select(l => (T)(Base)l).ToList();
        }

        /// <inheritdoc cref="MapBase.ReadLayersAsync"/>
        protected internal override async Task ReadLayersAsync(DbContext dbCtx)
        {
            Layers = await ReadLayersAsync<Layer>(dbCtx);
        }

        /// <inheritdoc cref="MapBase.CleanUpLayers"/>
        protected override async Task CleanUpLayers(DbContext dbCtx, Guid mapId)
        {
            await CleanUpLayers<Layer>(dbCtx, mapId);
        }

        /// <inheritdoc cref="MapBase.HandleLayers"/>
        protected override async Task HandleLayers(DbContext dbCtx, Guid mapId)
        {
            await HandleLayers<Layer>(dbCtx, mapId);
        }
    }
}
