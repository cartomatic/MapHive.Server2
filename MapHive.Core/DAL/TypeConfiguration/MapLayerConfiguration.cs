using Cartomatic.Utils.Ef;
using MapHive.Core.DAL.TypeConfiguration;
using MapHive.Core.DataModel.Map;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MapHive.Core.DAL.TypeConfiguration
{
    public static class MapLayerConfigurationExtensions
    {
        public static void ApplyMapLayerBaseConfiguration<TEntity>(this EntityTypeBuilder<TEntity> builder, string entityName, string tableName, string schema = null)
            where TEntity : LayerBase
        {
            builder.ApplyIBaseConfiguration(entityName, tableName);

            builder.Property(p => p.Identifier).HasColumnName("identifier");

            builder.Property(p => p.IsDefault).HasColumnName("is_default");
            builder.Property(p => p.Type).HasColumnName("type").HasConversion<int>();

            builder.Property(p => p.SourceLayerId).HasColumnName("source_layer_id");

            builder.Property(p => p.Order).HasColumnName("order");

            builder.Property(p => p.Name).HasColumnName("name");
            builder.Property(p => p.Description).HasColumnName("description");

            builder.Property(p => p.Visible).HasColumnName("visible");

            builder.Property(p => p.VisibilityScaleMin).HasColumnName("visibility_scale_min");
            builder.Property(p => p.VisibilityScaleMax).HasColumnName("visibility_scale_max");

            builder.Property(p => p.DataStoreId).HasColumnName("data_store_id");

            //ignore convenience properties
            builder.Ignore(p => p.SourceLayer);
        }

        public static void ApplyMapLayerBaseExtendedConfiguration<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : LayerBase
        {
            builder.Property(p => p.MetadataSerialized).HasColumnName("metadata");
            builder.Ignore(p => p.Metadata);

            builder.Property(p => p.DataSourceSerialized).HasColumnName("data_source");
            builder.Ignore(p => p.DataSource);

            builder.Property(p => p.StylesSerialized).HasColumnName("styles");
            builder.Ignore(p => p.Styles);

            builder.Property(p => p.WidgetsSerialized).HasColumnName("widgets");
            builder.Ignore(p => p.Widgets);
        }
    }

    public class MapLayerConfiguration : IEntityTypeConfiguration<Layer>
    {
        public void Configure(EntityTypeBuilder<Layer> builder)
        {
            builder.ApplyMapLayerBaseConfiguration(nameof(Layer), "layers");
            builder.ApplyMapLayerBaseExtendedConfiguration();
        }
    }

    public class MapLayerTruncatedConfiguration : IEntityTypeConfiguration<LayerTruncated>
    {
        public void Configure(EntityTypeBuilder<LayerTruncated> builder)
        {
            builder.ApplyMapLayerBaseConfiguration(nameof(LayerTruncated), LayerTruncated.ViewName);
        }
    }
}

