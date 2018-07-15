using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DAL;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel
{
    public partial class Lang
    {
        private static List<Lang> SupportedLangs { get; set; }

        /// <summary>
        /// Gets a list of supported languages
        /// </summary>
        /// <typeparam name="TDbCtx"></typeparam>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Lang>> GetSupportedLangsAsync(DbContext dbCtx)
        {
            if (!(dbCtx is ILocalizedDbContext iLocalizedDbCtx))
                return SupportedLangs;


            return (SupportedLangs ?? (SupportedLangs = await iLocalizedDbCtx.Langs.ToListAsync()));
        }

        /// <summary>
        /// Gets a list of supported lang codes
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<string>> GetSupportedLangCodesAsync(DbContext dbCtx)
        {
            var langs = await GetSupportedLangsAsync(dbCtx);

            return langs?.Select(l => l.LangCode) ?? new string[0];
        }
    }
}
