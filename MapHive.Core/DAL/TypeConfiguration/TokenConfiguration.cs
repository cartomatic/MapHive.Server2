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
    public class TokenConfiguration : EntityTypeConfiguration<Token>
    {
        public TokenConfiguration()
        {
            ToTable("tokens", "mh_meta");
            this.ApplyIBaseConfiguration(nameof(Token));

            Property(en => en.OrganizationId).HasColumnName("organization_id");
            Property(en => en.ApplicationId).HasColumnName("application_id");
            Property(en => en.Name).HasColumnName("name");
            Property(en => en.Description).HasColumnName("description");
            Property(en => en.Referrers.Serialized).HasColumnName("referrers");
            Property(en => en.CanIgnoreReferrer).HasColumnName("can_ignore_referrer");
            Property(en => en.IsMaster).HasColumnName("is_master");
        }
    }
#endif


#if NETSTANDARD
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
#endif
}

