using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Ef;
using MapHive.Core.DataModel;
using MapHive.Core.DataModel.Dictionary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MapHive.Core.DAL.TypeConfiguration
{
    public static partial class EntityTypeConfigurationExtensions
    {
        /// <summary>
        /// Applies IIdentifierDictionary confoguration
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="builder"></param>
        /// <param name="entityName"></param>
        /// <param name="tableName"></param>
        /// <param name="schema"></param>
        public static void ApplyIIdentifierDictionaryConfiguration<TEntity>(this EntityTypeBuilder<TEntity> builder, string entityName, string tableName, string schema = null)
            where TEntity : class, IBase, IIdentifierDictionary
        {
            builder.ApplyISimpleDictionaryConfiguration(entityName, tableName, schema);

            builder.Property(p => p.Identifier).HasColumnName("identifier");

            builder.HasIndex(p => p.Identifier)
                .HasName($"idx_{entityName.ToColumnName()}_uq_identifier")
                .IsUnique();
        }

        /// <summary>
        /// Applies ISimpleDictionary confoguration
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="builder"></param>
        /// <param name="entityName"></param>
        /// <param name="tableName"></param>
        /// <param name="schema"></param>
        public static void ApplyISimpleDictionaryConfiguration<TEntity>(this EntityTypeBuilder<TEntity> builder, string entityName, string tableName, string schema = null)
            where TEntity : class, IBase, ISimpleDictionary
        {
            builder.ApplyIBaseConfiguration(entityName, tableName, schema);

            builder.Property(p => p.Name).HasColumnName("name");
            builder.Property(p => p.Description).HasColumnName("description");
        }
    }

}
