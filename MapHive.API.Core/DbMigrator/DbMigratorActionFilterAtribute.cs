using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MapHive.Api.Core.ApiControllers;
using MapHive.Api.Core.UserConfiguration;
using MapHive.Api.Core;
using MapHive.Api.Core.Extensions;
using MapHive.Core.DataModel;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using RestSharp.Extensions;

namespace MapHive.Api.Core
{
    /// <summary>
    /// Attribute responsible for triggering auto db migrations at the API level as opposed to the usual web client config retrieval db migration init scenario; 
    /// </summary>
    /// <remarks>
    /// This attribute should be executed after the user configuration filter attribute has kicked in. This is because the user configuration attribute is responsible
    /// for obtaining the org context and therofore the org db context too!
    /// Basically the best place to configure this class is the API startup as it will take care of putting the attribute in the right place
    /// </remarks>
    public class DbMigratorActionFilterAtribute : ActionFilterAttribute
    {
        /// <summary>
        /// Db migrator that takes in org ctx
        /// </summary>
        private readonly Func<Organization, Task> _orgDbMigrator;

        /// <summary>
        /// a db migrator that works with a specified ctx
        /// </summary>
        private readonly Func<DbContext, Task> _dbMigrator;

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="orgDbMigrator"></param>
        /// <param name="dbMigrator"></param>
        public DbMigratorActionFilterAtribute(
            Func<Organization, Task> orgDbMigrator,
            Func<DbContext, Task> dbMigrator)
        {
            _orgDbMigrator = orgDbMigrator;
            _dbMigrator = dbMigrator;
        }

        /// <summary>
        /// org id property name that needs to be in the action arguments in order to 'guess' this is an org ctx
        /// </summary>
        protected string OrgIdPropertyName => OrganizationContextActionFilterAttribute.OrgIdPropertyName;


        /// <summary>
        /// Cache for the org db migrations, so can see when a migration took place and avoid migrating more than once in the app pool lifetime.
        /// re-publishing api will cause this cache to reset
        /// </summary>
        private readonly Dictionary<Guid, bool> _autoOrgMigrationsCache = new Dictionary<Guid, bool>();

        /// <summary>
        /// Whether or not the non org ctx has already been migrated; this should happen only once per app pool life time.
        /// re-publishing api will cause this cache to reset
        /// </summary>
        private bool _nonOrgCtxMigrated = false;

        /// <inheritdoc />
        public override async Task OnActionExecutionAsync(ActionExecutingContext actionContext, ActionExecutionDelegate next)
        {
            //need to trigger db migrations as configured in order to enable the api to automigrate

            if (_orgDbMigrator != null && actionContext.ActionArguments.ContainsKey(OrgIdPropertyName))
            {
                var orgId = (Guid)actionContext.ActionArguments[OrgIdPropertyName];

                //avoid migrating more than once per app pool life time; when api gets re-published this should reset, so migrations shoudl kick in automatically
                if (!_autoOrgMigrationsCache.ContainsKey(orgId) || !_autoOrgMigrationsCache[orgId])
                {
                    //grab the org
                    var org = UserConfigurationActionFilterAtribute.GetOrganization(actionContext.HttpContext, orgId);

                    if (org != null)
                    {
                        EnsureUserImpersonation(actionContext);

                        await _orgDbMigrator(org);
                        _autoOrgMigrationsCache[orgId] = true;
                    }
                }
            }


            //standard migrator with no org context - such apis can exist too ;)
            if (_dbMigrator != null && !_nonOrgCtxMigrated)
            {
                //try to grab the context...
                var ctrl = (IDbCtxController)actionContext.Controller;
                if (ctrl != null)
                {
                    EnsureUserImpersonation(actionContext);

                    await _dbMigrator(ctrl.GetDefaultDbContext());
                    _nonOrgCtxMigrated = true;
                }
            }


            await next();
        }

 
        /// <summary>
        /// Ensures user is impersonated
        /// </summary>
        /// <param name="actionContext"></param>
        protected void EnsureUserImpersonation(ActionExecutingContext actionContext)
        {
            //maphive context requires user to be known in order to save changes - this is required for auto populating
            //fields such as last modified by, or modify date.
            //apparently DbContext uses same methods for saving when running migrations and will fail without the known user
            //in order to avoid such problems need:
            //check if user identidier is known and if so use it 
            //if not, grab token and use it
            //if no user / token present then can use ghost

            var userId = Cartomatic.Utils.Identity.GetUserGuid();
            if (!userId.HasValue)
            {
                //check token and use it
                var authHdr = actionContext.HttpContext.ExtractAuthHeader();
                if (authHdr != null && !string.IsNullOrEmpty(authHdr?.parameter) && Guid.TryParse(authHdr?.parameter, out var token))
                {
                    Cartomatic.Utils.Identity.ImpersonateUser(token);
                }
                else
                {
                    //if no token, then ghost
                    Cartomatic.Utils.Identity.ImpersonateGhostUser();
                }
            }
        }
    }
}
