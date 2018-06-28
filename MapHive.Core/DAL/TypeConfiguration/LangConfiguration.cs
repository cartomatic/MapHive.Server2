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

            builder.Property(en => en.LangCode).HasColumnName("lang_code");
            builder.Property(en => en.Name).HasColumnName("name");
            builder.Property(en => en.Description).HasColumnName("description");
            builder.Property(en => en.IsDefault).HasColumnName("is_default");

        }
    }
}
