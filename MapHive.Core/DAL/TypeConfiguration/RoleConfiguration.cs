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
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ApplyIBaseConfiguration(nameof(Role), "roles", "mh_meta");

            builder.Property(en => en.Identifier).HasColumnName("identifier");
            builder.Property(en => en.Name).HasColumnName("name");
            builder.Property(en => en.Description).HasColumnName("description");

            builder.Ignore(en => en.Privileges);
            builder.Property(en => en.PrivilegesSerialized).HasColumnName("privileges");
        }
    }
}
