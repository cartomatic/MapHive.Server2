using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cartomatic.Utils.Data;
using Cartomatic.Utils.Ef;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

#pragma warning disable 1591
namespace MapHive.Core.IdentityServer.DAL
{
    public class MapHiveIdSrvConfigurationDbContext : ConfigurationDbContext, IProvideDbContextFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:IdentityServer4.EntityFramework.DbContexts.PersistedGrantDbContext" /> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="storeOptions">The store options.</param>
        /// <exception cref="T:System.ArgumentNullException">storeOptions</exception>
        public MapHiveIdSrvConfigurationDbContext(DbContextOptions<ConfigurationDbContext> options, ConfigurationStoreOptions storeOptions) : base(options, storeOptions)
        {
        }

        public DbContext ProduceDbContextInstance(string connStrName = null, bool isConnStr = false,
            DataSourceProvider provider = DataSourceProvider.EfInMemory)
        {
            return new MapHiveIdSrvConfigurationDbContext(
                ((DbContextOptionsBuilder<ConfigurationDbContext>)new DbContextOptionsBuilder<ConfigurationDbContext>().ConfigureIdSrvDbProvider(connStrName, isConnStr, provider)).Options,
                new ConfigurationStoreOptions().ConfigureConfiguratonStoreOptions()
            );
        }

        public DbContext ProduceDefaultDbContextInstance()
        {
            return ProduceDbContextInstance(StoreOptions.DefaultConnStrName, StoreOptions.DefaultIsConnStr, StoreOptions.DefaultDataSourceProvider);
        }
    }
}
