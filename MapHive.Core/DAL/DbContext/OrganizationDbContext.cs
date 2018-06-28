using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MapHive.Core.DAL
{
    /// <summary>
    /// Context used for org db creation
    /// </summary>
    public class OrganizationDbContext : BaseDbContext
    {

        /// <summary>
        /// paramless ctro so can use it as a generic param!
        /// </summary>
        public OrganizationDbContext() : this("dummy_conn_str")
        {
            throw new InvalidOperationException($"You tried to use a paramless {nameof(OrganizationDbContext)}... You should really not do that and use the one that requires a conn str name...");
        }

        /// <summary>
        /// Creates instance with either a specified conn str name or an actual connection str
        /// </summary>
        /// <param name="connStrName"></param>
        /// <param name="isConnStr"></param>
        public OrganizationDbContext(string connStrName, bool isConnStr = false)
            : base (DbContextFactory.GetDbContextOptions<OrganizationDbContext>(connStrName))
        {
        }
    }
}
