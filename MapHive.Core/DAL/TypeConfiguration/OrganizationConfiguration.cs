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
    public class OrganizationConfiguration : EntityTypeConfiguration<Organization>
    {
        public OrganizationConfiguration()
        {
            ToTable("organizations", "mh_meta");
            this.ApplyIBaseConfiguration(nameof(Organization));

            Property(p => p.Slug).HasColumnName("slug");
            Property(p => p.DisplayName).HasColumnName("display_name");
            Property(p => p.Description).HasColumnName("description");
            Property(p => p.Location).HasColumnName("location");
            Property(p => p.Url).HasColumnName("url");
            Property(p => p.Email).HasColumnName("email");
            Property(p => p.GravatarEmail).HasColumnName("gravatar_email");
            Ignore(p => p.ProfilePicture);
            Property(p => p.ProfilePictureId).HasColumnName("profile_picture_id");
            Property(p => p.BillingEmail).HasColumnName("billing_email");
            Property(p => p.BillingAddress).HasColumnName("billing_address");
            Property(p => p.BillingExtraInfo.Serialized).HasColumnName("billing_extra_info");

            Property(en => en.Slug)
                .HasColumnAnnotation(
                    "Index",
                    new IndexAnnotation(new IndexAttribute($"idx_slug_{nameof(Organization).ToLower()}") { IsUnique = true }));
        }
    }
#endif

#if NETSTANDARD
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
#endif
}
