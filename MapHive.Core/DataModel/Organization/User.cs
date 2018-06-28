using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Relational;

using Microsoft.EntityFrameworkCore;


namespace MapHive.Core.DataModel
{
    
    public partial class Organization
    {
        /// <summary>
        /// Gets a user that is associated with given organization (org is the user's profile counterpart
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        public async Task<MapHiveUser> GetOrganizationUserAsync(DbContext dbCtx)
        {
            return await dbCtx.Set<MapHiveUser>().FirstOrDefaultAsync(u => u.UserOrgId == Uuid);
        }

        /// <summary>
        /// Returns a list of organization user identifiers
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Guid>> GetOrganizationUserIdsAsync(DbContext dbCtx)
        {
            return (await this.GetChildLinksAsync<Organization, MapHiveUser>(dbCtx)).Select(l=>l.ChildUuid);
        }


        /// <summary>
        /// User application access credentials within an organization
        /// </summary>
        public class OrgUserAppAccessCredentials
        {
            /// <summary>
            /// Organization that the app access credentials are tested for
            /// </summary>
            public Organization Organization { get; set; }


            /// <summary>
            /// Application for which the access credentials are tested within an organization
            /// </summary>
            public Application Application { get; set; }

            /// <summary>
            /// Whether or not user can use given application within an organization
            /// </summary>
            public bool CanUseApp { get; set; }

            /// <summary>
            /// Whether or not user has application administration rights within an organization
            /// </summary>
            public bool IsAppAdmin { get; set; }
        }


        /// <summary>
        /// Returns user application credentials within an organization
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <param name="user"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        public async Task<OrgUserAppAccessCredentials> GetUserAppAccessCredentials(DbContext dbCtx, MapHiveUser user,
            Application app)
        {
            var output = new OrgUserAppAccessCredentials
            {
                Organization = this,
                Application = app
            };

            //check if organization can access and application if not, then good bye
            if (!await CanUseApp(dbCtx, app))
                return output;



            //check if user is an org owner or org admin
            output.IsAppAdmin = await IsOrgOwner(dbCtx, user) || await IsOrgAdmin(dbCtx, user);
            output.CanUseApp = output.IsAppAdmin;


            //user is not granted app admin access via org owner / admin roles, so need to check if user can access the app as a 'normal' user, so via teams
            //note: teams can also grant app admin role!
            if (!output.IsAppAdmin)
            {
                var orgTeams = await this.GetChildrenAsync<Organization, Team>(dbCtx);
                foreach (var team in orgTeams)
                {
                    //get team's app link
                    var teamAppLink = await team.GetChildLinkAsync(dbCtx, app);

                    //make sure team grants access to an app
                    if (teamAppLink == null)
                        continue;


                    //get team's user link
                    var teamUserLink = await team.GetChildLinkAsync(dbCtx, user);
                    var userCanUseApp = teamUserLink != null;

                    //if a team grants an access to an app for a user we can test if it also test if it is admin access
                    if (userCanUseApp)
                    {
                        //only apply true assignment here so it does not get reset when searching for app admin credentials!
                        output.CanUseApp = userCanUseApp;

                        //extract app access credentials link data
                        var appAccessCredentials = teamAppLink.LinkData.GetByKey(Team.AppAccessCredentialsLinkDataObject);


                        output.IsAppAdmin = appAccessCredentials != null &&
                                            appAccessCredentials.ContainsKey(Team.AppAdminAccess) &&
                                            (bool) appAccessCredentials[Team.AppAdminAccess];

                    }

                    //no point in testing other teams, as full app access is already granted
                    if (output.IsAppAdmin)
                        break;

                }

            }

            //if this is a default (dashboard) app a user should ALWAYS BE ABLE TO use it; the default (dashboard) app will take care of handling the context itself
            if (app.IsDefault)
                output.CanUseApp = true;

            return output;
        }

        /// <summary>
        /// Adds a member to an organization
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <param name="user"></param>
        /// <param name="userAccountService"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task AddOrganizationUser<TAccount>(DbContext dbCtx, MapHiveUser user, UserAccountService<TAccount> userAccountService, OrganizationRole role = OrganizationRole.Member)
            where TAccount : RelationalUserAccount
        {
            this.AddLink(user);
            await this.UpdateAsync(dbCtx);

            //by default assign a member role to a user
            var memberRole = await this.GetOrgRoleAsync(dbCtx, role);

            user.AddLink(memberRole);
            await user.UpdateAsync(dbCtx, userAccountService);
        }

        /// <summary>
        /// Changes a user role within the organization
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <param name="user"></param>
        /// <param name="userAccountService"></param>
        /// <returns></returns>
        public async Task ChangeOrganizationUserRole<TAccount>(DbContext dbCtx, MapHiveUser user, UserAccountService<TAccount> userAccountService)
            where TAccount : RelationalUserAccount
        {
            //this basically needs to remove all the org roles for a user and add the specified one
            var ownerRole = await GetOrgOwnerRoleAsync(dbCtx);
            var adminRole = await GetOrgAdminRoleAsync(dbCtx);
            var memberRole = await GetOrgMemberRoleAsync(dbCtx);

            var addRole = memberRole;
            switch (user.OrganizationRole)
            {
                case OrganizationRole.Admin:
                    addRole = adminRole;
                    break;
                case OrganizationRole.Owner:
                    addRole = ownerRole;
                    break;
            }

            user.RemoveLink(ownerRole);
            user.RemoveLink(adminRole);
            user.RemoveLink(memberRole);

            if (user.OrganizationRole.HasValue)
            {
                user.AddLink(addRole);
            }

            await user.UpdateAsync(dbCtx, userAccountService);
        }
    }
}
