using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DataModel;
using MapHive.Core.DAL.TypeConfiguration;
#if NETFULL
using System.Data.Entity;
#endif
#if NETSTANDARD
using Microsoft.EntityFrameworkCore;
#endif

namespace MapHive.Core.DAL
{
    public class MapHiveDbContext : BaseDbContext, ILinksDbContext, IMapHiveAppsDbContext, IMapHiveUsersDbContext<MapHiveUser>
    {
        public MapHiveDbContext()
            : this("MapHiveMeta") //use a default conn str name; useful when passing ctx as a generic param that is then instantiated 
        {
        }

        public MapHiveDbContext(string connStringName)
            : base (connStringName)
        {
        }

        public MapHiveDbContext(DbConnection conn, bool contextOwnsConnection) 
            : base (conn, contextOwnsConnection)
        {
        }

        public DbSet<Application> Applications { get; set; }
        public DbSet<MapHiveUser> Users { get; set; }

        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Team> Teams { get; set; }

        //ILinksDbContext
        public DbSet<Link> Links { get; set; }

        //ILocalised
        //public DbSet<LocalisationClass> LocalisationClasses { get; set; }
        //public DbSet<TranslationKey> TranslationKeys { get; set; }
        //public DbSet<EmailTemplateLocalisation> EmailTemplates { get; set; }
        //public DbSet<Lang> Langs { get; set; }

#if NETFULL
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("mh_meta");

            //type configs
            modelBuilder.Configurations.Add(new ApplicationConfiguration());
            modelBuilder.Configurations.Add(new MapHiveUserConfiguration());
            modelBuilder.Configurations.Add(new OrganizationConfiguration());
            modelBuilder.Configurations.Add(new RoleConfiguration());
            modelBuilder.Configurations.Add(new TeamConfiguration());

            modelBuilder.Configurations.Add(new LinkConfiguration());

            //Ilocalised type configs
            //modelBuilder.Configurations.Add(new LocalisationClassConfiguration());
            //modelBuilder.Configurations.Add(new EmailTemplateLocalisationConfiguration());
            //modelBuilder.Configurations.Add(new LangConfiguration());
            //modelBuilder.Configurations.Add(new TranslationKeyConfiguration());

        }
#endif

#if NETSTANDARD
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("mh_meta");

            //type configs
            modelBuilder.ApplyConfiguration(new ApplicationConfiguration());
            modelBuilder.ApplyConfiguration(new MapHiveUserConfiguration());
            modelBuilder.ApplyConfiguration(new OrganizationConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new TeamConfiguration());

            modelBuilder.ApplyConfiguration(new LinkConfiguration());

            //Ilocalised type configs
            //modelBuilder.Configurations.Add(new LocalisationClassConfiguration());
            //modelBuilder.Configurations.Add(new EmailTemplateLocalisationConfiguration());
            //modelBuilder.Configurations.Add(new LangConfiguration());
            //modelBuilder.Configurations.Add(new TranslationKeyConfiguration());
        }
#endif
    }

    
}
