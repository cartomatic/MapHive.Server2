using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.CmdPrompt.Core;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.Cmd
{
    public partial class CommandHandler
    {
        protected virtual async Task Handle_DefaultLangs(IDictionary<string, string> args)
        {
            var cmd = GetCallerName();

            if (GetHelp(args))
            {
                Console.WriteLine(
                    $"'{cmd}' : registers default langs in the system.");
                Console.WriteLine($"syntax: {cmd}");
                Console.WriteLine();

                return;
            }

            await RegisterLangsAsync(GetDefaultLangs());
        }

        /// <summary>
        /// Registers languages
        /// </summary>
        /// <param name="langs"></param>
        /// <returns></returns>
        protected virtual async Task RegisterLangsAsync(IEnumerable<Lang> langs)
        {
            using (var dbCtx = GetMapHiveDbContext())
            {
                foreach (var lang in langs)
                {
                    ConsoleEx.Write($"Registering lang: {lang.LangCode}... ", ConsoleColor.DarkYellow);

                    if (!await dbCtx.Langs.AnyAsync(l => l.Uuid == lang.Uuid))
                    {
                        dbCtx.Langs.Add(lang);
                    }

                    ConsoleEx.Write("Done!" + Environment.NewLine, ConsoleColor.DarkGreen);
                }
                await dbCtx.SaveChangesAsync();
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Returns a collection of default langs
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<Lang> GetDefaultLangs()
        {
            return new List<Lang>
            {
                new Lang
                {
                    Uuid = Guid.Parse("8323d1bb-e6f5-49d3-a441-837017d6e97e"),
                    LangCode = "en",
                    Name = "English",
                    IsDefault = true
                },
                new Lang
                {
                    Uuid = Guid.Parse("ece753c3-f079-4772-8aa2-0960aeabc94d"),
                    LangCode = "pl",
                    Name = "Polski"
                }
            };
        }
    }
}
