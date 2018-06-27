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
    public class LinkConfiguration : EntityTypeConfiguration<Link>
    {
        public LinkConfiguration(string table = "links")
        {
            ToTable(table, "mh_meta");

            HasKey(p => p.Id);
            Property(en => en.Id).HasColumnName("id");

            Property(en => en.ParentUuid).HasColumnName("parent_uuid");
            Property(en => en.ChildUuid).HasColumnName("child_uuid");
            Property(en => en.ParentTypeUuid).HasColumnName("parent_type_uuid");
            Property(en => en.ChildTypeUuid).HasColumnName("child_type_uuid");
            Property(en => en.SortOrder).HasColumnName("sort_order");

            Property(p => p.LinkData.Serialized).HasColumnName("link_json_data");

            Property(t => t.ParentUuid)
                .HasColumnAnnotation(
                    "Index",
                    new IndexAnnotation(new IndexAttribute("idx_parent_uuid") { }));

            Property(t => t.ChildUuid)
                .HasColumnAnnotation(
                    "Index",
                    new IndexAnnotation(new IndexAttribute("idx_child_uuid") { }));

            Property(t => t.ParentTypeUuid)
                .HasColumnAnnotation(
                    "Index",
                    new IndexAnnotation(new IndexAttribute("idx_parent_type_uuid") { }));

            Property(t => t.ChildTypeUuid)
                .HasColumnAnnotation(
                    "Index",
                    new IndexAnnotation(new IndexAttribute("idx_child_type_uuid") { }));

        }
    }
#endif

#if NETSTANDARD
    public class LinkConfiguration : IEntityTypeConfiguration<Link>
    {
        public void Configure(EntityTypeBuilder<Link> builder)
        {
            builder.ToTable(name: "links", schema: "mh_meta");

            builder.HasKey(p => p.Id);
            builder.Property(en => en.Id).HasColumnName("id");

            builder.Property(en => en.ParentUuid).HasColumnName("parent_uuid");
            builder.Property(en => en.ChildUuid).HasColumnName("child_uuid");
            builder.Property(en => en.ParentTypeUuid).HasColumnName("parent_type_uuid");
            builder.Property(en => en.ChildTypeUuid).HasColumnName("child_type_uuid");
            builder.Property(en => en.SortOrder).HasColumnName("sort_order");

            builder.Property(p => p.LinkData.Serialized).HasColumnName("link_json_data");


            builder.HasIndex(t => t.ParentUuid)
                .HasName($"idx_parent_{nameof(Link).ToLower()}")
                .IsUnique();

            builder.HasIndex(t => t.ChildUuid)
                .HasName($"idx_child_uuid_{nameof(Link).ToLower()}")
                .IsUnique();

            builder.HasIndex(t => t.ParentTypeUuid)
                .HasName($"idx_parent_type_uuid_{nameof(Link).ToLower()}")
                .IsUnique();

            builder.HasIndex(t => t.ChildTypeUuid)
                .HasName($"idx_child_type_uuid_{nameof(Link).ToLower()}")
                .IsUnique();
        }
    }
#endif
}
