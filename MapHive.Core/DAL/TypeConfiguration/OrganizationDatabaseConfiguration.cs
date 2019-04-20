using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Ef;
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

            builder.Property(p => p.OrganizationId).HasColumnName("organization_id");
            builder.Property(p => p.Identifier).HasColumnName("identifier");
            builder.Property(p => p.UserName).HasColumnName("user_name");
            builder.Property(p => p.Pass).HasColumnName("password");
            builder.Property(p => p.ServerHost).HasColumnName("server_host");
            builder.Property(p => p.ServerPort).HasColumnName("server_port");
            builder.Property(p => p.DbName).HasColumnName("db_name");

            builder.Property(p => p.ServiceUserName).HasColumnName("service_user_name");
            builder.Property(p => p.ServiceUserPass).HasColumnName("service_user_password");
            builder.Property(p => p.ServiceDb).HasColumnName("service_db_name");

            builder.Property(p => p.DataSourceProvider).HasColumnName("provider");

            builder.HasIndex(p => new { p.OrganizationId, p.Identifier }) //this should create a unique composite field idx!
                .HasName($"idx_{nameof(OrganizationDatabase).ToColumnName()}_uq_org_uuid_identifier")
                .IsUnique();
        }
    }
}
