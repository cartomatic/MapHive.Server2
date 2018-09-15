using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.CmdPrompt.Core;
using Cartomatic.Utils.Data;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace MapHive.Core.Cmd
{
    public partial class CommandHandler
    {
        protected Dictionary<string, string> Endpoints { get; set; }

        /// <summary>
        /// Sets default db credentials
        /// </summary>
        protected virtual void SetDefaultEndpoints()
        {
            try
            {
                var cfg = Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig();

                Endpoints = cfg.GetSection("RemoteAdminConfig:Endpoints").Get<Dictionary<string, string>>();

                if(Endpoints == null)
                    Endpoints = new Dictionary<string, string>();

                if (!Endpoints.ContainsKey("Core") || string.IsNullOrEmpty(Endpoints["Core"]) || !Endpoints.ContainsKey("Auth") || string.IsNullOrEmpty(Endpoints["Auth"]))
                    throw new Exception();

                PrintEndpoints();
            }
            catch
            {
                ConsoleEx.WriteLine("Default endpoints are not configured. Is this intentional?", ConsoleColor.DarkRed);
                Console.WriteLine();
            }
        }


        /// <summary>
        /// handles setting db credentials
        /// </summary>
        /// <param name="args"></param>
        protected virtual void Handle_Endpoints(Dictionary<string, string> args)
        {
            var cmd = GetCallerName();

            if (GetHelp(args))
            {
                Console.WriteLine($"'{cmd}' : prints or sets endpoints details for default connection");
                Console.WriteLine($"syntax: {cmd} space separated params: ");
                Console.WriteLine("\t[a:auth endpoint");
                Console.WriteLine("\t[c:core endpoint]");
                Console.WriteLine("\t[l:localization endpoint]");
                Console.WriteLine();
                Console.WriteLine($"example: {cmd} c:https://coreapi.maphive.net");
                Console.WriteLine();
                Console.WriteLine("The order of params is not important; it is possible to change any number of endpoints at once");
                Console.WriteLine();

                return;
            }

            if (args.Count > 0)
            {
                var core = ExtractParam("c", args);
                var auth = ExtractParam("a", args);
                var localization = ExtractParam("l", args);

                if (!string.IsNullOrEmpty(core))
                    Endpoints["Core"] = core;

                if (!string.IsNullOrEmpty(auth))
                    Endpoints["Auth"] = auth;

                if (!string.IsNullOrEmpty(localization))
                    Endpoints["Localization"] = localization;

            }

            PrintEndpoints();
        }

        /// <summary>
        /// Prints currently configured database credentials
        /// </summary>
        protected virtual void PrintEndpoints()
        {
            Console.WriteLine("Current endpoints:");

            var cl = ConsoleColor.DarkMagenta;

            Console.Write("core: ");
            ConsoleEx.Write(Endpoints["Core"] + Environment.NewLine, cl);
            Console.Write("auth: ");
            ConsoleEx.Write(Endpoints["Auth"] + Environment.NewLine, cl);
            Console.Write("localization: ");
            ConsoleEx.Write(Endpoints["Localization"] + Environment.NewLine, cl);
            Console.Write(Environment.NewLine);

            Console.WriteLine();
        }
    }
}
