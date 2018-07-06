using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cartomatic.Utils.Data;
using Cartomatic.Utils.Ef;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace MapHive.IdentityServer.DAL
{
    public static class StoreOptionsExtensions
    {
        public static OperationalStoreOptions ConfigureOperationalStoreOptions(this OperationalStoreOptions opts)
        {
            opts.ConfigureDbContext = builder =>
                builder.ConfigureIdSrvDbProvider();

            // this enables automatic token cleanup. this is optional.
            opts.EnableTokenCleanup = true;
            opts.TokenCleanupInterval = 3600; //seconds; same as dfault...
            opts.TokenCleanupBatchSize = 100; //same as defualt...
            //TODO - could make it confogurable via app settings...

            opts.DefaultSchema = "operations";

            return opts;
        }

        public static ConfigurationStoreOptions ConfigureConfiguratonStoreOptions(this ConfigurationStoreOptions opts)
        {
            opts.ConfigureDbContext = builder =>
                builder.ConfigureIdSrvDbProvider();

            opts.DefaultSchema = "configuration";

            //not that much else to configure here, unless one wants to dig into tbl cfg.

            return opts;
        }


        public static DbContextOptionsBuilder ConfigureIdSrvDbProvider(
            this DbContextOptionsBuilder builder)
        {
            builder.ConfigureProvider(
                DataSourceProvider.Npgsql,
                Cartomatic.Utils.Ef.DbContextFactory.GetConnStr("MapHiveIdSrv", false)
            );

            return builder;
        }
    }
}
