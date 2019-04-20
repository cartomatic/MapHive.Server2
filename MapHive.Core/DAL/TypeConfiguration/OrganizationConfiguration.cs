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
    public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
    {
        public void Configure(EntityTypeBuilder<Organization> builder)
        {
            builder.ApplyIBaseConfiguration(nameof(Organization), "organizations", "mh_meta");

            builder.Property(p => p.Slug).HasColumnName("slug");
            builder.Property(p => p.DisplayName).HasColumnName("display_name");
            builder.Property(p => p.Description).HasColumnName("description");
            builder.Property(p => p.Location).HasColumnName("location");
            builder.Property(p => p.Url).HasColumnName("url");
            builder.Property(p => p.Email).HasColumnName("email");
            builder.Property(p => p.GravatarEmail).HasColumnName("gravatar_email");
            builder.Ignore(p => p.ProfilePicture);
            builder.Property(p => p.ProfilePictureId).HasColumnName("profile_picture_id");
            builder.Property(p => p.BillingEmail).HasColumnName("billing_email");
            builder.Property(p => p.BillingAddress).HasColumnName("billing_address");

            builder.Ignore(p => p.BillingExtraInfo);
            builder.Property(p => p.BillingExtraInfoSerialized).HasColumnName("billing_extra_info");

            builder.Ignore(p => p.LicenseOptions);
            builder.Property(p => p.LicenseOptionsSerialized).HasColumnName("license_options");

            builder.Ignore(p => p.Admins);
            builder.Ignore(p => p.Owners);
            builder.Ignore(p => p.Roles);
            builder.Ignore(p => p.Users);
            builder.Ignore(p => p.Applications);
            builder.Ignore(p => p.Databases);
            builder.Ignore(p => p.EncryptedDatabases);

            builder.HasIndex(t => t.Slug)
                .HasName($"idx_{nameof(Organization).ToColumnName()}_uq_slug")
                .IsUnique();
        }
    }
}
