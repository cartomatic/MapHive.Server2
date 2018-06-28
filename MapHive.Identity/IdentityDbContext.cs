using System;
using System.Collections.Generic;
using System.Text;
using Cartomatic.Utils.Data;
using Cartomatic.Utils.Ef;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Identity
{
    public class MapHiveIdentityDbContext : IdentityDbContext
    {
        /// <summary>
        /// Creates instance with the default conn str name
        /// </summary>
        public MapHiveIdentityDbContext()
            : this("MapHiveIdentity")
        {
        }

        /// <summary>
        /// Creates instance with either a specified conn str name or an actual connection str
        /// By default uses npgsql db provider
        /// </summary>
        /// <param name="connStrName"></param>
        /// <param name="isConnStr"></param>
        /// <param name="provider">db provider</param>
        public MapHiveIdentityDbContext(string connStrName, bool isConnStr = false, DataSourceProvider provider = DataSourceProvider.Npgsql)
            : base(DbContextFactory.GetDbContextOptions<MapHiveIdentityDbContext>(connStrName, isConnStr, provider))
        {
        }

        /// <summary>
        /// Creates new instance with the specified opts
        /// </summary>
        /// <param name="opts"></param>
        public MapHiveIdentityDbContext(DbContextOptions opts)
            : base(opts)
        {
        }
    }
}
