using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrockAllen.MembershipReboot.Ef;
using System.Data.Common;


#if NETFULL
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
#endif

#if NETSTANDARD
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
#endif

namespace MapHive.MembershipReboot
{
    public class CustomDbContext : MembershipRebootDbContext<CustomUserAccount, CustomGroup>
    {
        //Note: paramless constructor needed when adding migrations
        public CustomDbContext() { }

        public CustomDbContext(string connStrName)
            : base(connStrName)
        {
            // where "Configuration" comes from Migrations.Configuration
            // so migration take place first, which includes db create
            // and "Seed" method can be used to seed db with initiall data


            //NOTE:
            //it is important to have the conn string passed to the Db initializer, so the db is created properly!
            //otherwise it will not be able to create a db in the proper context!
#if NETFULL
            Database.SetInitializer<CustomDbContext>(new MigrateDatabaseToLatestVersion<CustomDbContext, Migrations.Ef.Configuration>(connStrName));
#endif
#if NETSTANDARD
            //looks like a little problem with mbr dbctx...
            //Database.EnsureCreated();
#endif
        }

#if NETFULL
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //need to override the schema for the object here as it defaults to dbo of course
            modelBuilder.HasDefaultSchema("mr");

            //TODO - need to workout what objects do actually get created! and maybe make their names more postgres like - lowercase. this way all the potential querries will be easier to write! Though remapping them may become cumbersome when updates are available!

            base.OnModelCreating(modelBuilder);
        }
#endif

#if NETSTANDARD
        //looks like a little problem with mbr dbctx...

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.HasDefaultSchema("mr");

        //    base.OnModelCreating(modelBuilder);
        //}
#endif
    }
}
