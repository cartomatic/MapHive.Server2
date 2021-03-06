﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.CmdPrompt.Core;
using Cartomatic.Utils.Data;
using MapHive.Core.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace MapHive.Core.Cmd
{
    public partial class CommandHandler
    {
        /// <summary>
        /// data source credentials
        /// </summary>
        protected Cartomatic.Utils.Data.DataSourceCredentials Dsc { get; set; }

        /// <summary>
        /// Sets default db credentials
        /// </summary>
        protected virtual void SetDefaultDsc()
        {
            try
            {
                var cfg = Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig();

                var dsc = new DataSourceCredentials();
                cfg.GetSection("Dsc").Bind(dsc);

                //if could not read, this should be default
                if(dsc.DataSourceProvider == DataSourceProvider.Unknown)
                    throw new Exception();

                Dsc = dsc;


                PrintDbc();
            }
            catch
            {
                ConsoleEx.WriteLine("Default db credentials are not configured. Is this intentional?", ConsoleColor.DarkRed);
                Console.WriteLine();
            }
        }


        /// <summary>
        /// handles setting db credentials
        /// </summary>
        /// <param name="args"></param>
        protected virtual void Handle_Dsc(Dictionary<string, string> args)
        {
            var cmd = GetCallerName();
            PrintCommand("mh.core.cmd", cmd, args);

            if (GetHelp(args))
            {
                Console.WriteLine($"'{cmd}' : prints or sets db connection details for default connection");
                Console.WriteLine($"syntax: {cmd} space separated params: ");
                Console.WriteLine("\t[h:serverhost]");
                Console.WriteLine("\t[n:servername]");
                Console.WriteLine("\t[p:serverport]");
                Console.WriteLine("\t[d:database]");
                Console.WriteLine("\t[u:user]");
                Console.WriteLine("\t[P:pass]");
                Console.WriteLine();
                Console.WriteLine($"example: {cmd} h:localhost n: p:5432 d:postgres u:postgres P:postgres");
                Console.WriteLine();
                Console.WriteLine("The order of params is not important; required params are: host, port, user, pass;");
                Console.WriteLine("In most cases database is also a required param, although not always ie: dropping a db");
                Console.WriteLine("invalid params will result in errors when trying to connect.");
                Console.WriteLine();

                return;
            }

            if (args.Count > 0)
            {
                var serverhost = ExtractParam("h", args);
                var servername = ExtractParam("n", args);
                var serverport = ExtractParam<int?>("p", args);
                var database = ExtractParam("d", args);
                var user = ExtractParam("u", args);
                var pass = ExtractParam("P", args);

                if (ValidateDbCredentials(serverhost, servername, serverport, database, user, pass))
                {
                    Dsc = new DataSourceCredentials
                    {
                        ServerHost = serverhost,
                        ServerName = servername,
                        ServerPort = serverport,
                        DbName = database,
                        UserName = user,
                        Pass = pass,
                        DataSourceProvider = DataSourceProvider.Npgsql
                    };
                }
                else
                {
                    ConsoleEx.WriteErr($"Invalid database credentials. Type {cmd} help to get more details on the command usage.");
                }

            }

            PrintDbc();

        }

        /// <summary>
        /// Prints currently configured database credentials
        /// </summary>
        protected virtual void PrintDbc()
        {
            Console.WriteLine("Current database credentials:");

            var cl = ConsoleColor.DarkMagenta;

            Console.Write("serverhost:");
            ConsoleEx.Write(Dsc.ServerHost, cl);
            Console.Write(", servername:");
            ConsoleEx.Write(Dsc.ServerName, cl);
            Console.Write(", serverport:");
            ConsoleEx.Write(Dsc.ServerPort.ToString(), cl);
            Console.Write(", database:");
            ConsoleEx.Write(Dsc.DbName, cl);
            Console.Write(", user:");
            ConsoleEx.Write(Dsc.UserName, cl);
            Console.Write(", pass:");
            ConsoleEx.Write(Dsc.Pass, cl);
            Console.Write(Environment.NewLine);

            Console.WriteLine();
        }

        /// <summary>
        /// simplistic datasource validation
        /// </summary>
        /// <param name="serverhost"></param>
        /// <param name="servername"></param>
        /// <param name="serverport"></param>
        /// <param name="database"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        protected virtual bool ValidateDbCredentials(string serverhost, string servername, int? serverport, string database, string user, string pass)
        {
            return !string.IsNullOrEmpty(serverhost) &&
                   serverport.HasValue &&
                   !string.IsNullOrEmpty(user) &&
                   !string.IsNullOrEmpty(pass);
        }

        /// <summary>
        /// Produces a maphive db context with db speciofied via dsc
        /// </summary>
        /// <returns></returns>
        protected MapHiveDbContext GetMapHiveDbContext()
        {
            using (var dbCtx = new MapHiveDbContext())
            {
                return (MapHiveDbContext)dbCtx.ProduceDbContextInstance(Dsc.GetConnectionString(), true,
                    DataSourceProvider.Npgsql);
            }
        }
    }
}
