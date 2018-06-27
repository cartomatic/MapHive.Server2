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
    public class ApplicationConfiguration : EntityTypeConfiguration<Application>
    {
        public ApplicationConfiguration()
        {
            ToTable("applications", "mh_meta");
            this.ApplyIBaseConfiguration(nameof(Application));

            Property(en => en.ShortName).HasColumnName("short_name");
            Property(en => en.Name).HasColumnName("name");
            Property(en => en.Description).HasColumnName("description");
            Property(en => en.Urls).HasColumnName("urls");
            Property(en => en.UseSplashscreen).HasColumnName("use_splashscreen");
            Property(en => en.RequiresAuth).HasColumnName("requires_auth");
            Property(en => en.IsCommon).HasColumnName("is_common");
            Property(en => en.IsDefault).HasColumnName("is_default");
            Property(en => en.IsHome).HasColumnName("is_home");
            Property(en => en.IsHive).HasColumnName("is_hive");
            Property(en => en.ProviderId).HasColumnName("provider_id");

            Property(t => t.ShortName)
                .HasColumnAnnotation(
                    "Index",
                    new IndexAnnotation(new IndexAttribute("uq_short_name") { IsUnique = true }));
        }
    }
#endif


#if NETSTANDARD
    public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {
            builder.ApplyIBaseConfiguration(nameof(Application), "applications", "mh_meta");

            builder.Property(en => en.ShortName).HasColumnName("short_name");
            builder.Property(en => en.Name).HasColumnName("name");
            builder.Property(en => en.Description).HasColumnName("description");
            builder.Property(en => en.Urls).HasColumnName("urls");
            builder.Property(en => en.UseSplashscreen).HasColumnName("use_splashscreen");
            builder.Property(en => en.RequiresAuth).HasColumnName("requires_auth");
            builder.Property(en => en.IsCommon).HasColumnName("is_common");
            builder.Property(en => en.IsDefault).HasColumnName("is_default");
            builder.Property(en => en.IsHome).HasColumnName("is_home");
            builder.Property(en => en.IsHive).HasColumnName("is_hive");
            builder.Property(en => en.ProviderId).HasColumnName("provider_id");

            builder.HasIndex(t => t.ShortName)
                .HasName($"uq_slug_{nameof(Application).ToLower()}")
                .IsUnique();
        }
    }
#endif
}

