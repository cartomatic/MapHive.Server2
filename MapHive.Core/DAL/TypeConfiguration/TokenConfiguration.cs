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

            builder.Property(en => en.OrganizationId).HasColumnName("organization_id");
            builder.Property(en => en.ApplicationId).HasColumnName("application_id");
            builder.Property(en => en.Name).HasColumnName("name");
            builder.Property(en => en.Description).HasColumnName("description");
            builder.Property(en => en.Referrers.Serialized).HasColumnName("referrers");
            builder.Property(en => en.CanIgnoreReferrer).HasColumnName("can_ignore_referrer");
            builder.Property(en => en.IsMaster).HasColumnName("is_master");
        }
    }
}

