using System;
using System.Linq;
using System.Net.Http;
using MapHive.Core.Api.UserConfiguration;
using MapHive.Core.DataModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MapHive.Core.Api
{

    /// <summary>
    /// Action is run before every api request and gets databases data for organization
    /// It is assumed to work along with the UserConfigAttribute that is defined globally and therefore its data is present already when
    /// this filter kicks in
    /// </summary>
    public class OrganizationContextActionFilterAttribute : ActionFilterAttribute
    {
        public const string OrgIdPropertyName = "organizationuuid";


        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            if (actionContext.ActionArguments.ContainsKey(OrgIdPropertyName))
            {
                var guid = (Guid)actionContext.ActionArguments[OrgIdPropertyName];

                //save on the request object so can be retrieved later
                actionContext.HttpContext.Items[OrgIdPropertyName] = guid;

                //Note:
                //dbs are totally distributed, so it is more than likely that at the api level we have no access to the 'core' metadata
                //this means that in order to obtain org specific db credentials it is necessary to inspect output from UserConfogurationAttribute - it is applied to all the controllers by default so user / token cfg is always retrieved; this configuration not contains orgs, and their dbs.
           }

            base.OnActionExecuting(actionContext);
        }


        /// <summary>
        /// gets an org id the action should be scoped to - as retrieved by the filter
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Guid GetOrganizationId(HttpContext context)
        {
            context.Items.TryGetValue(OrgIdPropertyName, out var orgIdStr);

            if (Guid.TryParse(orgIdStr?.ToString() ?? string.Empty, out var orgId))
            {
                return orgId;
            }
            throw new ArgumentException("No organization id found.");
        }

        /// <summary>
        /// Gets org context for the controller
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Organization GetOrganizationContext(HttpContext context)
        {
            //Note: as described in the OnActionExecuting, org related stuff is now always retrieved by the UserConfiguration attribute.
            //so provided an action is not an anonymous action, all the needed stuff should really be here!

            var orgId = GetOrganizationId(context);

            //Note: dependancy on the UserConfigurationActionFilterAtribute may not be that sensible. but i think should not hurt really...

            var org = UserConfigurationActionFilterAtribute.GetOrganization(context, orgId);

            if (org == null)
                throw new ArgumentException("No organization found.");

            return org;
        }

        /// <summary>
        /// Extracts Organisation Db from request
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dbIdentifier">Identifier of a db to extract</param>
        /// <returns></returns>
        public static OrganizationDatabase GetOrganisationDatabase(HttpContext context, string dbIdentifier = "")
        {
            //Note: as described in the OnActionExecuting, org related stuff is now always retrieved by the UserConfiguration attribute.
            //so provided an action is not an anonymous action, all the needed stuff should really be here!

            var orgId = GetOrganizationId(context);

            //Note: dependancy on the UserConfigurationActionFilterAtribute may not be that sensible. but i think should not hurt really...

            //grab the specified org db
            //Note: because user config is returned with org databases there is no need to load or dbs explicitly
            var orgDb = UserConfigurationActionFilterAtribute.GetUserConfiguration(context)?.GetOrganizationdatabase(orgId, dbIdentifier);
                
            if (orgDb == null)
                throw new ArgumentException($"No database found for given organizationId: '{orgId}' and dbIdentifier: '{dbIdentifier}'.");

            return orgDb;
        }

        /// <summary>
        /// Resets organization context for given org id; this will cause a fresh reload of organization context for the subsequent api ops
        /// </summary>
        /// <param name="orgId"></param>
        public static void ResetOrganizationContext(Guid orgId)
        {
            UserConfigurationActionFilterAtribute.ResetUserConfigCacheByOrgId(orgId);
        }
    }

}