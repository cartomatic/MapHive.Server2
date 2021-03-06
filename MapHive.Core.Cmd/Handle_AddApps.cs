﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.CmdPrompt.Core;
using Cartomatic.Utils.Data;
using Cartomatic.Utils.Dto;
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
            PrintCommand("mh.core.cmd", cmd, args);

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

            var reload = ContainsParam("reload", args);

            var appsToAdd = (ExtractParam<string>("a", args) ?? "").Split(',');

            if (ContainsParam("all", args))
            {
                appsToAdd = apps.Select(a => a.ShortName).ToArray();
            }

            //just a regular register apps call
            await RegisterAppsAsync(appsToAdd, reload);


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

            ConsoleEx.WriteLine(
                $"Registering apps: {string.Join(", ", apps.Select(a => a.ShortName))} with an org: {org.DisplayName} ({org.Slug})... ",
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

            ConsoleEx.WriteOk($"{string.Join(", ", apps.Select(a => a.ShortName))} registered!" + Environment.NewLine);
        }

        /// <summary>
        /// Adds apps by a key
        /// </summary>
        /// <param name="appsToAdd"></param>
        /// <returns></returns>
        protected async Task RegisterAppsAsync(string[] appsToAdd, bool reload)
        {
            using (var dbCtx = GetMapHiveDbContext())
            {
                var apps = GetApps();

                ConsoleEx.WriteLine($"Verifying apps presence: {string.Join(", ", appsToAdd)}... ", ConsoleColor.DarkYellow);

                foreach (var appToAdd in appsToAdd)
                {
                    var app = apps.FirstOrDefault(a => a.ShortName == appToAdd);

                    if (app != null)
                    {
                        if (RemoteMode)
                        {
                            if (!await IsAppRegisteredRemoteAsync(app.ShortName) || reload)
                            {
                                ConsoleEx.WriteLine($"Registering {app.ShortName} app... ", ConsoleColor.DarkYellow);
                                await RegisterAppsRemoteAsync(app); //Note: backend does UPSERT already, no reload param is actually required
                                ConsoleEx.WriteLine($"{app.ShortName} app registered!" + Environment.NewLine, ConsoleColor.DarkGreen);
                            }
                        }
                        else
                        {
                            if (!await dbCtx.Applications.AnyAsync(a => a.Uuid == app.Uuid) || reload)
                            {
                                ConsoleEx.Write($"Registering {app.ShortName} app... ", ConsoleColor.DarkYellow);
                                if (reload && await dbCtx.Applications.AnyAsync(a => a.Uuid == app.Uuid))
                                {
                                    var currentApp = await dbCtx.Applications.Where(a => a.Uuid == app.Uuid)
                                        .FirstOrDefaultAsync();
                                    currentApp.CopyPublicPropertiesFrom(app);
                                }
                                else
                                {
                                    dbCtx.Applications.Add(app);
                                }
                                
                                ConsoleEx.Write("Done!" + Environment.NewLine, ConsoleColor.DarkGreen);
                            }
                        }
                    }
                    else
                    {
                        ConsoleEx.WriteErr($"Unrecognised app: {appToAdd}. Ignoring!");
                    }
                }

                ConsoleEx.WriteLine("App's presence verified!", ConsoleColor.DarkGreen);
                Console.WriteLine();

                await dbCtx.SaveChangesAsync();
            }
        }

        /// <summary>
        /// gets apps that can be added by this cmd
        /// </summary>
        /// <returns></returns>
        protected virtual List<Application> GetApps()
        {
            return MapHive.Core.Defaults.Applications.GetApplications();
        }
    }
}