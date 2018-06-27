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

    public static partial class EntityTypeConfigurationExtensions
    {

#if NETFULL
        public static EntityTypeConfiguration<T> ApplyMapHiveUserBaseConfiguration<T>(this EntityTypeConfiguration<T> entity) where T : MapHiveUserBase
        {
            entity.Property(en => en.Email).HasColumnName("email");
            entity.Property(en => en.IsAccountClosed).HasColumnName("is_account_closed");
            entity.Property(en => en.IsAccountVerified).HasColumnName("is_account_verified");

            entity.Property(en => en.Email)
                .HasColumnAnnotation(
                    "Index",
                    new IndexAnnotation(new IndexAttribute($"uq_email_{nameof(T).ToLower()}")));

            return entity;
        }
#endif

#if NETSTANDARD
        public static void ApplyMapHiveUserBaseConfiguration<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : MapHiveUserBase
        {
            builder.Property(en => en.Email).HasColumnName("email");
            builder.Property(en => en.IsAccountClosed).HasColumnName("is_account_closed");
            builder.Property(en => en.IsAccountVerified).HasColumnName("is_account_verified");

            builder.HasIndex(p => p.Email)
                .HasName($"uq_email_{nameof(TEntity).ToLower()}")
                .IsUnique();
        }
#endif

    }
}
