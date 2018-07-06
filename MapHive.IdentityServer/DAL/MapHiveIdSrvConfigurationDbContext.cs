using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace MapHive.IdentityServer.DAL
{
    public class MapHiveIdSrvConfigurationDbContext : ConfigurationDbContext
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
    }
}
