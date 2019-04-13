using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Migrations.Internal;

namespace MapHive.Core.DAL
{
    public class MapHiveDbContextHistoryRepository : NpgsqlHistoryRepository
    {
        public MapHiveDbContextHistoryRepository(HistoryRepositoryDependencies dependencies)
            : base(dependencies)
        {
        }

        protected override string TableSchema => "mh_meta";
        protected override string TableName => "__migrations_history";

        protected override void ConfigureTable(EntityTypeBuilder<HistoryRow> history)
        {
            base.ConfigureTable(history);

            history.Property(h => h.MigrationId).HasColumnName("migration_id");
            history.Property(h => h.ProductVersion).HasColumnName("product_version");
        }
    }
}
