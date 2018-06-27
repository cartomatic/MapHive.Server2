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
        /// <summary>
        /// Takes care of setting up type configuration specific to the IBase model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="entityName">Name of the entity. used for automated index naming</param>
        /// <returns></returns>
        public static EntityTypeConfiguration<T> ApplyIBaseConfiguration<T>(this EntityTypeConfiguration<T> entity, string entityName) where T : class, IBase
        {
            entity.HasKey(t => t.Uuid);

            entity.Property(en => en.Uuid).HasColumnName("uuid");
            entity.Property(en => en.CreatedBy).HasColumnName("created_by");
            entity.Property(en => en.LastModifiedBy).HasColumnName("last_modified_by");
            entity.Property(en => en.CreateDateUtc).HasColumnName("create_date_utc");
            entity.Property(en => en.ModifyDateUtc).HasColumnName("modify_date_utc");
            entity.Property(en => en.EndDateUtc).HasColumnName("end_date_utc");

            entity.Ignore(p => p.TypeUuid);
            entity.Ignore(p => p.Links);
            entity.Ignore(p => p.LinkData);

            entity.Property(en => en.CreateDateUtc)
                .HasColumnAnnotation(
                    "Index",
                    new IndexAnnotation(new IndexAttribute($"idx_create_date_{entityName.ToLower()}")));

            return entity;
        }
#endif

#if NETSTANDARD
        /// <summary>
        /// Takes care of setting up type configuration specific to the IBase model
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="builder"></param>
        /// <param name="entityName"></param>
        /// <param name="tableName"></param>
        /// <param name="schema"></param>
        public static void ApplyIBaseConfiguration<TEntity>(this EntityTypeBuilder<TEntity> builder, string entityName, string tableName, string schema)
            where TEntity : class, IBase
        {
            builder.ToTable(name: tableName, schema: schema);

            builder.HasKey(t => t.Uuid);

            builder.Property(en => en.Uuid).HasColumnName("uuid");
            builder.Property(en => en.CreatedBy).HasColumnName("created_by");
            builder.Property(en => en.LastModifiedBy).HasColumnName("last_modified_by");
            builder.Property(en => en.CreateDateUtc).HasColumnName("create_date_utc");
            builder.Property(en => en.ModifyDateUtc).HasColumnName("modify_date_utc");
            builder.Property(en => en.EndDateUtc).HasColumnName("end_date_utc");

            builder.Ignore(p => p.TypeUuid);
            builder.Ignore(p => p.Links);
            builder.Ignore(p => p.LinkData);

            builder.HasIndex(p => p.CreateDateUtc)
                .HasName($"idx_create_date_{entityName.ToLower()}");
        }
#endif

    }

}
