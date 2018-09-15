using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Defualt put action
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="uuid"></param>
        /// <param name="db">DbContext to be used; when not provided a default instance of TDbCtx will be used</param>
        /// <returns></returns>
        public virtual async Task<IActionResult> PutAsync(T obj, Guid uuid, DbContext db = null)
        {
            if (!await IsCrudPrivilegeGrantedForUpdateAsync(db))
                return NotAllowed();

            return await UpdateAsync(db ?? _dbCtx, obj, uuid);
        }

        /// <summary>
        /// Default put action 
        /// </summary>
        /// <typeparam name="TDto">DTO type to convert from to the core type</typeparam>
        /// <param name="obj"></param>
        /// <param name="uuid"></param>
        /// <param name="db">DbContext to be used; when not provided a default instance of TDbCtx will be used</param>
        /// <returns></returns>
        public virtual async Task<IActionResult> PutAsync<TDto>(TDto obj, Guid uuid, DbContext db = null) where TDto : class
        {
            if (!await IsCrudPrivilegeGrantedForUpdateAsync(db))
                return NotAllowed();

            return await UpdateAsync(db ?? _dbCtx, obj, uuid);
        }

        /// <summary>
        /// Updates an object
        /// </summary>
        /// <param name="db">DbContext to be used; when not provided a default instance of TDbCtx will be used</param>
        /// <param name="obj"></param>
        /// <param name="uuid"></param>
        /// <typeparam name="TDto">Type to transfer the data from when creating an instance of T; must implement IDto of TDto</typeparam>
        /// <returns></returns>
        protected virtual async Task<IActionResult> UpdateAsync<TDto>(DbContext db, TDto obj, Guid uuid) where TDto : class
        {
            try
            {
                T coreObj;

                //DM Note: this could and should be done in a more elegant way. but had no smart ideas at a time. will come back to this at some stage...
                if (typeof(T) != typeof(TDto))
                {
                    //Note: IDto is on the TDto and is implemented on instance obviously. so need one
                    var inst = Cartomatic.Utils.Dto.IDtoUtils.CrateIDtoInstance<TDto>();

                    coreObj = inst.FromDto<T>(obj);
                }
                else
                {
                    coreObj = obj as T;
                }

                var entity = await coreObj.UpdateAsync(db, uuid);

                if (entity != null)
                    return Ok(entity);

                return NotFound();

            }
            catch (Exception ex)
            {
                return this.HandleException(ex);
            }
        }
    }
}
