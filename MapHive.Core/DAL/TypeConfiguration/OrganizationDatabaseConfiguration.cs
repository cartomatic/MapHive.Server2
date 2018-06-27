using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MapHive.Core.DataModel;

#if NETFULL
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Infrastructure.Annotations;
#endif

#if NETSTANDARD
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
#endif

namespace MapHive.Core.DAL.TypeConfiguration
{
#if NETFULL
    public class OrganizationDatabaseConfiguration : EntityTypeConfiguration<OrganizationDatabase>
    {
        public OrganizationDatabaseConfiguration()
        {
            ToTable("organization_databases", "mh_meta");
            this.ApplyIBaseConfiguration(nameof(OrganizationDatabase));

            Property(en => en.OrganizationId).HasColumnName("organization_id");
            Property(en => en.Identifier).HasColumnName("identifier");
            Property(en => en.UserName).HasColumnName("user_name");
            Property(en => en.Pass).HasColumnName("password");
            Property(en => en.ServerHost).HasColumnName("server_host");
            Property(en => en.ServerPort).HasColumnName("server_port");
            Property(en => en.DbName).HasColumnName("db_name");

            Property(en => en.ServiceUserName).HasColumnName("service_user_name");
            Property(en => en.ServiceUserPass).HasColumnName("service_user_password");
            Property(en => en.ServiceDb).HasColumnName("service_db_name");

            Property(en => en.DataSourceProvider).HasColumnName("provider");

             //unique per org-identifier
            Property(t => t.OrganizationId)
                .HasColumnAnnotation(
                    "Index",
                    new IndexAnnotation(new IndexAttribute("uq_db_identifier") { IsUnique = true, Order = 1 }));
            Property(t => t.Identifier)
                .HasColumnAnnotation(
                    "Index",
                    new IndexAnnotation(new IndexAttribute("uq_db_identifier") { IsUnique = true, Order = 2 }));
        }
    }
#endif

#if NETSTANDARD
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
                .HasName($"uq_db_identifier")
                .IsUnique();
        }
    }
#endif
}
