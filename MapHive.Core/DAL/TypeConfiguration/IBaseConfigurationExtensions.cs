﻿using System;
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

    }

}
