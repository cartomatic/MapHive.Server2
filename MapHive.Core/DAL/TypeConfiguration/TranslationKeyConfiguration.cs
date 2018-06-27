using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DataModel;

#if NETFULL
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Infrastructure.Annotations;
#endif

#if NETSTANDARD
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
#endif

namespace MapHive.Core.DAL.TypeConfiguration
{
#if NETFULL
    public class TranslationKeyConfiguration : EntityTypeConfiguration<TranslationKey> 
        //Note:
        //Deriving from ILocalisationConfiguration<DataModel.AppLocalisation> does not work. EF needs a concrete type nd throws otherwise
    {
        public TranslationKeyConfiguration()
        {
            ToTable("translation_keys", "mh_localisation");
            this.ApplyIBaseConfiguration(nameof(TranslationKey));

            Property(en => en.LocalizationClassUuid).HasColumnName("localisation_class_uuid");
            Property(en => en.Key).HasColumnName("key");

            //Stuff below would be true if the class derived from ILocalisationConfiguration; this does not seem to work though...
            //and need to set the mapping explicitly. Looks like EF is not always happy with the interfaces.
            //Note: Translations dobe via ILocalisationConfiguration
            Property(p => p.Translations.Serialized).HasColumnName("translations");

            //indexes
            Property(en => en.LocalizationClassUuid)
                .HasColumnAnnotation(
                    "Index",
                    new IndexAnnotation(new IndexAttribute("uq_localisation_class_translation_key") { IsUnique = true, Order = 1 }));
            Property(en => en.Key)
                .HasColumnAnnotation(
                    "Index",
                    new IndexAnnotation(new IndexAttribute("uq_localisation_class_translation_key") { IsUnique = true, Order = 2 }));
        }
        
    }
#endif


#if NETSTANDARD
    public class TranslationKeyConfiguration : IEntityTypeConfiguration<TranslationKey>
    {
        public void Configure(EntityTypeBuilder<TranslationKey> builder)
        {
            builder.ApplyIBaseConfiguration(nameof(TranslationKey), "translation_keys", "mh_localisation");

            builder.Property(en => en.LocalizationClassUuid).HasColumnName("localisation_class_uuid");
            builder.Property(en => en.Key).HasColumnName("key");

            //Stuff below would be true if the class derived from ILocalisationConfiguration; this does not seem to work though...
            //and need to set the mapping explicitly. Looks like EF is not always happy with the interfaces.
            //Note: Translations dobe via ILocalisationConfiguration
            builder.Property(p => p.Translations.Serialized).HasColumnName("translations");


            builder.HasIndex(t => new { t.LocalizationClassUuid, t.Key}) //this should create a unique composite field idx!
                .HasName($"uq_localisation_class_translation_key")
                .IsUnique();
        }
    }
#endif
}
