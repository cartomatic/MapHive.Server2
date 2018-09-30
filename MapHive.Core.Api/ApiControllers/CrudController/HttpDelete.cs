using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using MapHive.Core.DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.Api.ApiControllers
{
    public abstract partial class CrudController<T, TDbCtx>
    {
        /// <summary>
        /// Defualt delete action
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="db">DbContext to be used; when not provided a default instance of TDbCtx will be used</param>
        /// <returns></returns>
        protected virtual async Task<IActionResult> DeleteAsync(Guid uuid, DbContext db = null)
        {
            if (!await IsCrudPrivilegeGrantedForDestroyAsync(db ?? _dbCtx))
                return NotAllowed();

            return await DestroyAsync(db ?? _dbCtx, uuid);
        }

        /// <summary>
        /// Destorys an object
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
                    return StatusCode(HttpStatusCode.NoContent);
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
