using Cartomatic.Utils.Ef;
using MapHive.Core.DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MapHive.Core.Api.ApiControllers
{
    public abstract partial class OrganizationCrudController<T, TDbContext> : CrudController<T, TDbContext>, IOrganizationApiController<TDbContext>
        where T : Base, new()
        where TDbContext : DbContext, IProvideDbContextFactory, new()
    {
        /// <summary>
        /// Default get all action
        /// </summary>
        /// <param name="sort"></param>
        /// <param name="filter"></param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <param name="db">DbContext to be used; when not provided a default instance of TDbCtx will be used</param>
        /// <returns></returns>
        protected override async Task<IActionResult> GetAsync(string sort = null, string filter = null, int start = 0, int limit = 25, DbContext db = null)
        {
            if (!await IsCrudPrivilegeGrantedForReadAsync(db ?? GetOrganizationDbContext()))
                return NotAllowed();

            return await ReadAsync<T, T>(db ?? GetOrganizationDbContext(), sort, filter, start, limit);
        }

        /// <summary>
        /// Default get all action
        /// </summary>
        /// <typeparam name="TDto">Defualt get by id action with automated DTO operation output</typeparam>
        /// <param name="sort"></param>
        /// <param name="filter"></param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <param name="db">DbContext to be used; when not provided a default instance of TDbCtx will be used</param>
        /// <returns></returns>
        protected override async Task<IActionResult> GetAsync<TDto>(string sort = null, string filter = null, int start = 0, int limit = 25, DbContext db = null)
        {
            if (!await IsCrudPrivilegeGrantedForReadAsync(db ?? GetOrganizationDbContext()))
                return NotAllowed();

            return await ReadAsync<T, TDto>(db ?? GetOrganizationDbContext(), sort, filter, start, limit);
        }

        /// <summary>
        /// Default get all action
        /// </summary>
        /// <typeparam name="TExtended">Type to use as a base for reading - used for customising the read src, usually for extended views</typeparam>
        /// <param name="sort"></param>
        /// <param name="filter"></param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <param name="db">DbContext to be used; when not provided a default instance of TDbCtx will be used</param>
        /// <returns></returns>
        protected override async Task<IActionResult> GetExtendedAsync<TExtended>(string sort = null, string filter = null, int start = 0, int limit = 25, DbContext db = null)
        {
            if (!await IsCrudPrivilegeGrantedForReadAsync(db ?? GetOrganizationDbContext()))
                return NotAllowed();

            return await ReadAsync<TExtended, TExtended>(db ?? GetOrganizationDbContext(), sort, filter, start, limit);
        }

        /// <summary>
        /// Defualt get by id action
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="db">DbContext to be used; when not provided a default instance of TDbCtx will be used</param>
        /// <returns></returns>
        protected override async Task<IActionResult> GetAsync(Guid uuid, DbContext db = null)
        {
            if (!await IsCrudPrivilegeGrantedForReadAsync(db ?? GetOrganizationDbContext()))
                return NotAllowed();

            return await ReadAsync<T>(db ?? GetOrganizationDbContext(), uuid);
        }

        /// <summary>
        /// Defualt get by id action with automated DTO operation output
        /// </summary>
        /// <typeparam name="TDto">DTO Type to convert the output into</typeparam>
        /// <param name="uuid"></param>
        /// <param name="db">DbContext to be used; when not provided a default instance of TDbCtx will be used</param>
        /// <returns></returns>
        protected override async Task<IActionResult> GetAsync<TDto>(Guid uuid, DbContext db = null)
        {
            if (!await IsCrudPrivilegeGrantedForReadAsync(db ?? GetOrganizationDbContext()))
                return NotAllowed();

            return await ReadAsync<TDto>(db ?? GetOrganizationDbContext(), uuid);
        }


        /// <summary>
        /// Reads links for given property
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="propertySpecifier"></param>
        /// <returns></returns>
        protected override async Task<IActionResult> ReadLinksAsync(Guid uuid, Expression<Func<T, IEnumerable<Base>>> propertySpecifier)
        {
            return await ReadLinksAsync(GetOrganizationDbContext(), uuid, propertySpecifier);
        }


        /// <summary>
        /// Reads parents of given type
        /// </summary>
        /// <typeparam name="TParent"></typeparam>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected override async Task<IActionResult> ReadParentsAsync<TParent>(Guid uuid)
        {
            return await ReadParentsAsync<TParent>(GetOrganizationDbContext(), uuid);
        }


        /// <summary>
        /// Reads children of given type
        /// </summary>
        /// <typeparam name="TChild"></typeparam>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected override async Task<IActionResult> ReadChildrenAsync<TChild>(Guid uuid)
        {

            return await ReadChildrenAsync<TChild>(GetOrganizationDbContext(), uuid);
        }


        /// <summary>
        /// Reads first child of given type
        /// </summary>
        /// <typeparam name="TChild"></typeparam>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected override async Task<IActionResult> ReadFirstChildAsync<TChild>(Guid uuid)
        {

            return await ReadFirstChildAsync<TChild>(GetOrganizationDbContext(), uuid);
        }
    }
}
