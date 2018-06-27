using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DataModel;

#if NETFULL
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;
#endif

#if NETSTANDARD
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
#endif

namespace MapHive.Core.DAL.TypeConfiguration
{
#if NETFULL
    public class EmailTemplateLocalizationConfiguration : EntityTypeConfiguration<EmailTemplateLocalization>
    //Note:
    //Deriving from ILocalizationConfiguration<DataModel.AppLocalization> does not work. EF needs a concrete type nd throws otherwise
    {
        public EmailTemplateLocalizationConfiguration()
        {
            ToTable("email_templates", "mh_localization");
            this.ApplyIBaseConfiguration(nameof(EmailTemplateLocalization));

            Property(en => en.ApplicationName).HasColumnName("application_name");
            Property(en => en.Name).HasColumnName("name");
            Property(en => en.Description).HasColumnName("description");
            Property(en => en.Identifier).HasColumnName("identifier");
            Property(en => en.IsBodyHtml).HasColumnName("is_body_html");

            //Stuff below would be true if the class derived from ILocalizationConfiguration; this does not seem to work though...
            //Note: Translations dobe via ILocalizationConfiguration
            Property(p => p.Translations.Serialized).HasColumnName("translations");


            Property(t => t.ApplicationName).HasColumnAnnotation(
                "Index", new IndexAnnotation(new IndexAttribute("idx_application")));

            //make a compond unique key 


            Property(t => t.ApplicationName)
                .HasColumnAnnotation(
                    "Index",
                    new IndexAnnotation(new IndexAttribute("uq_app_name_and_identifier") { IsUnique = true, Order = 1 }));
            Property(t => t.Identifier)
                .HasColumnAnnotation(
                    "Index",
                    new IndexAnnotation(new IndexAttribute("uq_app_name_and_identifier") { IsUnique = true, Order = 2 }));
        }
    }
#endif


#if NETSTANDARD
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
            builder.Property(p => p.Translations.Serialized).HasColumnName("translations");

            builder.HasIndex(t => t.ApplicationName)
                .HasName($"uq_slug_{nameof(Application).ToLower()}")
                .IsUnique();

            builder.HasIndex(t => new { t.ApplicationName, t.Identifier }) //this should create a unique composite field idx!
                .HasName($"uq_localization_class_translation_key")
                .IsUnique();
        }
    }
#endif
}
