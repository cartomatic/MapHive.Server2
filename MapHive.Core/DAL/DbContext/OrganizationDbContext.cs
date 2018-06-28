using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

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
            //Database.SetInitializer<OrganizationDbContext>(null);
            Database.EnsureCreated();
        }
    }
}
