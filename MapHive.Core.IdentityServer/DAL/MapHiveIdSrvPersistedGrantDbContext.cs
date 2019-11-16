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
    public class MapHiveIdSrvPersistedGrantDbContext : PersistedGrantDbContext, IProvideDbContextFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:IdentityServer4.EntityFramework.DbContexts.PersistedGrantDbContext" /> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="storeOptions">The store options.</param>
        /// <exception cref="T:System.ArgumentNullException">storeOptions</exception>
        public MapHiveIdSrvPersistedGrantDbContext(DbContextOptions<PersistedGrantDbContext> options, OperationalStoreOptions storeOptions) : base(options, storeOptions)
        {
        }

        public DbContext ProduceDbContextInstance(string connStrName = null, bool isConnStr = false,
            DataSourceProvider provider = DataSourceProvider.EfInMemory)
        {
            return new MapHiveIdSrvPersistedGrantDbContext(
                ((DbContextOptionsBuilder<PersistedGrantDbContext>)new DbContextOptionsBuilder<PersistedGrantDbContext>().ConfigureIdSrvDbProvider(connStrName, isConnStr, provider)).Options,
                new OperationalStoreOptions().ConfigureOperationalStoreOptions()
            );
        }

        public DbContext ProduceDefaultDbContextInstance()
        {
            return ProduceDbContextInstance(StoreOptions.DefaultConnStrName, StoreOptions.DefaultIsConnStr, StoreOptions.DefaultDataSourceProvider);
        }
    }
}
