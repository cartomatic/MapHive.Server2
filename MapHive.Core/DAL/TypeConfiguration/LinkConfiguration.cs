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
                .HasName($"idx_{nameof(Link).ToColumnName()}_parent_uuid");

            builder.HasIndex(t => t.ChildUuid)
                .HasName($"idx_{nameof(Link).ToColumnName()}_child_uuid");

            builder.HasIndex(t => t.ParentTypeUuid)
                .HasName($"idx_{nameof(Link).ToColumnName()}_parent_type_uuid");

            builder.HasIndex(t => t.ChildTypeUuid)
                .HasName($"idx_{nameof(Link).ToColumnName()}_child_type_uuid");
        }
    }
}
