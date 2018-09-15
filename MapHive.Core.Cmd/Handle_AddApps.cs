using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.CmdPrompt.Core;
using Cartomatic.Utils.Data;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.Cmd
{

    public partial class CommandHandler
    {
        /// <summary>
        /// Handles adding apps to mh env
        /// </summary>
        /// <param name="args"></param>
        protected virtual async Task Handle_AddApps(IDictionary<string, string> args)
        {
            var cmd = GetCallerName();

            if (GetHelp(args))
            {
                Console.WriteLine(
                    $"'{cmd}' : registers default apps in the system.");
                Console.WriteLine($"syntax: {cmd} space separated params: ");
                Console.WriteLine("\t[a:{string}; comma separated app names]");
                Console.WriteLine("\t[all; adds all apps]");
                Console.WriteLine("\t[list; lists apps that can be added via this command]");

                Console.WriteLine($"example: {cmd} a:hgis,hive");
                Console.WriteLine();

                return;
            }

            var apps = GetApps();

            if (ContainsParam("list", args))
            {
                Console.WriteLine("The following apps are available:");
                foreach (var app in apps.Keys)
                {
                    ConsoleEx.Write($"{app}:", ConsoleColor.DarkRed); 
                    ConsoleEx.Write($" {apps[app].Name}; {apps[app].Description}" + Environment.NewLine, ConsoleColor.DarkYellow);
                }
                Console.WriteLine();
                return;
            }

            var appsToAdd = (ExtractParam<string>("a", args) ?? "").Split(',');

            if (ContainsParam("all", args))
            {
                appsToAdd = apps.Keys.ToArray();
            }

            await RegisterAppsAsync(appsToAdd);

            Console.WriteLine();
        }

        /// <summary>
        /// Adds apps by a key
        /// </summary>
        /// <param name="appsToAdd"></param>
        /// <returns></returns>
        protected async Task RegisterAppsAsync(string[] appsToAdd)
        {
            var dbCtx = new MapHiveDbContext();

            var apps = GetApps();

            foreach (var app in appsToAdd)
            {
                if (apps.ContainsKey(app))
                {
                    ConsoleEx.Write($"Registering {app} app... ", ConsoleColor.DarkYellow);

                    if (!await dbCtx.Applications.AnyAsync(a => a.Uuid == apps[app].Uuid))
                    {
                        try
                        {
                            dbCtx.Applications.Add(apps[app]);
                        }
                        catch (Exception ex)
                        {
                            bool stop = true;
                        }
                    }

                    ConsoleEx.Write("Done!" + Environment.NewLine, ConsoleColor.DarkGreen);
                }
                else
                {
                    ConsoleEx.WriteErr($"Unrecognised app: {app}. Ignoring!");
                }
            }

            await dbCtx.SaveChangesAsync();
        }

        /// <summary>
        /// gets apps that can be added by this cmd
        /// </summary>
        /// <returns></returns>
        protected virtual Dictionary<string, Application> GetApps()
        {
            return new Dictionary<string, Application>
            {
                {
                    "hive",
                    new Application
                    {
                        Uuid = Guid.Parse("5f541902-4f42-4a58-8dee-523ea02cd1fd"),
                        ShortName = "hive",
                        Name = "The Hive",
                        Description = "Hive @ MapHive",
                        Urls = "https://maphive.local/|https://maphive.net/|https://hive.maphive.local/|https://hive.maphive.net/",
                        IsCommon = true,
                        IsHive = true
                    }
                },

                //home app when there is no org context
                {
                    "home",
                    new Application
                    {
                        Uuid = Guid.Parse("eea081b5-6a3d-4a11-87e8-55dbe042322c"),
                        ShortName = "home",
                        Name = "Home",
                        IsHome = true,
                        IsCommon = true,
                        Urls = "https://home.maphive.local/|https://home.maphive.net/"
                    }
                },
                //dashboard app when there is org context, but no app specified
                {
                    "dashboard",
                    new Application
                    {
                        Uuid = Guid.Parse("fe6801c4-c9cb-4b86-9416-a143b355deab"),
                        ShortName = "dashboard",
                        Name = "Dashboard",
                        IsDefault = true,
                        IsCommon = true,
                        Urls = "https://dashboard.maphive.local/|https://dashboard.maphive.net/",
                        RequiresAuth = true
                    }
                },
                {
                    "hgis1",
                    new Application
                    {
                        Uuid = Guid.Parse("30aca350-41a4-4906-be82-da1247537f19"),
                        ShortName = "hgis1",
                        Name = "HGIS v1",
                        Description = "Cartomatic\'s HGIS",
                        Urls = "https://hgisold.maphive.local/|https://hgisold.maphive.net/",
                        IsCommon = true
                    }
                },
                {
                    "hgis2",
                    new Application
                    {
                        Uuid = Guid.Parse("27321a8a-aa7d-47fd-a539-761b248ef248"),
                        ShortName = "hgis2",
                        Name = "HGIS v2",
                        Description = "Cartomatic\'s HGIS",
                        Urls = "https://hgis.maphive.local/|https://hgis.maphive.net/",
                        IsCommon = true
                    }
                },
                {
                    "masterofpuppets",
                    new Application
                    {
                        Uuid = Guid.Parse("1e025446-1a25-4639-a302-9ce0e2017a59"),
                        //no short name, so can test uuid in the url part!
                        Name = "MapHive SiteAdmin",
                        ShortName = "masterofpuppets",

                        IsCommon = false,
                        Description = "MapHive platform Admin app",
                        Urls = "https://masterofpuppets.maphive.local/|https://masterofpuppets.maphive.net/",
                        RequiresAuth = true
                    }
                },
                
            };
        }

    }
}