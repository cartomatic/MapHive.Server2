using Cartomatic.Utils.Ef;
using MapHive.Core.DAL.TypeConfiguration;
using MapHive.Core.DataModel.Map;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MapHive.Core.DAL.TypeConfiguration
{
    public static class MapDataStoreConfigurationExtensions
    {
        public static void ApplyMapDataStoreBaseConfiguration<TEntity>(this EntityTypeBuilder<TEntity> builder,
            string entityName, string tableName, string schema = null)
            where TEntity : DataStoreBase
        {
            builder.ApplyIBaseConfiguration(entityName, tableName);

            builder.Property(p => p.InUse).HasColumnName("name");
            builder.Property(p => p.InUse).HasColumnName("in_use");

            builder.Property(p => p.DataSourceSerialized).HasColumnName("data_source");
            builder.Ignore(p => p.DataSource);

            builder.Property(p => p.LinkedDataStoreIdsSerialized).HasColumnName("linked_data_stores");
            builder.Ignore(p => p.LinkedDataStoreIds);
            builder.Ignore(p => p.LinkedDataStores);
        }
    }

    public class MapDataStoreConfiguration : IEntityTypeConfiguration<DataStore>
    {
        public void Configure(EntityTypeBuilder<DataStore> builder)
        {
            builder.ApplyMapDataStoreBaseConfiguration(nameof(DataStore), "data_stores");
        }
    }
}

