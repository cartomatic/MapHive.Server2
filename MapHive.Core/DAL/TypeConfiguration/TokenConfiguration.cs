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
    public class TokenConfiguration : IEntityTypeConfiguration<Token>
    {
        public void Configure(EntityTypeBuilder<Token> builder)
        {
            builder.ApplyIBaseConfiguration(nameof(Token), "tokens", "mh_meta");

            builder.Property(p => p.OrganizationId).HasColumnName("organization_id");
            builder.Ignore(p => p.ApplicationIds);
            builder.Property(p => p.ApplicationIdsSerialized).HasColumnName("application_ids");
            builder.Property(p => p.Name).HasColumnName("name");
            builder.Property(p => p.Description).HasColumnName("description");

            builder.Ignore(p => p.Referrers);
            builder.Property(p => p.ReferrersSerialized).HasColumnName("referrers");

            builder.Property(p => p.CanIgnoreReferrer).HasColumnName("can_ignore_referrer");
            builder.Property(p => p.IsMaster).HasColumnName("is_master");
        }
    }
}

