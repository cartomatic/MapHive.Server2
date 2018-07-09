using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Ef;
using MapHive.Core.DataModel;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace MapHive.API.Core.ApiControllers
{
    public abstract partial class OrganizationCrudController<T, TDbContext> : CrudController<T, TDbContext>, IOrganizationApiController<TDbContext>
        where T : Base, new()
        where TDbContext : DbContext, IProvideDbContextFactory, new()
    {
        /// <summary>
        /// Whether or not user is an org admin
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        protected virtual bool UserIsOrgAdmin(Guid? userId)
        {
            return OrganizationContext.Admins.Any(u => u.Uuid == userId);
        }

        /// <summary>
        /// Whether or not user is an org owner
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        protected virtual bool UserIsOrgOwner(Guid? userId)
        {
            return OrganizationContext.Owners.Any(u => u.Uuid == userId);
        }

        /// <summary>
        /// Whether or not request can perform a read action
        /// </summary>
        /// <returns></returns>
        protected override async Task<bool> IsCrudPrivilegeGrantedForReadAsync(DbContext dbCtx)
        {
            var userId = Cartomatic.Utils.Identity.GetUserGuid();
            return UserIsOrgOwner(userId) || UserIsOrgAdmin(userId) || await base.IsCrudPrivilegeGrantedForReadAsync(dbCtx);
        }

        /// <summary>
        /// Whether or not request can perform a create action
        /// </summary>
        /// <returns></returns>
        protected override async Task<bool> IsCrudPrivilegeGrantedForCreateAsync(DbContext dbCtx)
        {
            var userId = Cartomatic.Utils.Identity.GetUserGuid();
            return UserIsOrgOwner(userId) || UserIsOrgAdmin(userId) || await base.IsCrudPrivilegeGrantedForCreateAsync(dbCtx);
        }

        /// <summary>
        /// Whether or not request can perform a update action
        /// </summary>
        /// <returns></returns>
        protected override async Task<bool> IsCrudPrivilegeGrantedForUpdateAsync(DbContext dbCtx)
        {
            var userId = Cartomatic.Utils.Identity.GetUserGuid();
            return UserIsOrgOwner(userId) || UserIsOrgAdmin(userId) || await base.IsCrudPrivilegeGrantedForUpdateAsync(dbCtx);
        }

        /// <summary>
        /// Whether or not request can perform a destroy action
        /// </summary>
        /// <returns></returns>
        protected override async Task<bool> IsCrudPrivilegeGrantedForDestroyAsync(DbContext dbCtx)
        {
            var userId = Cartomatic.Utils.Identity.GetUserGuid();
            return UserIsOrgOwner(userId) || UserIsOrgAdmin(userId) || await base.IsCrudPrivilegeGrantedForDestroyAsync(dbCtx);
        }
    }
}
