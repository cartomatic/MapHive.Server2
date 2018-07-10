using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.CmdPrompt.Core;
using MapHive.Core.DAL;
using MapHive.Identity;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Cmd.Core
{
    public partial class CommandHandler
    {
        protected virtual async Task Handle_UpgradeDbs(IDictionary<string, string> args)
        {
            //so warnings dissapear from async method when not using await
            await Task.Delay(0);

            var cmd = GetCallerName();

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

            var ctxsToMigrate = new Dictionary<string, List<Type>>();

            var all = ContainsParam("all", args);

            if (all || ContainsParam("mh", args))
            {
                ctxsToMigrate["maphive2_meta"] = new List<Type> { typeof(MapHiveDbContext) };
            }
            if (all || ContainsParam("id", args))
            {
                ctxsToMigrate["maphive2_identity"] = new List<Type> { typeof(MapHive.Identity.DAL.MapHiveIdentityDbContext) };
            }
            if (all || ContainsParam("idsrv", args))
            {
                ctxsToMigrate["maphive2_idsrv"] = new List<Type>
                {
                    typeof(MapHive.IdentityServer.DAL.MapHiveIdSrvPersistedGrantDbContext),
                    typeof(MapHive.IdentityServer.DAL.MapHiveIdSrvConfigurationDbContext)
                };
            }

            SetupDatabases(null, ctxsToMigrate, false);

            ClearEfConnectionPoolsCache(all || ContainsParam("mh", args), all || ContainsParam("id", args));

            Console.WriteLine();
        }
    }
}
