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
    public class RoleConfiguration : EntityTypeConfiguration<Role>
    {
        public RoleConfiguration()
        {
            ToTable("roles", "mh_meta");
            this.ApplyIBaseConfiguration(nameof(Role));

            Property(p => p.Identifier).HasColumnName("identifier");
            Property(p => p.Name).HasColumnName("name");
            Property(p => p.Description).HasColumnName("description");
        }
    }
#endif

#if NETSTANDARD
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ApplyIBaseConfiguration(nameof(Role), "roles", "mh_meta");

            builder.Property(en => en.Identifier).HasColumnName("identifier");
            builder.Property(en => en.Name).HasColumnName("name");
            builder.Property(en => en.Description).HasColumnName("description");
        }
    }
#endif
}
