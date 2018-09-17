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
        /// <summary>
        /// Whether or not the cmd works in remote mode
        /// </summary>
        protected bool RemoteMode = true;

        
        /// <summary>
        /// handles setting db credentials
        /// </summary>
        /// <param name="args"></param>
        protected virtual void Handle_RemoteMode(Dictionary<string, string> args)
        {
            var cmd = GetCallerName();

            if (GetHelp(args))
            {
                Console.WriteLine($"'{cmd}' : prints or sets remote mode status");
                Console.WriteLine($"syntax: {cmd} space separated params: ");
                Console.WriteLine("\t[t:bool presence] / [f:bool presence] - either true or false");
                Console.WriteLine();
                Console.WriteLine($"example: {cmd} t");
                Console.WriteLine();

                return;
            }

            if (args.Count > 0)
            {
                var t = ContainsParam("t", args);
                var f = ContainsParam("f", args);

                if (t)
                    RemoteMode = true;
                if (f)
                    RemoteMode = false;

            }

            PrintRemoteMode();
        }

        /// <summary>
        /// Prints currently configured database credentials
        /// </summary>
        protected virtual void PrintRemoteMode()
        {
            var cl = ConsoleColor.DarkMagenta;

            Console.Write("CMD mode: ");
            ConsoleEx.Write(RemoteMode ? "remote api" : "direct db" + Environment.NewLine, cl);

            Console.WriteLine();
        }
    }
}
