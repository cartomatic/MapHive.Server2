using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DataModel;
using MapHive.Core.DAL.TypeConfiguration;

using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DAL
{
    public class MapHiveDbContext : ApplicationDbContext, ILinksDbContext, IMapHiveAppsDbContext, ILocalizedDbContext, IMapHiveUsersDbContext<MapHiveUser>
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
            //Database.SetInitializer<MapHiveDbContext>(null);
            Database.EnsureCreated();
        }

        //common types
        public DbSet<Application> Applications { get; set; }
        public DbSet<MapHiveUser> Users { get; set; }

        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationDatabase> OrganizationDatabases { get; set; }

        public DbSet<Team> Teams { get; set; }

        //tokens
        public DbSet<Token> Tokens { get; set; }


        //ILocalizedDbContext
        public DbSet<LocalizationClass> LocalizationClasses { get; set; }
        public DbSet<TranslationKey> TranslationKeys { get; set; }
        public DbSet<EmailTemplateLocalization> EmailTemplates { get; set; }
        public DbSet<Lang> Langs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("mh_meta");

            //commen types configs
            modelBuilder.ApplyConfiguration(new ApplicationConfiguration());
            modelBuilder.ApplyConfiguration(new MapHiveUserConfiguration());
            modelBuilder.ApplyConfiguration(new OrganizationConfiguration());
            modelBuilder.ApplyConfiguration(new OrganizationDatabaseConfiguration());
            modelBuilder.ApplyConfiguration(new TeamConfiguration());

            //token
            modelBuilder.ApplyConfiguration(new TokenConfiguration());

            //Ilocalised type configs
            modelBuilder.ApplyConfiguration(new LocalizationClassConfiguration());
            modelBuilder.ApplyConfiguration(new EmailTemplateLocalizationConfiguration());
            modelBuilder.ApplyConfiguration(new LangConfiguration());
            modelBuilder.ApplyConfiguration(new TranslationKeyConfiguration());
        }
    }


}
