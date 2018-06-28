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
    public class MapHiveUserConfiguration : IEntityTypeConfiguration<MapHiveUser>
    {
        public void Configure(EntityTypeBuilder<MapHiveUser> builder)
        {
            builder.ApplyIBaseConfiguration(nameof(MapHiveUser), "users", "mh_meta");

            builder.ApplyMapHiveUserBaseConfiguration();

            builder.Property(p => p.Forename).HasColumnName("forename");
            builder.Property(p => p.Surname).HasColumnName("surname");
            builder.Property(p => p.ContactPhone).HasColumnName("contact_phone");
            builder.Property(p => p.Slug).HasColumnName("slug");
            builder.Property(p => p.Bio).HasColumnName("bio");
            builder.Property(p => p.Company).HasColumnName("company");
            builder.Property(p => p.Department).HasColumnName("department");
            builder.Property(p => p.Location).HasColumnName("location");
            builder.Property(p => p.GravatarEmail).HasColumnName("gravatar_email");

            builder.Ignore(p => p.ProfilePicture);
            builder.Property(p => p.ProfilePictureId).HasColumnName("profile_picture_id");

            builder.Property(p => p.IsOrgUser).HasColumnName("is_org_user");
            builder.Property(p => p.UserOrgId).HasColumnName("user_org_id");
            builder.Property(p => p.VisibleInCatalogue).HasColumnName("visible_in_catalogue");
            builder.Property(p => p.ParentOrganizationId).HasColumnName("parent_org_id");

            builder.Ignore(p => p.OrganizationRole);

            builder.HasIndex(t => t.Slug)
                .HasName($"uq_slug_{nameof(MapHiveUser).ToLower()}")
                .IsUnique();
        }
    }
}
