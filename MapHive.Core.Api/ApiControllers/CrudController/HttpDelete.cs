using MapHive.Core.DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
using System.Threading.Tasks;

namespace MapHive.Core.Api.ApiControllers
{
    public abstract partial class CrudController<T, TDbCtx>
    {
        /// <summary>
        /// Default delete action
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="db">DbContext to be used; when not provided a default instance of TDbCtx will be used</param>
        /// <returns></returns>
        protected virtual async Task<IActionResult> DeleteAsync(Guid uuid, DbContext db = null)
        {
            //enforced at the filter action attribute level for db ctx obtained from _dbCtx so just testing if passed dbCtx is different
            if (db != null && db != _dbCtx)
                if (!await IsCrudPrivilegeGrantedForDestroyAsync(db))
                    return NotAllowed();

            return await DestroyAsync(db ?? _dbCtx, uuid);
        }

        /// <summary>
        /// Destroys an object
        /// </summary>
        /// <param name="db">DbContext to be used; when not provided a default instance of TDbCtx will be used</param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected virtual async Task<IActionResult> DestroyAsync(DbContext db, Guid uuid)
        {
            //all stuff is instance based, so need to obtain one first
            T obj = (T)Activator.CreateInstance(typeof(T));

            try
            {
                obj = await obj.DestroyAsync(db, uuid);

                if (obj != null)
                {
                    return StatusCode((int)HttpStatusCode.NoContent);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return this.HandleException(ex);
            }
        }
    }
}
