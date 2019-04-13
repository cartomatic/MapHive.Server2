using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace MapHive.Core.DAL
{
    public static class DbContextMigrator
    {
        /// <summary>
        /// Db creation output
        /// </summary>
        public class CreateOrUpdateDatabaseOutput
        {
            /// <summary>
            /// Whether or not a database has been created by the last migration
            /// </summary>
            public bool Created { get; set; }

            /// <summary>
            /// whether or not a databas has been update by the last migration
            /// </summary>
            public bool Updated { get; set; }
        }

        /// <summary>
        /// Migrates db context to the lates version. if context des not need migrations nothing happens
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        public static CreateOrUpdateDatabaseOutput CreateOrUpdateDatabase(this DbContext dbCtx)
        {
            var output = new CreateOrUpdateDatabaseOutput
            {
                Created = !(dbCtx.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists(),
                Updated = dbCtx.Database.GetPendingMigrations().Any()
            };

            dbCtx.Database.Migrate();

            //pass the dbctx to extended views creator and seeder.
            //they will take care of them if required
            if (output.Created || output.Updated)
            {
                Seeder.SeedAll(dbCtx);
                ExtendedViewsCreator.CreateAll(dbCtx);
            }

            return output;
        }
    }
}
