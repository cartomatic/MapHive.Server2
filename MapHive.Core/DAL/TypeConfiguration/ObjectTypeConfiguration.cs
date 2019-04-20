using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Ef;
using MapHive.Core.DataModel;
using MapHive.Core.DAL.TypeConfiguration;


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MapHive.Core.DAL.TypeConfigs
{
    public class ObjectTypeConfiguration : IEntityTypeConfiguration<ObjectType>
    {
        public void Configure(EntityTypeBuilder<ObjectType> builder)
        {
            //object types take the default off the model builder, so can be reused in other schemas too!
            builder.ToTable(name: "object_types");

            builder.HasKey(p => p.Uuid);

            builder.Property(p => p.Uuid).HasColumnName("uuid");
            builder.Property(p => p.Name).HasColumnName("name");

            builder.HasIndex(t => t.Name)
                .HasName($"idx_{nameof(ObjectType).ToColumnName()}_uq_name")
                .IsUnique();
        }
    }
}
