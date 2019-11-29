using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DataModel;
using Microsoft.EntityFrameworkCore;


namespace MapHive.Core.DataModel.Map
{
    public abstract partial class LayerBase
    {
        /// <summary>
        /// Customised read procedure that also reads a source layer for the returned object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCtx"></param>
        /// <param name="uuid"></param>
        /// <param name="detached"></param>
        /// <returns></returns>
        protected internal override async Task<T> ReadAsync<T>(DbContext dbCtx, Guid uuid, bool detached = true)
        {
            throw new Exception("Override or Use the DestroyAsync<T, TLayer, TDataStore> method instead!");
        }

        protected internal virtual async Task<TLayer> ReadWithExtrasAsync<TLayer>(DbContext dbCtx, Guid uuid, bool detached = true)
            where TLayer : LayerBase
        {
            var obj = await base.ReadAsync<TLayer>(dbCtx, uuid, detached);

            await obj.ReadSourceLayer<TLayer>(dbCtx);

            return obj;
        }
    }
}
