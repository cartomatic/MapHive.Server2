using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            where TDbContext : DbContext, new()
        {

            //full framework
            //var conn = Cartomatic.Utils.Data.DbProviderFactories.GetFactory(DataSourceProvider.ToString()).CreateConnection();
            //conn.ConnectionString = GetConnectionString(superUser: superUser);

            var connStr = GetConnectionString(superUser: superUser);

            return (TDbContext)Activator.CreateInstance(typeof(TDbContext), new object[] { connStr, true });
        }
    }
}
