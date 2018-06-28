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

    public static partial class EntityTypeConfigurationExtensions
    {

        public static void ApplyMapHiveUserBaseConfiguration<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : MapHiveUserBase
        {
            builder.Property(en => en.Email).HasColumnName("email");
            builder.Property(en => en.IsAccountClosed).HasColumnName("is_account_closed");
            builder.Property(en => en.IsAccountVerified).HasColumnName("is_account_verified");

            builder.HasIndex(p => p.Email)
                .HasName($"uq_email_{nameof(TEntity).ToLower()}")
                .IsUnique();
        }

    }
}
