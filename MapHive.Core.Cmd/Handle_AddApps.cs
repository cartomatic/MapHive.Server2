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
                    $"'{cmd}' : registers default apps in the system OR adds apps to an organization.");
                Console.WriteLine($"syntax: {cmd} space separated params: ");
                Console.WriteLine("\t[a:{string}; comma separated app names]");
                Console.WriteLine("\t[all; adds all apps]");
                Console.WriteLine("\t[list; lists apps that can be added via this command]");
                Console.WriteLine("\t[o:{slug or id}; an organization to register the applications with]");

                Console.WriteLine($"example: {cmd} a:hgis,hive o:some-org-slug");
                Console.WriteLine();

                return;
            }

            var apps = GetApps();

            if (ContainsParam("list", args))
            {
                Console.WriteLine("The following apps are available:");
                foreach (var app in apps)
                {
                    ConsoleEx.Write($"{app.ShortName}:", ConsoleColor.DarkRed); 
                    ConsoleEx.Write($" {app.Name}; {app.Description}" + Environment.NewLine, ConsoleColor.DarkYellow);
                }
                Console.WriteLine();
                return;
            }

            //print remote mode, so it is explicitly communicated
            PrintRemoteMode();


            var appsToAdd = (ExtractParam<string>("a", args) ?? "").Split(',');

            if (ContainsParam("all", args))
            {
                appsToAdd = apps.Select(a => a.ShortName).ToArray();
            }

            //just a regular register apps call
            await RegisterAppsAsync(appsToAdd);


            var orgIdentifier = ExtractParam<string>("o", args);
            if (!string.IsNullOrEmpty(orgIdentifier))
            {
                ConsoleEx.Write($"Getting an org: {orgIdentifier}... ", ConsoleColor.DarkYellow);

                Organization org = null;
                if (RemoteMode)
                {
                    //grab an org remotely
                    org = await GetOrgRemoteAsync(orgIdentifier);
                }
                else
                {
                    using (var dbCtx = GetMapHiveDbContext())
                    {
                        if (Guid.TryParse(orgIdentifier, out var orgId))
                        {
                            org = await dbCtx.Organizations.AsNoTracking().Where(x => x.Uuid == orgId).FirstOrDefaultAsync();
                        }
                        else
                        {
                            org = await dbCtx.Organizations.AsNoTracking().Where(x => x.Slug == orgIdentifier).FirstOrDefaultAsync();
                        }
                    }
                }

                ConsoleEx.WriteOk("Done!" + Environment.NewLine);

                if (org != null)
                {
                    await RegisterAppsWithOrgAsync(org, appsToAdd);
                }
                else
                {
                    ConsoleEx.WriteErr($"Organization could not be found: {orgIdentifier}.");
                }
            }
           
            Console.WriteLine();
        }


        /// <summary>
        /// Registers apps with an organization - adds the apps to an organization as links
        /// </summary>
        /// <param name="org"></param>
        /// <param name="appsToAdd"></param>
        /// <returns></returns>
        protected async Task RegisterAppsWithOrgAsync(Organization org, string[] appsToAdd)
        {
            if (org == null)
            {
                ConsoleEx.WriteErr("Cannot add apps to org - organization missing!");
                return;
            }

            if (appsToAdd.Length == 0)
            {
                ConsoleEx.WriteErr("Cannot add apps to org - apps not specified!");
                return;
            }

            var apps = GetApps().Where(a => appsToAdd.Contains(a.ShortName));

            ConsoleEx.Write(
                $"Registering apps: {string.Join(",", apps.Select(a => a.ShortName))} with an org: {org.DisplayName} ({org.Slug})... ",
                ConsoleColor.DarkYellow);

            if (RemoteMode)
            {
                await RegisterAppsWithOrgRemoteAsync(org, apps);
            }
            else
            {
                foreach (var a in apps)
                {
                    org.AddLink(a);
                }

                using (var dbCtx = GetMapHiveDbContext())
                {
                    await org.UpdateAsync(dbCtx);
                }
            }

            ConsoleEx.WriteOk("Done!" + Environment.NewLine);
        }

        /// <summary>
        /// Adds apps by a key
        /// </summary>
        /// <param name="appsToAdd"></param>
        /// <returns></returns>
        protected async Task RegisterAppsAsync(string[] appsToAdd)
        {
            using (var dbCtx = GetMapHiveDbContext())
            {
                var apps = GetApps();

                foreach (var appToAdd in appsToAdd)
                {
                    var app = apps.FirstOrDefault(a => a.ShortName == appToAdd);

                    if (app != null)
                    {
                        ConsoleEx.Write($"Registering {app.ShortName} app... ", ConsoleColor.DarkYellow);

                        if (RemoteMode)
                        {
                            await RegisterAppsRemoteAsync(app);
                        }
                        else
                        {
                            if (!await dbCtx.Applications.AnyAsync(a => a.Uuid == app.Uuid))
                            {
                                dbCtx.Applications.Add(app);
                            }
                        }

                        ConsoleEx.Write("Done!" + Environment.NewLine, ConsoleColor.DarkGreen);
                    }
                    else
                    {
                        ConsoleEx.WriteErr($"Unrecognised app: {appToAdd}. Ignoring!");
                    }
                }

                await dbCtx.SaveChangesAsync();
            }
        }

        /// <summary>
        /// gets apps that can be added by this cmd
        /// </summary>
        /// <returns></returns>
        protected virtual List<Application> GetApps()
        {
            return new List<Application>
            {
                new Application
                {
                    Uuid = Guid.Parse("5f541902-4f42-4a58-8dee-523ea02cd1fd"),
                    ShortName = "hive",
                    Name = "The Hive",
                    Description = "Hive @ MapHive",
                    Urls = "https://maphive.local/|https://maphive.net/|https://hive.maphive.local/|https://hive.maphive.net/",
                    IsCommon = true,
                    IsHive = true
                },

                //home app when there is no org context
                new Application
                {
                    Uuid = Guid.Parse("eea081b5-6a3d-4a11-87e8-55dbe042322c"),
                    ShortName = "home",
                    Name = "Home",
                    IsHome = true,
                    IsCommon = true,
                    Urls = "https://home.maphive.local/|https://home.maphive.net/"
                },
                //dashboard app when there is org context, but no app specified
                new Application
                {
                    Uuid = Guid.Parse("fe6801c4-c9cb-4b86-9416-a143b355deab"),
                    ShortName = "dashboard",
                    Name = "Dashboard",
                    IsDefault = true,
                    IsCommon = true,
                    Urls = "https://dashboard.maphive.local/|https://dashboard.maphive.net/",
                    RequiresAuth = true
                },
                new Application
                {
                    Uuid = Guid.Parse("30aca350-41a4-4906-be82-da1247537f19"),
                    ShortName = "hgis1",
                    Name = "HGIS v1",
                    Description = "Cartomatic\'s HGIS",
                    Urls = "https://hgisold.maphive.local/|https://hgisold.maphive.net/",
                    IsCommon = true
                },
                new Application
                {
                    Uuid = Guid.Parse("27321a8a-aa7d-47fd-a539-761b248ef248"),
                    ShortName = "hgis2",
                    Name = "HGIS v2",
                    Description = "Cartomatic\'s HGIS",
                    Urls = "https://hgis.maphive.local/|https://hgis.maphive.net/",
                    IsCommon = true
                },
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
                },
                
            };
        }

    }
}