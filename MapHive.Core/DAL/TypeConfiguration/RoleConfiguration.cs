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
            //roles take the default off the model builder, so can be reused in other schemas too!
            builder.ApplyIBaseConfiguration(nameof(Role), "roles");

            builder.Property(en => en.Identifier).HasColumnName("identifier");
            builder.Property(en => en.Name).HasColumnName("name");
            builder.Property(en => en.Description).HasColumnName("description");

            builder.Ignore(en => en.Privileges);
            builder.Property(en => en.PrivilegesSerialized).HasColumnName("privileges");
        }
    }
}
