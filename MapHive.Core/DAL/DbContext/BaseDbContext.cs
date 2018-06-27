using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MapHive.Core.DataModel;


#if NETFULL
using System.Data.Entity;
#endif
#if NETSTANDARD
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
#endif

namespace MapHive.Core.DAL
{
    /// <summary>
    /// Customised DbContext that should be used for the IBase models. Takes care of automated creator / editor nad create / modify dates information application
    /// </summary>
    public abstract class BaseDbContext : DbContext
    {
        protected BaseDbContext()
            //Note:
            //need a paramless constructor that passes a non empty conn string or conn string name to DbContext
            //Obviously using such a db context will make it throw, so this is not a constructor one would use.
            //although providing a valid paramles ctor in a derived class (one that calls base("some_real_conn_str_name") will do.
            : this("need_this_for_migrations!!!")
        {
        }

#if NETFULL
        protected BaseDbContext(string connStringName) : base(connStringName)
        {
        }

        protected BaseDbContext(DbConnection conn, bool contextOwnsConnection) : base(conn, contextOwnsConnection) {
        }
#endif



#if NETSTANDARD

        protected BaseDbContext(string connStringName)
            : base(GetDbContextOptions(connStringName: connStringName))
        {
        }

        protected BaseDbContext(DbConnection conn, bool contextOwnsConnection) :
            base(GetDbContextOptions(conn: conn, contextOwnsConnection: contextOwnsConnection))
        {
        }

        static DbContextOptions<DbContext> GetDbContextOptions(string connStringName = null, bool contextOwnsConnection = true, DbConnection conn = null)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DbContext>();

            //FIXME
            throw new Exception("WHOAAAAAA, a bit more work to do on the db context for maphive server ZWEI!");

            //TODO - how the hell 

            //optionsBuilder.UseNpgsql(Configuration.GetConnectionString("ElasticSearchLiveUpdate"));

            return optionsBuilder.Options;
        }

#endif


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
