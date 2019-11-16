using Cartomatic.Utils.Ef;
using MapHive.Core.DAL.TypeConfiguration;
using MapHive.Core.DataModel.Map;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MapHive.Core.DAL.TypeConfiguration
{
    public class DataStoreConfiguration : IEntityTypeConfiguration<DataStore>
    {
        public void Configure(EntityTypeBuilder<DataStore> builder)
        {
            builder.ApplyIBaseConfiguration(nameof(DataStore), "data_stores");

            builder.Property(p => p.InUse).HasColumnName("name");
            builder.Property(p => p.InUse).HasColumnName("in_use");

            builder.Property(p => p.DataSourceSerialized).HasColumnName("data_source");
            builder.Ignore(p => p.DataSource);

            builder.Property(p => p.LinkedDataStoreIdsSerialized).HasColumnName("linked_data_stores");
            builder.Ignore(p => p.LinkedDataStoreIds);
            builder.Ignore(p => p.LinkedDataStores);
        }
    }
}

