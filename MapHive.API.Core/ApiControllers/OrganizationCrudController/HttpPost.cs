using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Cartomatic.Utils.Ef;
using MapHive.Core.DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MapHive.API.Core.ApiControllers
{
    public abstract partial class OrganizationCrudController<T, TDbContext> : CrudController<T, TDbContext>, IOrganizationApiController<TDbContext>
        where T : Base, new()
        where TDbContext : DbContext, IProvideDbContextFactory, new()
    {
        /// <summary>
        /// Defualt post action
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="db">DbContext to be used; when not provided a default instance of TDbCtx will be used</param>
        /// <returns></returns>
        public override async Task<IActionResult> PostAsync(T obj, DbContext db = null)
        {
            return await CreateAsync(GetOrganizationDbContext(), obj);
        }

        /// <summary>
        /// Defualt post action with automated conversion from DTO
        /// </summary>
        /// <typeparam name="DTO">DTO type to convert from to the core type</typeparam>
        /// <param name="obj"></param>
        /// <param name="db">DbContext to be used; when not provided a default instance of TDbCtx will be used</param>
        /// <returns></returns>
        public override async Task<IActionResult> PostAsync<DTO>(DTO obj, DbContext db = null)
        {
            return await CreateAsync(GetOrganizationDbContext(), obj);
        }
    }
}
