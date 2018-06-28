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
            builder.Property(p => p.BillingExtraInfo.Serialized).HasColumnName("billing_extra_info");

            builder.HasIndex(t => t.Slug)
                .HasName($"uq_slug_{nameof(Organization).ToLower()}")
                .IsUnique();
        }
    }
}
