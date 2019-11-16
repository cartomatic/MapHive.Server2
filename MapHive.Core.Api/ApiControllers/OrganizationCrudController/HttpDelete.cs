using Cartomatic.Utils.Ef;
using MapHive.Core.DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace MapHive.Core.Api.ApiControllers
{
    public abstract partial class OrganizationCrudController<T, TDbContext> : CrudController<T, TDbContext>, IOrganizationApiController<TDbContext>
        where T : Base, new()
        where TDbContext : DbContext, IProvideDbContextFactory, new()
    {
        /// <summary>
        /// Default delete action
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="db">DbContext to be used; when not provided a default instance of TDbCtx will be used</param>
        /// <returns></returns>
        protected override async Task<IActionResult> DeleteAsync(Guid uuid, DbContext db = null)
        {
            //enforced at the filter action attribute level for db ctx obtained from GetOrganizationDbContext() so just testing if passed dbCtx is different
            if (db != null && db != GetOrganizationDbContextSafe())
                if (!await IsCrudPrivilegeGrantedForDestroyAsync(db))
                    return NotAllowed();

            return await DestroyAsync(db ?? GetOrganizationDbContext(), uuid);
        }
    }
}
