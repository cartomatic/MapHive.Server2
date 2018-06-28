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
    public class EmailTemplateLocalizationConfiguration : IEntityTypeConfiguration<EmailTemplateLocalization>
    {
        public void Configure(EntityTypeBuilder<EmailTemplateLocalization> builder)
        {
            builder.ApplyIBaseConfiguration(nameof(EmailTemplateLocalization), "email_templates", "mh_localization");

            builder.Property(en => en.ApplicationName).HasColumnName("application_name");
            builder.Property(en => en.Name).HasColumnName("name");
            builder.Property(en => en.Description).HasColumnName("description");
            builder.Property(en => en.Identifier).HasColumnName("identifier");
            builder.Property(en => en.IsBodyHtml).HasColumnName("is_body_html");

            //Stuff below would be true if the class derived from ILocalizationConfiguration; this does not seem to work though...
            //Note: Translations dobe via ILocalizationConfiguration
            builder.Ignore(p => p.Translations);
            builder.Property(p => p.TranslationsSerialized).HasColumnName("translations");

            builder.HasIndex(t => t.ApplicationName)
                .HasName($"uq_slug_{nameof(Application).ToLower()}")
                .IsUnique();

            builder.HasIndex(t => new { t.ApplicationName, t.Identifier }) //this should create a unique composite field idx!
                .HasName($"uq_localization_class_translation_key")
                .IsUnique();
        }
    }
}
