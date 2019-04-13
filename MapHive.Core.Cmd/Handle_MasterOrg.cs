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
        protected internal virtual string MasterOrgId { get; set; }
        /// <summary>
        /// Master org name
        /// </summary>
        protected internal virtual string MasterOrgName { get; set; }

        /// <summary>
        /// Master org description
        /// </summary>
        protected internal virtual string MasterOrgDesc { get; set; }
        /// <summary>
        /// Master org email
        /// </summary>
        protected internal virtual string MasterOrgEmail { get; set; }

        /// <summary>
        /// Master org pass
        /// </summary>
        protected internal virtual string MasterOrgPass { get; set; }

        /// <summary>
        /// Sets default db credentials
        /// </summary>
        protected virtual void SetDefaultMasterOrg()
        {
            try
            {
                var cfg = Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig();

                var morg = cfg.GetSection("MasterOrg").Get<Dictionary<string, string>>();

                if(morg == null)
                    morg = new Dictionary<string, string>();

                if (
                    !morg.ContainsKey("Email") || string.IsNullOrEmpty(morg["Email"]) || 
                    !morg.ContainsKey("Password") || string.IsNullOrEmpty(morg["Password"]) ||
                    !morg.ContainsKey("Name") || string.IsNullOrEmpty(morg["Name"]) ||
                    !morg.ContainsKey("Description") || string.IsNullOrEmpty(morg["Description"])
                )
                    throw new Exception();

                MasterOrgId = morg["Id"];
                MasterOrgName = morg["Name"];
                MasterOrgDesc = morg["Description"];
                MasterOrgEmail = morg["Email"];
                MasterOrgPass = morg["Password"];

                PrintMorg();
            }
            catch
            {
                ConsoleEx.WriteLine("Default master org is not configured. Is this intentional?", ConsoleColor.DarkRed);
                Console.WriteLine();
            }
        }


        /// <summary>
        /// handles setting db credentials
        /// </summary>
        /// <param name="args"></param>
        protected virtual void Handle_MasterOrg(Dictionary<string, string> args)
        {
            var cmd = GetCallerName();
            PrintCommand("mh.core.cmd", cmd, args);

            if (GetHelp(args))
            {
                Console.WriteLine($"'{cmd}' : prints or sets remote admin details for default connection");
                Console.WriteLine($"syntax: {cmd} space separated params: ");
                Console.WriteLine("\t[e:email");
                Console.WriteLine("\t[p:pass]");
                Console.WriteLine("\t[n:name]");
                Console.WriteLine("\t[d:description]");
                Console.WriteLine();
                Console.WriteLine($"example: {cmd} n:{MasterOrgName} d:{MasterOrgDesc} e:{MasterOrgEmail} p:{MasterOrgPass}");
                Console.WriteLine();
                Console.WriteLine("The order of params is not important; it is possible to change any number of params at once");
                Console.WriteLine();

                return;
            }

            if (args.Count > 0)
            {
                var email = ExtractParam("e", args);
                var pass = ExtractParam("p", args);
                var name = ExtractParam("n", args);
                var desc = ExtractParam("d", args);
                var oid = ExtractParam("oid", args);


                if (!string.IsNullOrEmpty(email))
                    MasterOrgEmail = email;

                if (!string.IsNullOrEmpty(pass))
                    MasterOrgPass = pass;

                if (!string.IsNullOrEmpty(name))
                    MasterOrgName = name;

                if (!string.IsNullOrEmpty(desc))
                    MasterOrgDesc = desc;

                if (!string.IsNullOrEmpty(oid))
                    MasterOrgId = oid;

            }

            PrintMorg();
        }

        /// <summary>
        /// Prints currently configured database credentials
        /// </summary>
        protected virtual void PrintMorg()
        {
            Console.WriteLine("Master org:");

            var cl = ConsoleColor.DarkMagenta;

            Console.Write("id: ");
            ConsoleEx.Write(MasterOrgId + Environment.NewLine, cl);

            Console.Write("name: ");
            ConsoleEx.Write(MasterOrgName + Environment.NewLine, cl);
            Console.Write("description: ");
            ConsoleEx.Write(MasterOrgDesc + Environment.NewLine, cl);

            Console.Write("email: ");
            ConsoleEx.Write(MasterOrgEmail + Environment.NewLine, cl);
            Console.Write("password: ");
            ConsoleEx.Write(MasterOrgPass + Environment.NewLine, cl);

            Console.WriteLine();
        }
    }
}
