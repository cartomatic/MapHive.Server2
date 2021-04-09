using Cartomatic.Utils.Ef;
using MapHive.Core.DAL.TypeConfiguration;
using MapHive.Core.DataModel.Map;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MapHive.Core.DAL.TypeConfiguration.Map
{
    public static class MapConfigurationExtensions
    {
        public static void ApplyMapBaseConfiguration<TEntity>(this EntityTypeBuilder<TEntity> builder, string entityName, string tableName, string schema = null)
            where TEntity : MapBase
        {
            builder.ApplyIBaseConfiguration(entityName, tableName);

            builder.Property(p => p.Permalink).HasColumnName("permalink");

            builder.Property(p => p.Name).HasColumnName("name");
            builder.Property(p => p.Description).HasColumnName("description");

            builder.Property(p => p.InitLo).HasColumnName("init_lo");
            builder.Property(p => p.InitLa).HasColumnName("init_la");

            builder.Property(p => p.InitZoom).HasColumnName("init_zoom");

            builder.Property(p => p.BaseLayer).HasColumnName("base_layer");

            builder.Ignore(p => p.Layers);

            builder.HasIndex(p => p.Permalink)
                .HasName($"idx_{entityName.ToColumnName()}");
        }

    }

    public class MapConfiguration : IEntityTypeConfiguration<DataModel.Map.Map>
    {
        public void Configure(EntityTypeBuilder<DataModel.Map.Map> builder)
        {
            builder.ApplyMapBaseConfiguration(nameof(DataModel.Map.Map), "maps");
        }
    }

}

