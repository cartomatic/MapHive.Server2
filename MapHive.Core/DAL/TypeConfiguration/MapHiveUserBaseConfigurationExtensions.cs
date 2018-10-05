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
            builder.Property(p => p.Email).HasColumnName("email");
            builder.Property(p => p.IsAccountClosed).HasColumnName("is_account_closed");
            builder.Property(p => p.IsAccountVerified).HasColumnName("is_account_verified");

            builder.HasIndex(p => p.Email)
                .HasName($"uq_email_{typeof(TEntity).Name.ToLower()}")
                .IsUnique();
        }

    }
}
