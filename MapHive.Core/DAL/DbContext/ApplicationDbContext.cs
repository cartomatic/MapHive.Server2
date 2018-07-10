using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using MapHive.Core.DAL.TypeConfigs;
using MapHive.Core.DAL.TypeConfiguration;

using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DAL
{
    /// <summary>
    /// Base for apps specific contexts - provides links & roles related db storage; the actual configurations are app specific
    /// </summary>
    public abstract class ApplicationDbContext : BaseDbContext, ILinksDbContext
    {
        /// <inheritdoc />
        protected ApplicationDbContext(DbContextOptions opts) : base(opts)
        {
        }

        //so we can seed the object types and the db uuids become less cryptic
        public DbSet<ObjectType> ObjectTypes { get; set; }

        //ILinksDbContext
        public DbSet<Link> Links { get; set; }


        public DbSet<Role> Roles { get; set; }

        //public DbSet<Privilege> Privileges { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("mh_meta");

            //type configs
            modelBuilder.ApplyConfiguration(new ObjectTypeConfiguration());
            modelBuilder.ApplyConfiguration(new LinkConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());

            //modelBuilder.ApplyConfiguration(new PrivilegeConfiguration());
        }
    }
}