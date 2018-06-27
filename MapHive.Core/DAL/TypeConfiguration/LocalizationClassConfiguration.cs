using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DataModel;

#if NETFULL
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
    public class LocalizationClassConfiguration : EntityTypeConfiguration<LocalizationClass> 
        //Note:
        //Deriving from ILocalizationConfiguration<DataModel.AppLocalization> does not work. EF needs a concrete type nd throws otherwise
    {
        public LocalizationClassConfiguration()
        {
            ToTable("localization_classes", "mh_localization");
            this.ApplyIBaseConfiguration(nameof(LocalizationClass));

            Property(en => en.ApplicationName).HasColumnName("application_name");
            Property(en => en.ClassName).HasColumnName("class_name");
            Property(en => en.InheritedClassName).HasColumnName("inherited_class_name");

            Ignore(p => p.TranslationKeys);

            //indexes
            Property(en => en.ApplicationName)
                .HasColumnAnnotation(
                    "Index",
                    new IndexAnnotation(new IndexAttribute("uq_app_name_class_name") { IsUnique = true, Order = 1 }));
            Property(en => en.ClassName)
                .HasColumnAnnotation(
                    "Index",
                    new IndexAnnotation(new IndexAttribute("uq_app_name_class_name") { IsUnique = true, Order = 2 }));
        }
        
    }
#endif


#if NETSTANDARD
    public class LocalizationClassConfiguration : IEntityTypeConfiguration<LocalizationClass>
    {
        public void Configure(EntityTypeBuilder<LocalizationClass> builder)
        {
            builder.ApplyIBaseConfiguration(nameof(LocalizationClass), "localization_classes", "mh_localization");

            builder.Property(en => en.ApplicationName).HasColumnName("application_name");
            builder.Property(en => en.ClassName).HasColumnName("class_name");
            builder.Property(en => en.InheritedClassName).HasColumnName("inherited_class_name");

            builder.Ignore(p => p.TranslationKeys);

            builder.HasIndex(t => new { t.ApplicationName, t.ClassName }) //this should create a unique composite field idx!
                .HasName($"uq_localization_class_translation_key")
                .IsUnique();
        }
    }
#endif
}
