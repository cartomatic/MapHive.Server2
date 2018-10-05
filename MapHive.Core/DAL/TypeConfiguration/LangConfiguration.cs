using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DataModel;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MapHive.Core.DAL.TypeConfiguration
{
    public class LangConfiguration : IEntityTypeConfiguration<Lang>
    {
        public void Configure(EntityTypeBuilder<Lang> builder)
        {
            builder.ApplyIBaseConfiguration(nameof(Lang), "langs", "mh_localization");

            builder.Property(p => p.LangCode).HasColumnName("lang_code");
            builder.Property(p => p.Name).HasColumnName("name");
            builder.Property(p => p.Description).HasColumnName("description");
            builder.Property(p => p.IsDefault).HasColumnName("is_default");

        }
    }
}
