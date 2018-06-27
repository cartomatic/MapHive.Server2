using System.CodeDom;
using System.Collections.Generic;
using MapHive.Core.DataModel.Validation;

namespace MapHive.Core.DAL.Migrations.Ef.OrganizationDbCxtMigrations
{
#if NETFULL
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<OrganizationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "MapHive.Core.DAL.OrganizationDbContext";
            MigrationsDirectory = @"DAL\Migrations\Ef\OrganizationDbCxtMigrations";
        }

        protected override void Seed(OrganizationDbContext context)
        {
            Cartomatic.Utils.Identity.ImpersonateGhostUser();

        }
    }
#endif
}
