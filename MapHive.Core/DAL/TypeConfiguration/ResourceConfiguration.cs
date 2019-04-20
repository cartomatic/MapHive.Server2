using Cartomatic.Utils.Ef;
using MapHive.Core.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MapHive.Core.DAL.TypeConfiguration
{
    public class ResourceConfiguration : IEntityTypeConfiguration<Resource>
    {
        public void Configure(EntityTypeBuilder<Resource> builder)
        {
            builder.ApplyIBaseConfiguration(nameof(Resource), "resources");

            builder.Property(p => p.OwnerId).HasColumnName("owner_id");
            builder.Property(p => p.OwnerTypeId).HasColumnName("owner_type_id");
            builder.Property(p => p.Identifier).HasColumnName("identifier");
            builder.Property(p => p.OriginalFileName).HasColumnName("original_file_name");
            builder.Property(p => p.Mime).HasColumnName("mime");
            builder.Property(p => p.Data).HasColumnName("data");

            builder.HasIndex(p => p.OwnerId)
                .HasName($"idx_{nameof(Resource).ToColumnName()}_owner_id");
        }
    }
}

