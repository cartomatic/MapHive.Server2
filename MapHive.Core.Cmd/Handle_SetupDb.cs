using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.CmdPrompt.Core;
using Cartomatic.Utils.Data;
using Cartomatic.Utils.Ef;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.Cmd
{

    public partial class CommandHandler
    {

        /// <summary>
        /// Handles setting up the MapHive environment - maphive meta db, idsrv db and membership reboot db
        /// </summary>
        /// <param name="args"></param>
        protected virtual async Task Handle_SetUpDb(IDictionary<string, string> args)
        {
            var cmd = GetCallerName();
            PrintCommand("mh.core.cmd", cmd, args);

            if (GetHelp(args))
            {
                Console.WriteLine(
                    $"'{cmd}' : sets up the maphive environment databases - maphive2_meta, maphive2_idsrv, maphive2_identity; uses the configured db credentials to connect to the db server.");
                Console.WriteLine("Most often used for keeping local dev dbs intact (maphive_meta_db) so generating migrations is possible; can update any db though as configured via dsc.");
                Console.WriteLine($"syntax: {cmd} space separated params: ");
                Console.WriteLine("\t[full]; all the maphive databases should be created/ugraded");
                Console.WriteLine("\t[xfull]; all the maphive databases should be dropped prior to being recreated");
                Console.WriteLine("\t[mh]; maphive2_meta should be created/ugraded");
                Console.WriteLine("\t[id]; maphive2_identity (ASPNET Identity) should be created/ugraded");
                Console.WriteLine("\t[idsrv]; maphive2_idsrv (IdentityServer) should be created/ugraded");
                Console.WriteLine("\t[xmh]; maphive2_meta should be dropped prior to being recreated");
                Console.WriteLine("\t[xid]; maphive2_mr (ASPNET Identity) should be dropped prior to being recreated");
                Console.WriteLine("\t[xidsrv]; maphive2_idsrv (IdentityServer) should be dropped prior to being recreated");
                Console.WriteLine("\t[clean:bool]; when dropping mh db, org dbs should also be dropped; defaults to true;");

                Console.WriteLine($"example: {cmd} m mr idsrv xm xmr xidsrv");
                Console.WriteLine();

                return;
            }

            if (RemoteMode)
            {
                ConsoleEx.WriteLine($"{nameof(Handle_SetUpDb)} works only in direct db mode, skipping!", ConsoleColor.DarkGray);
                Console.WriteLine();
                return;
            }

            var dbsToDrop = new List<string>();
            var ctxsToMigrate = new Dictionary<string, List<Type>>();

            var full = ContainsParam("full", args);
            var xfull = ContainsParam("xfull", args);

            if (full || ContainsParam("mh", args))
            {
                ctxsToMigrate["maphive2_meta"] = new List<Type> {typeof(MapHiveDbContext)};
            }
            if (xfull || ContainsParam("xmh", args))
            {
                dbsToDrop.Add("maphive2_meta");
            }
            if (full || ContainsParam("id", args))
            {
                ctxsToMigrate["maphive2_identity"] = new List<Type> { typeof(MapHive.Core.Identity.DAL.MapHiveIdentityDbContext)};
            }
            if (xfull || ContainsParam("xid", args))
            {
                dbsToDrop.Add("maphive2_identity");
            }
            if (full || ContainsParam("maphive2_idsrv", args))
            {

                ctxsToMigrate["maphive2_idsrv"] = new List<Type>
                {
                    typeof(MapHive.Core.IdentityServer.DAL.MapHiveIdSrvPersistedGrantDbContext),
                    typeof(MapHive.Core.IdentityServer.DAL.MapHiveIdSrvConfigurationDbContext)
                };
            }
            if (xfull || ContainsParam("xidsrv", args))
            {
                dbsToDrop.Add("maphive2_idsrv");
            }

            var clean = !ContainsParam("clean", args) || ExtractParam<bool>("clean", args);


            if (dbsToDrop.Count == 0 && ctxsToMigrate.Count == 0)
            {
                ConsoleEx.WriteLine(
                    "Looks like i have nothing to do... Type 'setup help' for more details on how to use this command.",
                    ConsoleColor.DarkYellow);
            }
            else
            {
                if ((xfull || ContainsParam("xmh", args)) && clean)
                {
                    
                    try
                    {
                        //FIXME
                        //this will use the default dsc. perhaps should make it dynamic, huh...
                        //currently can create a db remotely, but will not obtain proper data of it...
                        //get all the orgs
                        using (var dbCtx = GetMapHiveDbContext())
                        {
                            var orgs = await dbCtx.Organizations.ToListAsync();

                            if (orgs.Count > 0)
                            {
                                ConsoleEx.WriteLine("Wiping out org databases...", ConsoleColor.DarkYellow);

                                foreach (var org in orgs)
                                {
                                    ConsoleEx.Write($"Org name: {org.DisplayName}; dropping...", ConsoleColor.DarkYellow);
                                    await org.DestroyAsync(dbCtx);
                                    ConsoleEx.Write("Done!" + Environment.NewLine, ConsoleColor.DarkGreen);
                                }
                                ConsoleEx.Write("Done!" + Environment.NewLine, ConsoleColor.DarkGreen);
                            }
                        }
                    }
                    catch
                    {
                        //ignore
                    }
                    
                }

                //check if the default confirmDrop should be waved off
                var confirmDrop = !ContainsParam("suppressDropConfirmation", args);
                
                SetupDatabases(dbsToDrop, ctxsToMigrate, confirmDrop);

                ClearEfConnectionPoolsCache(full || ContainsParam("mh", args), full || ContainsParam("id", args));
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Sets up databases
        /// </summary>
        /// <param name="dbsToDrop"></param>
        /// <param name="ctxsToMmigrate"></param>
        /// <param name="confirmDrop"></param>
        protected void SetupDatabases(List<string> dbsToDrop, Dictionary<string, List<Type>> ctxsToMmigrate, bool confirmDrop = true)
        {
            //got here, so need to drop the dbs first in order to recreate them later
            if (dbsToDrop?.Count > 0)
            {
                if (confirmDrop)
                {
                    if (
                    !PromptUser(
                        $"You are about to drop the following databases: {string.Join(", ", dbsToDrop)}. Are you sure you want to proceed?"))
                        return;
                }

                DropDb(dbsToDrop.ToArray());
            }


            if (ctxsToMmigrate?.Count > 0)
            {
                try
                {
                    foreach (var dbName in ctxsToMmigrate.Keys)
                    {
                        var dbc = new DataSourceCredentials
                        {
                            DbName = dbName,
                            ServerHost = Dsc.ServerHost,
                            ServerPort = Dsc.ServerPort,
                            UserName = Dsc.UserName,
                            Pass = Dsc.Pass,
                            DataSourceProvider = DataSourceProvider.Npgsql
                        };

                        try
                        {
                            ConsoleEx.WriteLine($"Updating db: {dbName}... ", ConsoleColor.DarkGray);

                            //context will be scoped to credentials defined as default for the cmd
                            foreach (var type in ctxsToMmigrate[dbName])
                            {
                                

                                if (!typeof(IProvideDbContextFactory).GetTypeInfo().IsAssignableFrom(type.Ge‌​tTypeInfo()))
                                {
                                    ConsoleEx.WriteLine($"{type.FullName} does not implement {nameof(IProvideDbContextFactory)} and therefore will be skipped", ConsoleColor.DarkMagenta);
                                    continue;
                                }

                                ConsoleEx.Write($"Running migration on: {type.FullName}... ", ConsoleColor.DarkYellow);

                                //cannot rely on specific constructors in db contexts!
                                //var dbCtx = (DbContext)Activator.CreateInstance(type, new object[] { dbc.GetConnectionString(), true, dbc.DataSourceProvider });
                                //
                                //instead need to use some hocus pocus and only serve contexts that implement IProvideDbContextFactory
                                var dbCtxFacade = (IProvideDbContextFactory)Cartomatic.Utils.Ef.DbContextFactory.CreateDbContextFacade(type);

                                var dbCtx = dbCtxFacade.ProduceDbContextInstance(dbc.GetConnectionString(), true, dbc.DataSourceProvider);
                                
                                //this will create db if it dies not exist and apply all the pending migrations
                                dbCtx.CreateOrUpdateDatabase();

                                ConsoleEx.Write("Done!" + Environment.NewLine, ConsoleColor.DarkGreen);
                            }

                            ConsoleEx.Write("Done!" + Environment.NewLine, ConsoleColor.DarkGreen);
                            Console.WriteLine();
                        }
                        catch (Exception ex)
                        {
                            ConsoleEx.WriteErr($"OOOPS... Failed to create/update database: {dbName}");
                            HandleException(ex, true);
                            throw;
                        }
                    }
                }
                catch 
                {
                    //ignore
                }
            }
        }


        /// <summary>
        /// Resets the EF connection pools cache
        /// </summary>
        /// <param name="mh"></param>
        /// <param name="mbr"></param>
        protected void ClearEfConnectionPoolsCache(bool mh = true, bool mbr = true)
        {
            ConsoleEx.Write("Clearing EF connection pool cache... ", ConsoleColor.DarkYellow);

            //When exception will take place, EF will reaload internal cache
            //this is because EF reuses a pooled connection - connection details are still the same - maphive meta or mbr, but the 
            //database itself has been wiped out and pgsql will refuse to connect to it!

            if (mh)
            {
                try
                {
                    using (var ctx = GetMapHiveDbContext())
                    {
                        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                        ctx.Applications.FirstOrDefault();
                    }
                }
                catch (Exception)
                {
                    //ignore
                }
            }

            if (mbr)
            {
                try
                {
                    Task.WaitAll(Task.Factory.StartNew(async () =>
                    {
                        var userManager = MapHive.Core.Identity.UserManagerUtils.GetUserManager();
                        await userManager.FindByEmailAsync("some@email.com");
                    }));

                }
                catch (Exception)
                {
                    //ignore
                }
            }

            ConsoleEx.Write("Done!" + Environment.NewLine, ConsoleColor.DarkGreen);
        }
    }
}