using System.CodeDom;
using System.Collections.Generic;
using MapHive.Core.DataModel.Validation;

namespace MapHive.Core.DAL.Migrations.Ef.MapHiveDbCtxMigrations
{
#if NETFULL
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<MapHiveDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "MapHive.Core.DAL.MapHiveDbContext";
            MigrationsDirectory = @"DAL\Migrations\Ef\MapHiveDbCtxMigrations";
        }

        protected override void Seed(MapHiveDbContext context)
        {
            Cartomatic.Utils.Identity.ImpersonateGhostUser();

            
        }
    }
#endif
}
