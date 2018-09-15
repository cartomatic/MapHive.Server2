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
        protected Dictionary<string, string> RemoteAdmin { get; set; }

        /// <summary>
        /// Sets default db credentials
        /// </summary>
        protected virtual void SetDefaultRemoteAdmin()
        {
            try
            {
                var cfg = Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig();

                RemoteAdmin = cfg.GetSection("RemoteAdminConfig:Credentials").Get<Dictionary<string, string>>();

                if(RemoteAdmin == null)
                    RemoteAdmin = new Dictionary<string, string>();

                if (!RemoteAdmin.ContainsKey("Email") || string.IsNullOrEmpty(RemoteAdmin["Email"]) || !RemoteAdmin.ContainsKey("Password") || string.IsNullOrEmpty(RemoteAdmin["Password"]))
                    throw new Exception();

                PrintRemoteAdmin();
            }
            catch
            {
                ConsoleEx.WriteLine("Default remote admin is not configured. Is this intentional?", ConsoleColor.DarkRed);
                Console.WriteLine();
            }
        }


        /// <summary>
        /// handles setting db credentials
        /// </summary>
        /// <param name="args"></param>
        protected virtual void Handle_RemoteAdmin(Dictionary<string, string> args)
        {
            var cmd = GetCallerName();

            if (GetHelp(args))
            {
                Console.WriteLine($"'{cmd}' : prints or sets remote admin details for default connection");
                Console.WriteLine($"syntax: {cmd} space separated params: ");
                Console.WriteLine("\t[e:email");
                Console.WriteLine("\t[p:pass]");
                Console.WriteLine();
                Console.WriteLine($"example: {cmd} e:queen@maphive.net p:test");
                Console.WriteLine();
                Console.WriteLine("The order of params is not important; it is possible to change any number of params at once");
                Console.WriteLine();

                return;
            }

            if (args.Count > 0)
            {
                var email = ExtractParam("e", args);
                var pass = ExtractParam("p", args);

                if (!string.IsNullOrEmpty(email))
                    RemoteAdmin["Email"] = email;

                if (!string.IsNullOrEmpty(pass))
                    RemoteAdmin["Password"] = pass;

            }

            PrintRemoteAdmin();
        }

        /// <summary>
        /// Prints currently configured database credentials
        /// </summary>
        protected virtual void PrintRemoteAdmin()
        {
            Console.WriteLine("Remote admin:");

            var cl = ConsoleColor.DarkMagenta;

            Console.Write("email: ");
            ConsoleEx.Write(RemoteAdmin["Email"] + Environment.NewLine, cl);
            Console.Write("password: ");
            ConsoleEx.Write(RemoteAdmin["Password"] + Environment.NewLine, cl);
            Console.Write(Environment.NewLine);

            Console.WriteLine();
        }
    }
}
