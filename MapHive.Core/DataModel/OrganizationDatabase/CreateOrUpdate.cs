using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;

using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Npgsql;

#if NETFULL
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
#endif

#if NETSTANDARD
using Microsoft.EntityFrameworkCore;
#endif

namespace MapHive.Core.DataModel
{
    public partial class OrganizationDatabase
    {
        public class CreateOrUpdateDatabaseOutput
        {
            public bool Created { get; set; }

            public bool Updated { get; set; }
        }

#if NETFULL
        /// <summary>
        /// Method create database using migration configuration or update if database exist
        /// </summary>
        public CreateOrUpdateDatabaseOutput CreateOrUpdateDatabase(DbMigrationsConfiguration configuration = null)
        {
            //if no specific cfg provided, do use the 'blank' organisation cfg.
            if (configuration == null)
                configuration = new DAL.Migrations.Ef.OrganizationDbCxtMigrations.Configuration();

            configuration.TargetDatabase = new DbConnectionInfo(GetConnectionString(), DataSourceProvider.ToString());

            var migrator = new DbMigrator(configuration);

            //if there are any pending migratgions, it means the db needs some attentions. therefore a line below should update it. 
            //otherwise, an update will not take place.
            var output = new CreateOrUpdateDatabaseOutput
            {
                Created = !migrator.GetDatabaseMigrations().Any() && migrator.GetLocalMigrations().Any(),
                Updated = migrator.GetPendingMigrations().Any()
            };

            migrator.Update();

            return output;
        }
#endif

#if NETSTANDARD
        public CreateOrUpdateDatabaseOutput CreateOrUpdateDatabase()
        {
            var ctx = GetDbContext();

            var output = new CreateOrUpdateDatabaseOutput
            {   
                //FIXME... 
                //for the time being just return false

                //TODO - work out the way of knowing when db has been created, when updated.
            };

            ctx.Database.Migrate();

            return output;
        }
#endif

    }
}
