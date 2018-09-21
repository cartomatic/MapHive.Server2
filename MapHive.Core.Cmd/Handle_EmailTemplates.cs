using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.CmdPrompt.Core;
using MapHive.Core.Configuration;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.Cmd
{
    public partial class CommandHandler
    {
        /// <summary>
        /// Handles adding email templates in the mh env
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual async Task Handle_EmailTemplates(IDictionary<string, string> args)
        {
            var cmd = GetCallerName();
            PrintCommand("mh.core.cmd", cmd, args);

            if (GetHelp(args))
            {
                Console.WriteLine(
                    $"'{cmd}' : registers default emails in the system.");
                Console.WriteLine($"syntax: {cmd}");
                Console.WriteLine();

                return;
            }

            //explicit info on the cmd mode
            PrintRemoteMode();

            await RegisterEmailTemplatesAsync(GetEmailTemplates());
        }

        /// <summary>
        /// Registers email templates
        /// </summary>
        /// <param name="ets"></param>
        /// <returns></returns>
        protected virtual async Task RegisterEmailTemplatesAsync(IEnumerable<EmailTemplateLocalization> ets)
        {
            if (RemoteMode)
            {
                ConsoleEx.Write($"Registering email template: {string.Join(", ", ets.Select(x=>x.Identifier))}... ", ConsoleColor.DarkYellow);
                await RegisterEmailTemplatesRemoteAsync(ets);
                ConsoleEx.Write("Done!" + Environment.NewLine, ConsoleColor.DarkGreen);
            }
            else
            {
                using (var dbCtx = GetMapHiveDbContext())
                {
                    foreach (var et in ets)
                    {
                        ConsoleEx.Write($"Registering email template: {et.Identifier}... ", ConsoleColor.DarkYellow);

                        if (!await dbCtx.EmailTemplates.AnyAsync(t => t.Uuid == et.Uuid))
                        {
                            dbCtx.EmailTemplates.Add(et);
                        }

                        ConsoleEx.Write("Done!" + Environment.NewLine, ConsoleColor.DarkGreen);
                    }
                    await dbCtx.SaveChangesAsync();
                }
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Returns a collection of default email templates
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<EmailTemplateLocalization> GetEmailTemplates()
        {
            return MapHive.Core.Defaults.EmailTemplates.GetEmailTemplates();
        }
    }
}
