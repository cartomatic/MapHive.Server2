using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.CmdPrompt.Core;
using Cartomatic.Utils.Data;
using MapHive.Core.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace MapHive.Core.Cmd
{
    public partial class CommandHandler
    {
        /// <summary>
        /// handles setting db credentials
        /// </summary>
        /// <param name="args"></param>
        protected virtual async Task Handle_FindOrg(Dictionary<string, string> args)
        {
            var cmd = GetCallerName();
            PrintCommand("mh.core.cmd", cmd, args);

            if (GetHelp(args))
            {
                Console.WriteLine($"'{cmd}' : finds an org and prints its details");
                Console.WriteLine($"syntax: {cmd} space separated params: ");
                Console.WriteLine("\t[i:identifier");
                Console.WriteLine();
                Console.WriteLine($"example: {cmd} i:some-org");
                Console.WriteLine();

                return;
            }

            var identifier = ExtractParam("i", args);
            if (string.IsNullOrEmpty(identifier))
            {
                ConsoleEx.WriteErr("No org identigier provided");
                Console.WriteLine();
                return;
            }

            Organization org = null;

            if (RemoteMode)
            {
                //grab an org remotely
                org = await GetOrgRemoteAsync(identifier);
            }
            else
            {
                using (var dbCtx = GetMapHiveDbContext())
                {
                    if (Guid.TryParse(identifier, out var orgId))
                    {
                        org = await dbCtx.Organizations.Where(x => x.Uuid == orgId).FirstOrDefaultAsync();
                    }
                    else
                    {
                        org = await dbCtx.Organizations.Where(x => x.Slug == identifier).FirstOrDefaultAsync();
                    }
                }
            }

            PrintOrgInfo(org);
        }

        /// <summary>
        /// Prints orgInfo
        /// </summary>
        protected virtual void PrintOrgInfo(Organization org)
        {
            if (org == null)
            {
                Console.WriteLine("Org not found");
            }
            else
            {
                Console.WriteLine("Org details:");

                var cl = ConsoleColor.DarkMagenta;

                Console.Write("name: ");
                ConsoleEx.Write(org.DisplayName + Environment.NewLine, cl);
                Console.Write("description: ");
                ConsoleEx.Write(org.Description + Environment.NewLine, cl);

                Console.Write("slug: ");
                ConsoleEx.Write(org.Slug + Environment.NewLine, cl);
                Console.Write("id: ");
                ConsoleEx.Write(org.Uuid + Environment.NewLine, cl);
            }

            Console.WriteLine();
        }
    }
}
