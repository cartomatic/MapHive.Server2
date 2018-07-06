using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Data;
using Cartomatic.Utils.Ef;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MapHive.Core.DAL
{
    /// <summary>
    /// Context used for org db creation
    /// </summary>
    public class OrganizationDbContext : BaseDbContext, IProvideDbContextFactory
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
        /// By default uses npgsql db provider
        /// </summary>
        /// <param name="connStrName"></param>
        /// <param name="isConnStr"></param>
        /// <param name="provider">db provider</param>
        public OrganizationDbContext(string connStrName, bool isConnStr = false, DataSourceProvider provider = DataSourceProvider.Npgsql)
            : base (DbContextFactory.GetDbContextOptions<OrganizationDbContext>(connStrName, isConnStr, provider))
        {
        }

        public DbContext ProduceDbContextInstance(string connStrName = null, bool isConnStr = false,
            DataSourceProvider provider = DataSourceProvider.EfInMemory)
        {
            return new OrganizationDbContext(connStrName, isConnStr, provider);
        }

        public DbContext ProduceDefaultDbContextInstance()
        {
            //this will throw deeper of course!!!
            return new OrganizationDbContext();
        }
    }
}
