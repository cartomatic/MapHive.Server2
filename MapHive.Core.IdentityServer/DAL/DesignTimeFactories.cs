using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MapHive.Core.IdentityServer.DAL
{
    public class MapHiveIdSrvPersistedGrantDbContextDesignTimeFactory : IDesignTimeDbContextFactory<MapHiveIdSrvPersistedGrantDbContext>
    {
        /// <summary>Creates a new instance of a derived context.</summary>
        /// <param name="args"> Arguments provided by the design-time service. </param>
        /// <returns> An instance of <typeparamref name="TContext" />. </returns>
        public MapHiveIdSrvPersistedGrantDbContext CreateDbContext(string[] args)
        {
            return Cartomatic.Utils.Ef.DbContextFactory.CreateDbContext<MapHiveIdSrvPersistedGrantDbContext>();
        }
    }

    public class MapHiveIdSrvConfigurationDbContextDesignTimeFactory : IDesignTimeDbContextFactory<MapHiveIdSrvConfigurationDbContext>
    {
        /// <summary>Creates a new instance of a derived context.</summary>
        /// <param name="args"> Arguments provided by the design-time service. </param>
        /// <returns> An instance of <typeparamref name="TContext" />. </returns>
        public MapHiveIdSrvConfigurationDbContext CreateDbContext(string[] args)
        {
            return Cartomatic.Utils.Ef.DbContextFactory.CreateDbContext<MapHiveIdSrvConfigurationDbContext>();
        }
    }
}