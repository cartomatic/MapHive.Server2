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

            builder.Ignore(p => p.LinkData);
            builder.Property(p => p.LinkDataSerialized).HasColumnName("link_data");


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
}
