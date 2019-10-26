using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Ef;
using MapHive.Core.DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.Api.ApiControllers
{
    public abstract partial class OrganizationCrudController<T, TDbContext> : CrudController<T, TDbContext>, IOrganizationApiController<TDbContext>
        where T : Base, new()
        where TDbContext : DbContext, IProvideDbContextFactory, new()
    {
        /// <summary>
        /// Default put action
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="uuid"></param>
        /// <param name="db">DbContext to be used; when not provided a default instance of TDbCtx will be used</param>
        /// <returns></returns>
        protected override async Task<IActionResult> PutAsync(T obj, Guid uuid, DbContext db = null)
        {
            //enforced at the filter action attribute level for db ctx obtained from GetOrganizationDbContext() so just testing if passed dbCtx is different
            if(db != null && db != GetOrganizationDbContext())
                if (!await IsCrudPrivilegeGrantedForUpdateAsync(db))
                    return NotAllowed();

            return await UpdateAsync(db ?? GetOrganizationDbContext(), obj, uuid);
        }

        /// <summary>
        /// Default put action 
        /// </summary>
        /// <typeparam name="TDto">DTO type to convert from to the core type</typeparam>
        /// <param name="obj"></param>
        /// <param name="uuid"></param>
        /// <param name="db">DbContext to be used; when not provided a default instance of TDbCtx will be used</param>
        /// <returns></returns>
        protected override async Task<IActionResult> PutAsync<TDto>(TDto obj, Guid uuid, DbContext db = null)
        {
            //enforced at the filter action attribute level for db ctx obtained from GetOrganizationDbContext() so just testing if passed dbCtx is different
            if (db != null && db != GetOrganizationDbContext())
                if (!await IsCrudPrivilegeGrantedForUpdateAsync(db))
                    return NotAllowed();

            return await UpdateAsync(db ?? GetOrganizationDbContext(), obj, uuid);
        }
    }
}
