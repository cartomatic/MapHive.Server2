using System;
using System.Collections.Generic;
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
    public class LangConfiguration : EntityTypeConfiguration<Lang> 
        //Note:
        //Deriving from ILocalizationConfiguration<DataModel.AppLocalization> does not work. EF needs a concrete type nd throws otherwise
    {
        public LangConfiguration()
        {
            ToTable("langs", "mh_localization");
            this.ApplyIBaseConfiguration(nameof(Lang));

            Property(en => en.LangCode).HasColumnName("lang_code");
            Property(en => en.Name).HasColumnName("name");
            Property(en => en.Description).HasColumnName("description");
            Property(en => en.IsDefault).HasColumnName("is_default");
        }
    }
#endif


#if NETSTANDARD
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
#endif
}
