using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.CmdPrompt.Core;

namespace MapHive.Core.Cmd
{
    public partial class CommandHandler : Cartomatic.CmdPrompt.Core.DefaultCmdCommandHandler
    {
        public CommandHandler(string handlerInfo)
            : base(handlerInfo)
        {
            //register some xtra commands
            SetUpCommandMap(
                new CommandMap()
                    .AddAliases(nameof(Handle_SetUp), "s")
                    .AddAliases(nameof(Handle_Dsc), "conn")
                    .AddAliases(nameof(Handle_DestroyUser), "xuser", "xu")
                    .AddAliases(nameof(Handle_AddApps), "apps")
                    .AddAliases(nameof(Handle_AddUser), "u")
                    .AddAliases(nameof(Handle_AddMasterOrg), "addmorg")
                    .AddAliases(nameof(Handle_MasterOrg), "morg")
                    .AddAliases(nameof(Handle_FindOrg), "getorg")
                    .AddAliases(nameof(Handle_Endpoints), "ep")
                    .AddAliases(nameof(Handle_RemoteMode), "rm")
                    .AddAliases(nameof(Handle_RemoteAdmin), "ra")
                    .AddAliases(nameof(Handle_EmailTemplates), "et")
                    .AddAliases(nameof(Handle_DefaultLangs), "dl")
            );

            PrintHandlerInfo();

            //default db credentials
            SetDefaultDsc();

            SetDefaultMasterOrg();

            SetDefaultEndpoints();

            SetDefaultRemoteAdmin();

            PrintRemoteMode();
        }
        
        public CommandHandler()
            : this("MapHive2 CMD v1.0....")
        {
            Cartomatic.Utils.Identity.ImpersonateGhostUser();
        }

        protected void PrintCommand(string cmdType, string commandName, IDictionary<string, string> args)
        {
            var prms = args.Select(kv => $"{kv.Key}" + (string.IsNullOrEmpty(kv.Value) ? "" : $":{kv.Value}"));

            ConsoleEx.Write($"{cmdType}: ", ConsoleColor.DarkCyan);
            ConsoleEx.Write($"{commandName} {string.Join(" ", prms)}", ConsoleColor.DarkGray);
            Console.WriteLine();
        }
    }
}
