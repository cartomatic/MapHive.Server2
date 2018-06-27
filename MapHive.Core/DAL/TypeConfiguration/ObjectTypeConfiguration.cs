using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.Data;
using MapHive.Core.DAL.TypeConfiguration;


#if NETFULL
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Infrastructure.Annotations;
#endif

#if NETSTANDARD
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
#endif

namespace MapHive.Core.DAL.TypeConfigs
{
#if NETFULL
    public class ObjectTypeConfiguration : EntityTypeConfiguration<ObjectType>
    {
        public ObjectTypeConfiguration()
        {
            ToTable("object_types");

            HasKey(t => t.Uuid);
            Property(en => en.Uuid).HasColumnName("uuid");
            Property(en => en.Name).HasColumnName("name");

            Property(t => t.Name)
                .HasColumnAnnotation(
                    "Index",
                    new IndexAnnotation(new IndexAttribute("uq_name") { IsUnique = true }));
        }
    }
#endif

#if NETSTANDARD
public class ObjectTypeConfiguration : IEntityTypeConfiguration<ObjectType>
    {
        public void Configure(EntityTypeBuilder<ObjectType> builder)
        {
            builder.ToTable(name: "object_types", schema: "mh_meta");

            builder.HasKey(p => p.Uuid);

            builder.Property(en => en.Uuid).HasColumnName("uuid");
            builder.Property(en => en.Name).HasColumnName("name");

            builder.HasIndex(t => t.Name)
                .HasName($"uq_name_{nameof(ObjectType).ToLower()}")
                .IsUnique();
        }
    }
#endif
}
