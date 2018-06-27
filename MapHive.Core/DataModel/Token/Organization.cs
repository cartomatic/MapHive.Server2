using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DAL;

#if NETFULL
using System.Data.Entity;
#endif
#if NETSTANDARD
using Microsoft.EntityFrameworkCore;
#endif

namespace MapHive.Core.DataModel
{
    public partial class Token
    {
        /// <summary>
        /// Sets an orgnization for a token
        /// </summary>
        /// <param name="org"></param>
        public void SetOrganization(Organization org)
        {
            this.OrganizationId = org?.Uuid;
        }

        /// <summary>
        /// Gets organization assigned to token
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        public async Task<Organization> GetOrganization(MapHiveDbContext dbCtx)
        {
            return await dbCtx.Organizations.FirstOrDefaultAsync(org => org.Uuid == OrganizationId);
        }
    }
}
