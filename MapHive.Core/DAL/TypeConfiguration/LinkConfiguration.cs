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
            //links take the default off the model builder, so can be reused in other schemas too!
            builder.ToTable(name: "links");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("id");

            builder.Property(p => p.ParentUuid).HasColumnName("parent_uuid");
            builder.Property(p => p.ChildUuid).HasColumnName("child_uuid");
            builder.Property(p => p.ParentTypeUuid).HasColumnName("parent_type_uuid");
            builder.Property(p => p.ChildTypeUuid).HasColumnName("child_type_uuid");
            builder.Property(p => p.SortOrder).HasColumnName("sort_order");

            builder.Ignore(p => p.LinkData);
            builder.Property(p => p.LinkDataSerialized).HasColumnName("link_data");


            builder.HasIndex(t => t.ParentUuid)
                .HasName($"idx_parent_uuid_{nameof(Link).ToLower()}");

            builder.HasIndex(t => t.ChildUuid)
                .HasName($"idx_child_uuid_{nameof(Link).ToLower()}");

            builder.HasIndex(t => t.ParentTypeUuid)
                .HasName($"idx_parent_type_uuid_{nameof(Link).ToLower()}");

            builder.HasIndex(t => t.ChildTypeUuid)
                .HasName($"idx_child_type_uuid_{nameof(Link).ToLower()}");
        }
    }
}
