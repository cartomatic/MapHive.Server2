using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DataModel;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace MapHive.API.Core.ApiControllers
{
    public abstract partial class CrudController<T, TDbCtx>
    {
        /// <summary>
        /// Whether ot not a Read CRUD privilege is required in order to perform an action
        /// </summary>
        protected bool IsCrudPrivilegeRequiredForRead => CheckAttributePresence<CrudPrivilegeRequiredRead>();

        /// <summary>
        /// Whether ot not a Create CRUD privilege is required in order to perform an action
        /// </summary>
        protected bool IsCrudPrivilegeRequiredForCreate => CheckAttributePresence<CrudPrivilegeRequiredCreate>();

        /// <summary>
        /// Whether ot not a Update CRUD privilege is required in order to perform an action
        /// </summary>
        protected bool IsCrudPrivilegeRequiredForUpdate => CheckAttributePresence<CrudPrivilegeRequiredUpdate>();

        /// <summary>
        /// Whether ot not a Destroy CRUD privilege is required in order to perform an action
        /// </summary>
        protected bool IsCrudPrivilegeRequiredForDestroy => CheckAttributePresence<CrudPrivilegeRequiredDestroy>();

        /// <summary>
        /// Checs if either a controller or an action is decorated with given attribute
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        private bool CheckAttributePresence<TAttribute>()
        where TAttribute : ActionFilterAttribute
        {
            //check controller level
            return this.GetType().GetCustomAttributes(typeof(TAttribute)).Any()
                   //AND action level
                   || (this.ControllerContext.ActionDescriptor).MethodInfo.GetCustomAttributes(typeof(TAttribute)).Any();
        }

        /// <summary>
        /// Whether or not request can perform a read action
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<bool> IsCrudPrivilegeGrantedForReadAsync(DbContext dbCtx)
        {
            // Check if permission is required
            if (!IsCrudPrivilegeRequiredForRead)
                return true;

            var roles = await GetUserRoles(dbCtx, Cartomatic.Utils.Identity.GetUserGuid());

            // Check if user roles have required permission
            return roles.Any(r=> r.Privileges.Any(p => p.TypeId == ObjectTypeExtensions.GetTypeUuid(typeof(T)) && p.Read == true));
        }

        /// <summary>
        /// Whether or not request can perform a create action
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<bool> IsCrudPrivilegeGrantedForCreateAsync(DbContext dbCtx)
        {
            // Check if permission is required
            if (!IsCrudPrivilegeRequiredForCreate)
                return true;

            var roles = await GetUserRoles(dbCtx, Cartomatic.Utils.Identity.GetUserGuid());

            // Check if user roles have required permission
            return roles.Any(r => r.Privileges.Any(p => p.TypeId == ObjectTypeExtensions.GetTypeUuid(typeof(T)) && p.Create == true));
        }

        /// <summary>
        /// Whether or not request can perform a update action
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<bool> IsCrudPrivilegeGrantedForUpdateAsync(DbContext dbCtx)
        {
            // Check if permission is required
            if (!IsCrudPrivilegeRequiredForUpdate)
                return true;

            var roles = await GetUserRoles(dbCtx, Cartomatic.Utils.Identity.GetUserGuid());

            // Check if user roles have required permission
            return roles.Any(r => r.Privileges.Any(p => p.TypeId == ObjectTypeExtensions.GetTypeUuid(typeof(T)) && p.Update == true));
        }

        /// <summary>
        /// Whether or not request can perform a destroy action
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<bool> IsCrudPrivilegeGrantedForDestroyAsync(DbContext dbCtx)
        {
            // Check if permission is required
            if (!IsCrudPrivilegeRequiredForDestroy)
                return true;

            var roles = await GetUserRoles(dbCtx, Cartomatic.Utils.Identity.GetUserGuid());

            // Check if user roles have required permission
            return roles.Any(r => r.Privileges.Any(p => p.TypeId == ObjectTypeExtensions.GetTypeUuid(typeof(T)) && p.Destroy == true));
        }

        /// <summary> == 
        /// Gets roles linked to a usr with the specified id
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        protected async Task<IEnumerable<Role>> GetUserRoles(DbContext dbCtx, Guid? userId)
        {
            if(!userId.HasValue)
                return new Role[0];

            return await new MapHiveUser
            {
                Uuid = userId.Value
            }.GetChildrenAsync<MapHiveUser, Role>(dbCtx);
        }

        /// <summary>
        /// returns 403 result
        /// </summary>
        /// <returns></returns>
        protected IActionResult NotAllowed()
        {
            return new ForbidResult();
        }
    }
}
