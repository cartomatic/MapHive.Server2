using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DAL;
using MapHive.MembershipReboot;
#if NETFULL
using System.Data.Entity;
#endif
#if NETSTANDARD
using Microsoft.EntityFrameworkCore;
#endif

namespace MapHive.Core.DataModel
{
    public static partial class OrganizationCrudExtensions
    {
        /// <summary>
        /// Adds a user to an org as an admin. 
        /// </summary>
        /// <param name="org"></param>
        /// <param name="dbCtx"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public static async Task AddAdmin(this Organization org, DbContext dbCtx, MapHiveUser u)
        {
            //first find the admin role - it should always be there as it is created on org create
            var adminR = await org.GetOrgAdminRoleAsync(dbCtx);

            //and next link it to a user
            u.AddLink(adminR);
            await u.UpdateAsync(dbCtx,  MembershipRebootUtils.GetUserAccountService(MembershipRebootUtils.GetMembershipRebootDbctx()), u.Uuid);

            //and also add a user to an org - this will not change anything if a user is already linked
            org.AddLink(u);
            await org.UpdateAsync(dbCtx, org.Uuid);

        }

        /// <summary>
        /// Removes owner from an organisation; this is done by simply removing a link to the admin role
        /// </summary>
        /// <param name="org"></param>
        /// <param name="dbCtx"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public static async Task RemoveAdmin(this Organization org, DbContext dbCtx, MapHiveUser u)
        {
            //first grab the admin role for the org
            var adminR = await org.GetOrgAdminRoleAsync(dbCtx);

            //and remove the link
            var mhDb = (MapHiveDbContext) dbCtx;
            mhDb.Links.Remove(
                mhDb.Links.FirstOrDefault(r => r.ParentUuid == u.Uuid && r.ChildUuid == adminR.Uuid));

            await mhDb.SaveChangesAsync();
        }

        /// <summary>
        /// Gets a list of admins of an organisation
        /// </summary>
        /// <param name="org"></param>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<MapHiveUser>> GetAdminsAsync(this Organization org, DbContext dbCtx)
        {
            var mhDb = (MapHiveDbContext)dbCtx;

            var adminR = await org.GetOrgAdminRoleAsync(dbCtx);

            return await adminR.GetParentsAsync<Role, MapHiveUser>(mhDb);
        }

        /// <summary>
        /// Checks if a given user is an admin of an organization
        /// </summary>
        /// <param name="org"></param>
        /// <param name="dbCtx"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<bool> CheckIfUserIsAdmin(this Organization org, DbContext dbCtx, Guid userId)
        {
            var owners = await org.GetAdminsAsync(dbCtx);
            return owners.Any(o => o.Uuid == userId);
        }
    }
}
