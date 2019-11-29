using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DAL;
using Microsoft.EntityFrameworkCore;


namespace MapHive.Core.DataModel.Map
{
    public abstract partial class LayerBase
    {
        /// <summary>
        /// Reads a source layer for this layer
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        public async Task ReadSourceLayer<TLayer>(DbContext dbCtx)
            where TLayer : LayerBase
        {
            if (SourceLayerId.HasValue && dbCtx is IMapDbContext mapDbCtx)
            {
                SourceLayer = await mapDbCtx.GetLayersDbSet<TLayer>().AsNoTracking().FirstOrDefaultAsync(l => l.Uuid == SourceLayerId);
            }
        }
    }
}
