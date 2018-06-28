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
        /// <summary>
        /// Creates instance with the default conn str name
        /// </summary>
        public MapHiveDbContext()
            : this("MapHiveMetadata")
        {
        }

        /// <summary>
        /// Creates instance with either a specified conn str name or an actual connection str
        /// </summary>
        /// <param name="connStrName"></param>
        /// <param name="isConnStr"></param>
        public MapHiveDbContext(string connStrName, bool isConnStr = false)
            : base(DbContextFactory.GetDbContextOptions<MapHiveDbContext>(connStrName, isConnStr))
        {
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
