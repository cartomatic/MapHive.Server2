using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cartomatic.Utils.Data;
using Cartomatic.Utils.Ef;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.IdentityServer.DAL
{
#pragma warning disable 1591
    public static class StoreOptions
    {
        public const string DefaultConnStrName = "MapHiveIdSrv";
        public const bool DefaultIsConnStr = false;
        public const DataSourceProvider DefaultDataSourceProvider = DataSourceProvider.Npgsql;


        public static OperationalStoreOptions ConfigureOperationalStoreOptions(this OperationalStoreOptions opts)
        {
            opts.ConfigureDbContext = builder =>
                builder.ConfigureIdSrvDbProvider(DefaultConnStrName, DefaultIsConnStr, DefaultDataSourceProvider);

            // this enables automatic token cleanup. this is optional.
            opts.EnableTokenCleanup = true;
            opts.TokenCleanupInterval = 3600; //seconds; same as dfault...
            opts.TokenCleanupBatchSize = 100; //same as default...
            //TODO - could make it confogurable via app settings...

            opts.DefaultSchema = "operations";

            return opts;
        }

        public static ConfigurationStoreOptions ConfigureConfiguratonStoreOptions(this ConfigurationStoreOptions opts)
        {
            opts.ConfigureDbContext = builder =>
                builder.ConfigureIdSrvDbProvider(DefaultConnStrName, DefaultIsConnStr, DefaultDataSourceProvider);

            opts.DefaultSchema = "configuration";

            //not that much else to configure here, unless one wants to dig into tbl cfg.

            return opts;
        }

        public static DbContextOptionsBuilder ConfigureIdSrvDbProvider(
            this DbContextOptionsBuilder builder, string connStrName, bool isConnStr, DataSourceProvider provider)
        {
            builder.ConfigureProvider(
                provider,
                Cartomatic.Utils.Ef.DbContextFactory.GetConnStr(connStrName, isConnStr)
            );

            return builder;
        }
    }
}
