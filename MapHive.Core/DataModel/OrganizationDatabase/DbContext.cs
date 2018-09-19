using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Ef;
using MapHive.Core.DAL;
using Newtonsoft.Json;
using Npgsql;


using Microsoft.EntityFrameworkCore;


namespace MapHive.Core.DataModel
{
    public partial class OrganizationDatabase
    {
        /// <summary>
        /// Gets db context instance for this org db object
        /// </summary>
        /// <returns></returns>
        public OrganizationDbContext GetDbContext()
        {
            return GetDbContext<OrganizationDbContext>();
        }

        /// <summary>
        /// Gets db context instance for this org db object
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="superUser">whether or not create context with user elevated to superuser - sometimes required for ops such as COPY</param>
        /// <returns></returns>
        public TDbContext GetDbContext<TDbContext>(bool superUser = false)
            where TDbContext : DbContext, IProvideDbContextFactory, new()
        {

            //full framework
            //var conn = Cartomatic.Utils.Data.DbProviderFactories.GetFactory(DataSourceProvider.ToString()).CreateConnection();
            //conn.ConnectionString = GetConnectionString(superUser: superUser);

            var connStr = GetConnectionString(superUser: superUser);

            //assume here the context has a constructor that takes in connStr, flag confirming it's a connStr, and a data source provider
            //the constructor should have signatrue that is an equivalent to IProvideDbContextFactory.ProduceDbContextInstance(string connStrName = null, bool isConnStr = false, DataSourceProvider provider = DataSourceProvider.EfInMemory)
            var dbCtx = (TDbContext)Activator.CreateInstance(typeof(TDbContext));
            return (TDbContext)dbCtx.ProduceDbContextInstance(connStr, true, DataSourceProvider);
        }
    }
}
