using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.CmdPrompt.Core;
using Cartomatic.Utils.Data;
using MapHive.Core.Data;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using MapHive.MembershipReboot;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Cmd.Core
{

    public partial class CommandHandler
    {

        /// <summary>
        /// Handles setting up the MapHive environment - maphive meta db, idsrv db and membership reboot db
        /// </summary>
        /// <param name="args"></param>
        protected virtual async Task Handle_SetUp(IDictionary<string, string> args)
        {
            var cmd = GetCallerName();

            if (GetHelp(args))
            {
                Console.WriteLine(
                    $"'{cmd}' : sets up the maphive environment - maphive2_meta, maphive2_idsrv, maphive2_mr; uses the configured db credentials to connect to the db server.");
                Console.WriteLine($"syntax: {cmd} space separated params: ");
                Console.WriteLine("\t[full]; all the maphive databases should be created/ugraded");
                Console.WriteLine("\t[xfull]; all the maphive databases should be dropped prior to being recreated");
                Console.WriteLine("\t[mh]; maphive2_meta should be created/ugraded");
                Console.WriteLine("\t[mh]; maphive2_mr (MembershipReboot) should be created/ugraded");
                Console.WriteLine("\t[idsrv]; maphive2_idsrv (IdentityServer) should be created/ugraded");
                Console.WriteLine("\t[xmh]; maphive2_meta should be dropped prior to being recreated");
                Console.WriteLine("\t[xmr]; maphive2_mr (MembershipReboot) should be dropped prior to being recreated");
                Console.WriteLine("\t[xidsrv]; maphive2_idsrv (IdentityServer) should be dropped prior to being recreated");
                Console.WriteLine("\t[clean:bool]; when dropping mh db, org dbs should also be dropped; defaults to true;");

                Console.WriteLine($"example: {cmd} m mr idsrv xm xmr xidsrv");
                Console.WriteLine();

                return;
            }

            var dbsToDrop = new List<string>();
            var migrationCtxs = new Dictionary<DbContext, string>();

            var full = ContainsParam("full", args);
            var xfull = ContainsParam("xfull", args);

            if (full || ContainsParam("mh", args))
            {
                migrationCtxs[new MapHiveDbContext()] = "maphive2_meta";
            }
            if (xfull || ContainsParam("xmh", args))
            {
                dbsToDrop.Add("maphive2_meta");
            }
            if (full || ContainsParam("mr", args))
            {
                //FIXME...
                //GOT A PROBLEM HERE...
                //migrationCtxs[new MapHive.MembershipReboot.CustomDbContext("")] = "maphive2_mr";
            }
            if (xfull || ContainsParam("xmr", args))
            {
                dbsToDrop.Add("maphive2_mr");
            }
            if (full || ContainsParam("maphive2_idsrv", args))
            {
                //TODO - no new idsrv implementation yet!
            }
            if (xfull || ContainsParam("xidsrv", args))
            {
                dbsToDrop.Add("maphive2_idsrv");
            }

            var clean = !ContainsParam("clean", args) || ExtractParam<bool>("clean", args);


            if (dbsToDrop.Count == 0 && migrationCtxs.Count == 0)
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
                        //get all the orgs
                        var dbCtx = new MapHiveDbContext();
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
                    catch
                    {
                        //ignore
                    }
                    
                }

                //check if the default confirmDrop should be waved off
                var confirmDrop = !ContainsParam("suppressDropConfirmation", args);
                
                SetupDatabases(dbsToDrop, migrationCtxs, confirmDrop);

                ClearEfConnectionPoolsCache(full || ContainsParam("mh", args), full || ContainsParam("mr", args));
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Sets up databases
        /// </summary>
        /// <param name="dbsToDrop"></param>
        /// <param name="migrationCtxs"></param>
        protected void SetupDatabases(List<string> dbsToDrop, Dictionary<DbContext, string> migrationCtxs, bool confirmDrop = true)
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


            if (migrationCtxs?.Count > 0)
            {
                try
                {
                    foreach (var migrationCfg in migrationCtxs.Keys)
                    {
                        var dbc = new DataSourceCredentials
                        {
                            DbName = migrationCtxs[migrationCfg],
                            ServerHost = Dsc.ServerHost,
                            ServerPort = Dsc.ServerPort,
                            UserName = Dsc.UserName,
                            Pass = Dsc.Pass,
                            DataSourceProvider = DataSourceProvider.Npgsql
                        };

                        try
                        {
                            ConsoleEx.Write($"Updating db: {migrationCfg}... ", ConsoleColor.DarkYellow);

                            migrationCfg.Database.EnsureCreated();
                            migrationCfg.Database.Migrate();

                            ////TODO - make the provider name somewhat more dynamic...
                            //migrationCfg.TargetDatabase = new DbConnectionInfo(dbc.GetConnectionString(), "Npgsql");

                            //var migrator = new DbMigrator(migrationCfg);


                            //migrator.Update();


                            ConsoleEx.Write("Done!" + Environment.NewLine, ConsoleColor.DarkGreen);
                        }
                        catch (Exception ex)
                        {
                            ConsoleEx.WriteErr($"OOOPS... Failed to create/update database: {migrationCfg}");
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
                    var ctx = new MapHiveDbContext();
                    // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                    ctx.Applications.FirstOrDefault();
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
                    var userAccountService = CustomUserAccountService.GetInstance("MapHiveMembershipReboot");
                    userAccountService.GetByEmail("some@email.com");
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