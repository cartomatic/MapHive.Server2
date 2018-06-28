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

using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel
{
    public partial class OrganizationDatabase
    {
        public class CreateOrUpdateDatabaseOutput
        {
            public bool Created { get; set; }

            public bool Updated { get; set; }
        }

        public CreateOrUpdateDatabaseOutput CreateOrUpdateDatabase()
        {
            //net full approach
            //if no specific cfg provided, do use the 'blank' organisation cfg.
            //if (configuration == null)
            //    configuration = new DAL.Migrations.Ef.OrganizationDbCxtMigrations.Configuration();

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

    }
}
