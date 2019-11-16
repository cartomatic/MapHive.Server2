using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DAL;
using Microsoft.EntityFrameworkCore;


namespace MapHive.Core.DataModel.Map
{
    public partial class Layer
    {
        /// <summary>
        /// Reads a source layer for this layer
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        public async Task ReadSourceLayer(DbContext dbCtx)
        {
            if (!(dbCtx is IMapDbContext))
                throw new ArgumentException($"DbCtx is expected to implement {nameof(IMapDbContext)}");

            var mapDbCtx = (IMapDbContext)dbCtx;

            if (SourceLayerId.HasValue)
            {
                SourceLayer = await mapDbCtx.Layers.AsNoTracking().FirstOrDefaultAsync(l => l.Uuid == SourceLayerId);
            }
        }
    }
}
