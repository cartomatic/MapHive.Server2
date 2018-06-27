using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DataModel;

#if NETFULL
using System.Data.Entity;
#endif
#if NETSTANDARD
using Microsoft.EntityFrameworkCore;
#endif

namespace MapHive.Core.DAL
{
    /// <summary>
    /// Interface for a DbContext that provides access to localization data
    /// </summary>
    public interface ILocalizedDbContext
    {
        DbSet<LocalizationClass> LocalizationClasses { get; set; }
        DbSet<TranslationKey> TranslationKeys { get; set; }
        DbSet<EmailTemplateLocalization> EmailTemplates { get; set; }
        DbSet<Lang> Langs { get; set; }
    }
}
