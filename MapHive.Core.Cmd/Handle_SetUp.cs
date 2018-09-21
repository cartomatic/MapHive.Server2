using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.CmdPrompt.Core;
using Cartomatic.Utils.Data;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Npgsql;

namespace MapHive.Core.Cmd
{
    public partial class CommandHandler
    {

        /// <summary>
        /// Handles setting up the MapHive environment - maphive meta db, idsrv db and identity db
        /// Registers the default apps, creates a master org
        /// </summary>
        /// <param name="args"></param>
        protected virtual async Task Handle_SetUp(Dictionary<string, string> args)
        {
            var cmd = GetCallerName();
            PrintCommand("mh.core.cmd", cmd);

            if (GetHelp(args))
            {
                Console.WriteLine($"'{cmd}' : sets up the full maphive platform environment - dbs, apps, default master organization.");
                Console.WriteLine();

                await Handle_SetUpDb(args);

                //apps do not require help really, as it just adds default apps

                await Handle_AddMasterOrg(args);
                Console.WriteLine();

                return;
            }

            await Handle_SetUpDb(args);
            await Handle_DefaultLangs(args);
            await Handle_EmailTemplates(args);

            //when ready add apps and a master user
            if (ContainsParam("full", args) || ContainsParam("mh", args))
            {
                Console.WriteLine();
                await Handle_AddApps(new Dictionary<string, string>{{"all", "true"}});
                Console.WriteLine();
                await Handle_AddMasterOrg(new Dictionary<string, string>());
            }

            Console.WriteLine();
        }
        
    }
}
