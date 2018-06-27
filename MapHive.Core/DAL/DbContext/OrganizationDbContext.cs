using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if NETFULL
using System.Data.Entity;
#endif
#if NETSTANDARD
using Microsoft.EntityFrameworkCore;
#endif

namespace MapHive.Core.DAL
{
    /// <summary>
    /// Context used for org db creation
    /// </summary>
    public class OrganizationDbContext : BaseDbContext
    {
        public OrganizationDbContext() : base()
        {
        }

        public OrganizationDbContext(DbConnection dbConnection, bool contextOwnsConnection)
            : base(dbConnection, contextOwnsConnection)
        {
#if NETFULL
            Database.SetInitializer<OrganizationDbContext>(null);
#endif
#if NETSTANDARD
            Database.EnsureCreated();
#endif
        }
    }
}
