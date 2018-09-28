using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;

using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Ef;
using MapHive.Core.DAL;
using Newtonsoft.Json;
using Npgsql;

using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel
{
    public partial class OrganizationDatabase
    {
        /// <summary>
        /// Creates or updates a database with a simple OrganizationDbContext; this is useful for creating empty databases
        /// </summary>
        /// <returns></returns>
        public DbContextMigrator.CreateOrUpdateDatabaseOutput CreateOrUpdateDatabase()
        {
            //no specific db context provided; will use a blank OrganizationDbContext
            var ctx = GetDbContext();

            return CreateOrUpdateDatabase(ctx);
        }

        /// <summary>
        /// Creates or updates a db for a specified generic db ctx type
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <returns></returns>
        public DbContextMigrator.CreateOrUpdateDatabaseOutput CreateOrUpdateDatabase<TDbContext>()
            where TDbContext : DbContext, IProvideDbContextFactory, new()
        {
            var ctx = GetDbContext<TDbContext>();

            return CreateOrUpdateDatabase(ctx);
        }

        /// <summary>
        /// Creates or updates a database for a specified db context instance
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public DbContextMigrator.CreateOrUpdateDatabaseOutput CreateOrUpdateDatabase(DbContext ctx)
        {
            //net full approach - needed a db migration configuration
            //net core does not use such class

            //-------------------------------

            //configuration.TargetDatabase = new DbConnectionInfo(GetConnectionString(), DataSourceProvider.ToString());

            //var migrator = new DbMigrator(configuration);

            ////if there are any pending migratgions, it means the db needs some attentions. therefore a line below should update it. 
            ////otherwise, an update will not take place.
            //var output = new CreateOrUpdateDatabaseOutput
            //{
            //    Created = !migrator.GetDatabaseMigrations().Any() && migrator.GetLocalMigrations().Any(),
            //    Updated = migrator.GetPendingMigrations().Any()
            //};

            //migrator.Update();
            //-------------------------------

            //redirect the work to a standardised util
            return ctx.CreateOrUpdateDatabase();

        }
    }
}
