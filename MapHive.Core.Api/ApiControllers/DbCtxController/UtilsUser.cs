using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using Microsoft.EntityFrameworkCore;
using RestSharp;

namespace MapHive.Core.Api.ApiControllers
{
    public abstract partial class DbCtxController<TDbContext>
    {
        /// <summary>
        /// checks if a user with given credentials is a super admin and can modify the env (platform); if so impersonates a user to interact with the api
        /// </summary>
        /// <param name="remoteCheck">whether or not should check via core api</param>
        /// <returns></returns>
        protected async Task<bool> UserIsEnvAdminAsync(bool remoteCheck = false)
        {
            if (remoteCheck)
                return await UserIsEnvAdminRemoteCheckAsync();

            //grab the user's org and see if an org has an access to the env admin app
            //at this particular stage it should be enough to make sure user is allowed to perform 'superadmin' like ops

            var uuid = Cartomatic.Utils.Identity.GetUserGuid();

            var db = _db as MapHiveDbContext;
            if (db == null)
                return false;

            //read user
            var user = await db.Users.FirstOrDefaultAsync(u => u.Uuid == uuid);

            if (user != null)
            {
                //get orgs a user is assigned to
                var orgs = await user.GetParentsAsync<MapHiveUser, Organization>(_db);

                foreach (var org in orgs)
                {
                    await org.MaterializeLinksAsDetachedAsync(_db, o => o.Applications);
                    if (org.Applications.Any(app => app.ShortName == "masterofpuppets"))
                    {
                        Cartomatic.Utils.Identity.ImpersonateUser(uuid.Value);
                        return true;
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// Checks whether or not a user is an env (platform) admin user
        /// </summary>
        /// <returns></returns>
        protected async Task<bool> UserIsEnvAdminRemoteCheckAsync()
        {
            var cfg = Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig();

            //call a remote api...
            return (await RestApiCall<bool>(
                cfg["Endpoints:Core"] + "envconfig/",
                "envadmin",
                Method.GET,

                //add auth token
                authToken: $"{Request.Headers.Authorization.Scheme} {Request.Headers.Authorization.Parameter}"
            )).Output;
        }

        /// <summary>
        /// Checks if a current (obtained via ClaimsPrinicipal) user is an organization owner
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        protected async Task<bool> UserIsOrgOwner(Guid orgId)
        {
            var userId = Cartomatic.Utils.Identity.GetUserGuid();
            return await UserIsOrgOwner(orgId, userId);
        }

        /// <summary>
        /// Wherther or not a user is an org admin
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        protected async Task<bool> UserIsOrgOwner(Guid orgId, Guid? userId)
        {   
            var dbCtx = _db as MapHiveDbContext;
            if (dbCtx == null)
                return false;

            var org = await dbCtx.Organizations.FirstOrDefaultAsync(o => o.Uuid == orgId);

            return userId == null || org == null || await org.IsOrgOwnerAsync(dbCtx, userId.Value);
        }


        /// <summary>
        /// Checks if a current (obtained via ClaimsPrinicipal) user is an organization admin
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        protected async Task<bool> UserIsOrgAdmin(Guid orgId)
        {
            var userId = Cartomatic.Utils.Identity.GetUserGuid();

            var dbCtx = _db as MapHiveDbContext;
            if (dbCtx == null)
                return false;

            var org = await dbCtx.Organizations.FirstOrDefaultAsync(o => o.Uuid == orgId);


            return userId == null || org == null || await org.IsOrgAdminAsync(dbCtx, userId.Value);
        }

        /// <summary>
        /// Checks if a current (obtained via ClaimsPrinicipal) user is an organization owner or admin
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        protected async Task<bool> UserIsOrgOwnerOrAdmin(Guid orgId)
        {
            var userId = Cartomatic.Utils.Identity.GetUserGuid();

            var dbCtx = _db as MapHiveDbContext;
            if (dbCtx == null)
                return false;

            var org = await dbCtx.Organizations.FirstOrDefaultAsync(o => o.Uuid == orgId);

            return userId == null || org == null || await org.IsOrgOwnerAsync(dbCtx, userId.Value) || await org.IsOrgAdminAsync(dbCtx, userId.Value);
        }

    }
}
