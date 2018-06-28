using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MapHive.Core.DataModel;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MapHive.Core.DAL.TypeConfiguration
{
    public class OrganizationDatabaseConfiguration : IEntityTypeConfiguration<OrganizationDatabase>
    {
        public void Configure(EntityTypeBuilder<OrganizationDatabase> builder)
        {
            builder.ApplyIBaseConfiguration(nameof(OrganizationDatabase), "organization_databases", "mh_meta");

            builder.Property(en => en.OrganizationId).HasColumnName("organization_id");
            builder.Property(en => en.Identifier).HasColumnName("identifier");
            builder.Property(en => en.UserName).HasColumnName("user_name");
            builder.Property(en => en.Pass).HasColumnName("password");
            builder.Property(en => en.ServerHost).HasColumnName("server_host");
            builder.Property(en => en.ServerPort).HasColumnName("server_port");
            builder.Property(en => en.DbName).HasColumnName("db_name");

            builder.Property(en => en.ServiceUserName).HasColumnName("service_user_name");
            builder.Property(en => en.ServiceUserPass).HasColumnName("service_user_password");
            builder.Property(en => en.ServiceDb).HasColumnName("service_db_name");

            builder.Property(en => en.DataSourceProvider).HasColumnName("provider");

            builder.HasIndex(t => new { t.OrganizationId, t.Identifier }) //this should create a unique composite field idx!
                .HasName($"uq_db_identifier_{nameof(OrganizationDatabase).ToLower()}")
                .IsUnique();
        }
    }
}
