using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DAL;
using MapHive.Core.DataModel;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.Configuration
{
    public class OrganizationConfiguration
    {
        /// <summary>
        /// Reads config details (org + all the assets) for a specified organization id.
        /// </summary>
        /// <typeparam name="TDbCtx"></typeparam>
        /// <param name="dbctx"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public static async Task<Organization> GetAsync<TDbCtx>(
            TDbCtx dbctx, Guid orgId
        )
            where TDbCtx : MapHiveDbContext, new()
        {
            var dbCtx = new TDbCtx();

            var org = await dbCtx.Organizations.FirstOrDefaultAsync(x => x.Uuid == orgId);


            org.Owners = (await org.GetOwnersAsync(dbCtx)).ToList();
            org.Admins = (await org.GetAdminsAsync(dbCtx)).ToList();

            org.Roles = (await org.GetOrgRolesAsync(dbCtx)).ToList();

            await org.ReadLicenseOptionsAsync(dbCtx);

            await org.LoadDatabasesAsync(dbCtx);

            org.EncryptDatabases();
            
            return org;
        }
    }
}
