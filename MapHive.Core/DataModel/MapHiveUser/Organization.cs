using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel
{
    public partial class MapHiveUser
    {
        /// <summary>
        /// creates an organization for maphhive user and sets links as expected. org name, slug and such is based on user email!
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        public async Task<Organization> CreateUserOrganizationAsync(DbContext dbCtx)
        {
            if (string.IsNullOrEmpty(Slug))
            {
                throw new Exception("Cannot create a user organization without a valid slug.");
            }

            //org creation step 1
            var org = new Organization
            {
                DisplayName = $"{Email}'s organization",
                Slug = Utils.Slug.GetOrgSlug(null, Email) //no org name here
            };
            //tie up user to an org
            org.AddLink(this);

            await org.CreateAsync(dbCtx);


            //tie up the org to a user
            UserOrgId = org.Uuid;
            //add org owner role to user
            this.AddLink(await org.GetOrgOwnerRoleAsync(dbCtx));

            await this.UpdateAsync(dbCtx);

            return org;
        }

        /// <summary>
        /// gets user's organization - the org that is a counter part of user profile
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        public async Task<Organization> GetUserOrganizationAsync(DbContext dbCtx)
        {
            if (!UserOrgId.HasValue)
            {
                return null;
            }
            return await dbCtx.Set<Organization>().FirstOrDefaultAsync(o => o.Uuid == UserOrgId.Value);
        }

        /// <summary>
        /// Gets organizations a user has an access to. If user has an own org, then it is returned at the begining
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Organization>> GetUserOrganizationsAsync(DbContext dbCtx, Guid userId)
        {
            var user = await dbCtx.Set<MapHiveUser>().FirstOrDefaultAsync(u => u.Uuid == userId);

            if (user == null)
            {
                throw new InvalidOperationException("Unknown user");
            }

            return await user.GetUserOrganizationsAsync(dbCtx);
        }

        /// <summary>
        /// Gets organizations a user has an access to. If user has an own org, then it is returned at the begining
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Organization>> GetUserOrganizationsAsync(DbContext dbCtx)
        {
            //users are assigned to orgs
            //make sure the 'user' org is at the very begining
            return (await this.GetParentsAsync<MapHiveUser, Organization>(dbCtx)).OrderByDescending(o => o.Uuid == UserOrgId);

            //todo: will need to also properly order orgs a user has an owner role, so they seem a bit more important than the other orgs
        }



        public static async Task<IEnumerable<Organization>> GetUserOrganizationsAsync(DbContext dbCtx, Guid uuid,
            string appShortName)
        {
            return await GetUserOrganizationsAsync(dbCtx, uuid, new[] { appShortName });
        }

        //FIXME - make it possible to pass all the org assets as a param, so can avoid re-reading db!

        /// <summary>
        /// Gets a list of user's organisations that have access to specified apps
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <param name="uuid"></param>
        /// <param name="appShortNames">app short names - when provided, only orgs that contain a listed app are returned</param>
        /// <returns></returns>
        public static async Task<IEnumerable<Organization>> GetUserOrganizationsAsync(DbContext dbCtx, Guid uuid,
            IEnumerable<string> appShortNames)
        {
            var user = new MapHiveUser();
            user = await user.ReadAsync(dbCtx, uuid);

            if (user == null)
                return null;

            var organizations = await user.GetParentsAsync<MapHiveUser, Organization>(dbCtx, detached: true);

            if (organizations == null || !organizations.Any())
                return new List<Organization>();

            var filteredOrgs = new List<Organization>();

            foreach (var organization in organizations)
            {
                //get the apps linked directly
                organization.Applications = (await organization.GetOrganizationAssetsAsync<Application>((MapHiveDbContext)dbCtx))?.assets.ToList();

                if (appShortNames != null && appShortNames.Any())
                {
                    if (organization.Applications == null ||
                        !organization.Applications.Any(x => appShortNames.Contains(x.ShortName)))
                        continue;

                    filteredOrgs.Add(organization);
                }
                else
                {
                    filteredOrgs.Add(organization);
                }
            }

            return filteredOrgs;
        }
    }
}
