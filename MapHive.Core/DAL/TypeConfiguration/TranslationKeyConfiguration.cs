using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Ef;
using MapHive.Core.DataModel;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MapHive.Core.DAL.TypeConfiguration
{
    public static class TranslationKeyConfigurationExtensions
    {
        public static void ApplyTranslationKeyBaseConfiguration<TEntity>(this EntityTypeBuilder<TEntity> builder, string entityName, string tableName, string schema = null)
            where TEntity : TranslationKeyBase
        {
            builder.ApplyIBaseConfiguration(entityName, tableName, schema);

            builder.Property(p => p.LocalizationClassUuid).HasColumnName("localization_class_uuid");

            builder.Property(p => p.Key).HasColumnName("key");

            builder.Property(p => p.Inherited).HasColumnName("inherited");
            builder.Property(p => p.Overwrites).HasColumnName("overwrites");

            //Stuff below would be true if the class derived from ILocalizationConfiguration; this does not seem to work though...
            //and need to set the mapping explicitly. Looks like EF is not always happy with the interfaces.
            //Note: Translations dobe via ILocalizationConfiguration
            builder.Ignore(p => p.Translations);
            builder.Property(p => p.TranslationsSerialized).HasColumnName("translations");


            builder.HasIndex(t => new { t.LocalizationClassUuid, t.Key }) //this should create a unique composite field idx!
                .HasName($"idx_{nameof(TranslationKey).ToColumnName()}_uq_localization_class_translation_key")
                .IsUnique();
        }
    }

    public class TranslationKeyConfiguration : IEntityTypeConfiguration<TranslationKey>
    {
        public void Configure(EntityTypeBuilder<TranslationKey> builder)
        {
            builder.ApplyTranslationKeyBaseConfiguration(nameof(TranslationKey), "translation_keys", "mh_localization");
        }
    }

    public class TranslationKeyExtendedConfiguration : IEntityTypeConfiguration<TranslationKeyExtended>
    {
        public void Configure(EntityTypeBuilder<TranslationKeyExtended> builder)
        {
            builder.ApplyTranslationKeyBaseConfiguration(nameof(TranslationKeyExtended), TranslationKeyExtended.ViewName, "mh_localization");

            builder.Property(p => p.ApplicationName).HasColumnName("application_name");
            builder.Property(p => p.ClassName).HasColumnName("class_name");
            builder.Property(p => p.InheritedClassName).HasColumnName("inherited_class_name");
            builder.Property(p => p.FullKey).HasColumnName("full_key");

        }
    }
}
