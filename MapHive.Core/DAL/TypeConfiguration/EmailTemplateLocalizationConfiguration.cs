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
    public class EmailTemplateLocalizationConfiguration : IEntityTypeConfiguration<EmailTemplateLocalization>
    {
        public void Configure(EntityTypeBuilder<EmailTemplateLocalization> builder)
        {
            builder.ApplyIBaseConfiguration(nameof(EmailTemplateLocalization), "email_templates", "mh_localization");

            builder.Property(p => p.ApplicationIdentifier).HasColumnName("application_identifier");
            builder.Property(p => p.Name).HasColumnName("name");
            builder.Property(p => p.Description).HasColumnName("description");
            builder.Property(p => p.Identifier).HasColumnName("identifier");
            builder.Property(p => p.IsBodyHtml).HasColumnName("is_body_html");

            //Stuff below would be true if the class derived from ILocalizationConfiguration; this does not seem to work though...
            //Note: Translations dobe via ILocalizationConfiguration
            builder.Ignore(p => p.Translations);
            builder.Property(p => p.TranslationsSerialized).HasColumnName("translations");


            builder.HasIndex(t => t.ApplicationIdentifier)
                .HasName($"idx_{nameof(EmailTemplateLocalization).ToColumnName()}_app_identifier");

            builder.HasIndex(t => new { ApplicationName = t.ApplicationIdentifier, t.Identifier }) //this should create a unique composite field idx!
                .HasName($"idx_{nameof(EmailTemplateLocalization).ToColumnName()}_uq_app_name_translation_identifier")
                .IsUnique();
        }
    }
}
