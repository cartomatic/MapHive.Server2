using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DataModel;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MapHive.Core.DAL.TypeConfiguration
{
    public class TranslationKeyConfiguration : IEntityTypeConfiguration<TranslationKey>
    {
        public void Configure(EntityTypeBuilder<TranslationKey> builder)
        {
            builder.ApplyIBaseConfiguration(nameof(TranslationKey), "translation_keys", "mh_localization");

            builder.Property(en => en.LocalizationClassUuid).HasColumnName("localization_class_uuid");
            builder.Property(en => en.Key).HasColumnName("key");

            //Stuff below would be true if the class derived from ILocalizationConfiguration; this does not seem to work though...
            //and need to set the mapping explicitly. Looks like EF is not always happy with the interfaces.
            //Note: Translations dobe via ILocalizationConfiguration
            builder.Ignore(p => p.Translations);
            builder.Property(p => p.TranslationsSerialized).HasColumnName("translations");


            builder.HasIndex(t => new { t.LocalizationClassUuid, t.Key}) //this should create a unique composite field idx!
                .HasName($"uq_localization_class_translation_key")
                .IsUnique();
        }
    }
}
