using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.CmdPrompt.Core;
using MapHive.Core.DAL;
using MapHive.Core.Identity;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.Cmd
{
    public partial class CommandHandler
    {
        /// <summary>
        /// Handles upgrading dbs
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual async Task Handle_UpgradeDbs(IDictionary<string, string> args)
        {
            //so warnings dissapear from async method when not using await
            await Task.Delay(0);

            var cmd = GetCallerName();
            PrintCommand("mh.core.cmd", cmd, args);

            if (GetHelp(args))
            {
                Console.WriteLine(
                    $"'{cmd}' : updates the maphive environment dbs - maphive2_meta, maphive2_idsrv, maphive2_mr; uses the configured db credentials to connect to the db server.");
                Console.WriteLine($"syntax: {cmd} space separated params: ");
                Console.WriteLine("\t[all]; all the maphive databases should be upgraded");
                Console.WriteLine("\t[mh]; maphive2_meta should be upgraded");
                Console.WriteLine("\t[mr]; maphive2_mr (MembershipReboot) should be upgraded");
                Console.WriteLine("\t[idsrv]; maphive2_idsrv (IdentityServer) should be upgraded");

                Console.WriteLine($"example: {cmd} all");
                Console.WriteLine();

                return;
            }

            if (RemoteMode)
            {
                ConsoleEx.WriteLine($"{nameof(Handle_UpgradeDbs)} works only in direct db mode, skipping!", ConsoleColor.DarkGray);
                Console.WriteLine();
                return;
            }

            var ctxsToMigrate = new Dictionary<string, List<Type>>();

            var all = ContainsParam("all", args);

            if (all || ContainsParam("mh", args))
            {
                ctxsToMigrate["maphive2_meta"] = new List<Type> { typeof(MapHiveDbContext) };
            }
            if (all || ContainsParam("id", args))
            {
                ctxsToMigrate["maphive2_identity"] = new List<Type> { typeof(MapHive.Core.Identity.DAL.MapHiveIdentityDbContext) };
            }
            if (all || ContainsParam("idsrv", args))
            {
                ctxsToMigrate["maphive2_idsrv"] = new List<Type>
                {
                    typeof(MapHive.Core.IdentityServer.DAL.MapHiveIdSrvPersistedGrantDbContext),
                    typeof(MapHive.Core.IdentityServer.DAL.MapHiveIdSrvConfigurationDbContext)
                };
            }

            SetupDatabases(null, ctxsToMigrate, false);

            ClearEfConnectionPoolsCache(all || ContainsParam("mh", args), all || ContainsParam("id", args));

            Console.WriteLine();
        }
    }
}
