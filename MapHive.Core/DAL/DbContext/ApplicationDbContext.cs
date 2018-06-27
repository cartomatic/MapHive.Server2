using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.Data;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using MapHive.Core.DAL.TypeConfigs;
using MapHive.Core.DAL.TypeConfiguration;

#if NETFULL
using System.Data.Entity;
#endif
#if NETSTANDARD
using Microsoft.EntityFrameworkCore;
#endif

namespace MapHive.Core.DAL
{
    /// <summary>
    /// Base for apps specific contexts - provides links & roles related db storage; the actual configurations are app specific
    /// </summary>
    public abstract class ApplicationDbContext : BaseDbContext, ILinksDbContext
    {
        protected ApplicationDbContext() : base()
        {
        }

        protected ApplicationDbContext(string connStringName) : base(connStringName)
        {
        }

        protected ApplicationDbContext(DbConnection dbConnection, bool contextOwnsConnection)
            : base(dbConnection, contextOwnsConnection)
        {
#if NETFULL
            Database.SetInitializer<ApplicationDbContext>(null);
#endif
#if NETSTANDARD
            Database.EnsureCreated();
#endif
        }




        //so we can seed the object types and the db uuids become less cryptic
        public DbSet<ObjectType> ObjectTypes { get; set; }

        //ILinksDbContext
        public DbSet<Link> Links { get; set; }


        public DbSet<Role> Roles { get; set; }

        //public DbSet<Privilege> Privileges { get; set; }


#if NETFULL
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ObjectTypeConfiguration());
            modelBuilder.Configurations.Add(new LinkConfiguration());
            modelBuilder.Configurations.Add(new RoleConfiguration());

            //modelBuilder.Configurations.Add(new PrivilegeConfiguration());

            base.OnModelCreating(modelBuilder);
        }

#endif

#if NETSTANDARD
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("mh_meta");

            //type configs
            modelBuilder.ApplyConfiguration(new ObjectTypeConfiguration());
            modelBuilder.ApplyConfiguration(new LinkConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());

            //modelBuilder.ApplyConfiguration(new PrivilegeConfiguration());
        }
#endif
    }
}