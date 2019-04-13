using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Data;
using Cartomatic.Utils.Ef;
using MapHive.Core.DataModel;
using MapHive.Core.DAL.TypeConfiguration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MapHive.Core.DAL
{
    /// <summary>
    /// Maphive db ctx
    /// </summary>
    public class MapHiveDbContext : ApplicationDbContext, ILinksDbContext, IMapHiveAppsDbContext, ILocalizedDbContext, IMapHiveUsersDbContext<MapHiveUser>, IProvideDbContextFactory
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
        /// By default uses npgsql db provider
        /// </summary>
        /// <param name="connStrName"></param>
        /// <param name="isConnStr"></param>
        /// <param name="provider">db provider</param>
        public MapHiveDbContext(string connStrName, bool isConnStr = false, DataSourceProvider provider = DataSourceProvider.Npgsql)
            : base(DbContextFactory.GetDbContextOptions<MapHiveDbContext>(connStrName, isConnStr, provider))
        {
        }


        /// <inheritdoc />
        public DbContext ProduceDbContextInstance(string connStrName = null, bool isConnStr = false,
            DataSourceProvider provider = DataSourceProvider.EfInMemory)
        {
            return new MapHiveDbContext(connStrName, isConnStr, provider);
        }

        /// <inheritdoc />
        public DbContext ProduceDefaultDbContextInstance()
        {
            return new MapHiveDbContext();
        }


        //common types
        public DbSet<Application> Applications { get; set; }
        public DbSet<MapHiveUser> Users { get; set; }

        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationDatabase> OrganizationDatabases { get; set; }

        public DbSet<Team> Teams { get; set; }

        //tokens
        public DbSet<Token> Tokens { get; set; }

        //resources
        public DbSet<Resource> Resources { get; set; }

        //ILocalizedDbContext
        public DbSet<LocalizationClass> LocalizationClasses { get; set; }
        public DbSet<TranslationKey> TranslationKeys { get; set; }
        public DbSet<EmailTemplateLocalization> EmailTemplates { get; set; }
        public DbSet<Lang> Langs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //important to set up schema prior to calling base, so base is aware of the desired schema
            modelBuilder.HasDefaultSchema("mh_meta");

            base.OnModelCreating(modelBuilder);

            //common types configs
            modelBuilder.ApplyConfiguration(new ApplicationConfiguration());
            modelBuilder.ApplyConfiguration(new MapHiveUserConfiguration());
            modelBuilder.ApplyConfiguration(new OrganizationConfiguration());
            modelBuilder.ApplyConfiguration(new OrganizationDatabaseConfiguration());
            modelBuilder.ApplyConfiguration(new TeamConfiguration());

            //token
            modelBuilder.ApplyConfiguration(new TokenConfiguration());

            //resource
            modelBuilder.ApplyConfiguration(new ResourceConfiguration());

            //Ilocalised type configs
            modelBuilder.ApplyConfiguration(new LocalizationClassConfiguration());
            modelBuilder.ApplyConfiguration(new EmailTemplateLocalizationConfiguration());
            modelBuilder.ApplyConfiguration(new LangConfiguration());
            modelBuilder.ApplyConfiguration(new TranslationKeyConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.ReplaceService<IHistoryRepository, MapHiveDbContextHistoryRepository>();
        }
    }
}
