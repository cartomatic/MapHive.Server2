using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DAL;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel
{
    public static partial class OrganizationCrudExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="org"></param>
        /// <param name="dbCtx"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public static async Task AddOwnerAsync(this Organization org, DbContext dbCtx, MapHiveUser u)
        {
            //first find the owner role - it should always be there as it is created on org create
            var ownerR = await org.GetOrgOwnerRoleAsync(dbCtx);

            //and next link it to a user
            u.AddLink(ownerR);
            await u.UpdateWithNoIdentityChangesAsync<MapHiveUser>(dbCtx);

            //and also add a user to an org
            org.AddLink(u);
            await org.UpdateAsync(dbCtx);
        }

        /// <summary>
        /// Removes owner from an organisation; this is done by simpoy removing a link to the owner role
        /// </summary>
        /// <param name="org"></param>
        /// <param name="dbCtx"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public static async Task RemoveOwnerAsync(this Organization org, DbContext dbCtx, MapHiveUser u)
        {
            //first grab the owner role for the org
            var ownerR = await org.GetOrgOwnerRoleAsync(dbCtx);

            //and remove the link
            var mhDb = (MapHiveDbContext) dbCtx;
            mhDb.Links.Remove(
                mhDb.Links.FirstOrDefault(r => r.ParentUuid == u.Uuid && r.ChildUuid == ownerR.Uuid));

            await mhDb.SaveChangesAsync();
        }

        /// <summary>
        /// Gets a list of owners of an organisation
        /// </summary>
        /// <param name="org"></param>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<MapHiveUser>> GetOwnersAsync(this Organization org, DbContext dbCtx)
        {
            var mhDb = (MapHiveDbContext)dbCtx;

            var ownerR = await org.GetOrgOwnerRoleAsync(dbCtx);

            return await ownerR.GetParentsAsync<Role, MapHiveUser>(mhDb);
        }

        /// <summary>
        /// Checks if a given user is an owner of an organization
        /// </summary>
        /// <param name="org"></param>
        /// <param name="dbCtx"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<bool> CheckIfUserIsOwnerAsync(this Organization org, DbContext dbCtx, Guid userId)
        {
            var owners = await org.GetOwnersAsync(dbCtx);
            return owners.Any(o => o.Uuid == userId);
        }
    }
}
