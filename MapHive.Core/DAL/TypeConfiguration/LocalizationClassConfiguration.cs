using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DataModel;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MapHive.Core.DAL.TypeConfiguration
{
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
}
