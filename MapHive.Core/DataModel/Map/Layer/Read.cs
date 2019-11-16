using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DataModel;
using Microsoft.EntityFrameworkCore;


namespace MapHive.Core.DataModel.Map
{
    public partial class Layer
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
            var obj = (Layer)(Base) await base.ReadAsync<T>(dbCtx, uuid, detached);

            await obj.ReadSourceLayer(dbCtx);

            return (T)(Base)obj;
        }
    }
}
