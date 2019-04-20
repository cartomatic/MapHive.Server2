using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Ef;
using MapHive.Core.DataModel;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MapHive.Core.DAL.TypeConfiguration
{

    public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {
            builder.ApplyIBaseConfiguration(nameof(Application), "applications", "mh_meta");

            builder.Property(p => p.ShortName).HasColumnName("short_name");
            builder.Property(p => p.Name).HasColumnName("name");
            builder.Property(p => p.Description).HasColumnName("description");
            builder.Property(p => p.Urls).HasColumnName("urls");
            builder.Property(p => p.UseSplashscreen).HasColumnName("use_splashscreen");
            builder.Property(p => p.RequiresAuth).HasColumnName("requires_auth");
            builder.Property(p => p.IsCommon).HasColumnName("is_common");
            builder.Property(p => p.IsDefault).HasColumnName("is_default");
            builder.Property(p => p.IsHome).HasColumnName("is_home");
            builder.Property(p => p.IsHive).HasColumnName("is_hive");
            builder.Property(p => p.IsApi).HasColumnName("is_api");
            builder.Property(p => p.ProviderId).HasColumnName("provider_id");

            builder.Ignore(p => p.LicenseOptions);
            builder.Property(p => p.LicenseOptionsSerialized).HasColumnName("license_options");

            builder.Ignore(p => p.OrgUserAppAccessCredentials);

            builder.HasIndex(p => p.ShortName)
                .HasName($"idx_{nameof(Application).ToColumnName()}_uq_short_name")
                .IsUnique();
        }
    }
}

