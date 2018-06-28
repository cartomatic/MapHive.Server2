using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MapHive.Core.DataModel;


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace MapHive.Core.DAL
{
    /// <summary>
    /// Customised DbContext that should be used for the IBase models. Takes care of automated creator / editor nad create / modify dates information application
    /// </summary>
    public abstract class BaseDbContext : DbContext
    {
        protected BaseDbContext(DbContextOptions opts) : base(opts)
        {
        }

        private static IConfiguration Configuration { get; set; }

        /// <summary>
        /// Retrieves conn str from app configuration
        /// </summary>
        /// <param name="connStrName"></param>
        /// <returns></returns>
        protected string GetConfiguredConnectionString(string connStrName)
        {
            if (Configuration == null)
            {
                Configuration = Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig();
            }

            return Configuration?[connStrName];
        }

        /// <summary>
        /// Updates some custom IBase related properties
        /// </summary>
        private void UpdateDateProperties()
        {
            //grab all the IBase types
            var changeSet = ChangeTracker.Entries<IBase>();

            if (changeSet == null)
                return;

            //grab user identifier. since got here, user roles should have been checked by the appropriate crud methods, so guid should be obtainable
            //if not present though, must throw
            var guid = Cartomatic.Utils.Identity.GetUserGuid();

            if (!guid.HasValue)
                throw new Exception("Failed to impersonate user.");


            foreach (var entry in changeSet.Where(c => c.State != EntityState.Unchanged))
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreateDateUtc = DateTime.UtcNow;
                    entry.Entity.CreatedBy = guid;
                }
                else
                {
                    entry.Property(x => x.CreateDateUtc).IsModified = false;
                    entry.Property(x => x.CreatedBy).IsModified = false;
                }

                entry.Entity.ModifyDateUtc = DateTime.UtcNow;
                entry.Entity.LastModifiedBy = guid;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Saves changes applying all the custom stuff...
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            UpdateDateProperties();

            return base.SaveChanges();
        }


        /// <inheritdoc />
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            UpdateDateProperties();
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
