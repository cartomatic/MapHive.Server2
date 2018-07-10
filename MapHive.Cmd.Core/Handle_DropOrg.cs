using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cartomatic.CmdPrompt.Core;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Cmd.Core
{
    public partial class CommandHandler
    {
        /// <summary>
        /// Drops an organization
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual async Task Handle_DropOrg(Dictionary<string, string> args)
        {
            var cmd = GetCallerName();

            if (GetHelp(args))
            {
                Console.WriteLine($"'{cmd}' : drops an org");
                Console.WriteLine($"syntax: {cmd} space separated params: ");
 Console.WriteLine("\t[org:orgname] name of the organisation. Defaults to 'THE HIVE'");
                Console.WriteLine("\t[clean:bool=true] whether or not to drop an organisation (and its database) previously assigned to a user, if any; defaults to true");
                Console.WriteLine();
                Console.WriteLine($"example: {cmd} e:queen@maphive.net p:test");
                return;
            }

            var clean = !args.ContainsKey("clean") || ExtractParam<bool>("clean", args);
            var orgName = ExtractParam<string>("org", args);

            await DropOrganizationAsync(orgName, clean);

            Console.WriteLine();
        }


        /// <summary>
        /// Drops an organization
        /// </summary>
        /// <param name="orgName"></param>
        /// <param name="clean"></param>
        /// <returns>Whether or not an org drop procedure has been performed</returns>
        protected async Task<bool> DropOrganizationAsync(string orgName, bool clean)
        {
            //if an organisation for a specified user exists, destroy it
            using (var dbCtx = new MapHiveDbContext())
            {
                var org = await dbCtx.Organizations.Where(x => x.DisplayName == orgName).FirstOrDefaultAsync();


                if (org != null)
                {
                    //to drop db need to connect to a proper db (org can be somewhere out there...), so need to save the current dsc
                    var dsc = Dsc.Clone();
                    org.LoadDatabases(dbCtx);

                    Console.WriteLine();
                    if (clean &&
                        !PromptUser(
                            "A database previously registered to the specified user already exists and you are about to delete it. Proceed?"))
                        return false;

                    //clean the org rec in the metadata
                    ConsoleEx.Write($"Dropping organisation '{org.DisplayName}' ({org.Slug}) with its db... ", ConsoleColor.DarkYellow);

                    await org.DestroyAsync(dbCtx, clean);

                    ConsoleEx.Write("Done!" + Environment.NewLine, ConsoleColor.DarkGreen);
                    Console.WriteLine();
                }
            }
            return true;
        }


        /// <summary>
        /// Drops an organization
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual async Task Handle_DropOrgRemote(Dictionary<string, string> args)
        {
            var cmd = GetCallerName();

            if (GetHelp(args))
            {
                Console.WriteLine($"'{cmd}' : drops an org");
                Console.WriteLine($"syntax: {cmd} space separated params: ");
                Console.WriteLine("\t[org:orgname] name of the organisation. Defaults to 'THE HIVE'");
                Console.WriteLine();
                Console.WriteLine($"example: {cmd} org:someOrgName");
                return;
            }

            var orgName = ExtractParam<string>("org", args);
            
            //cmd is a simplistic tool, so splits params on spaces
            //need to provide a fake mechanism for hacking it
            orgName = (orgName ?? "").Replace("_", " ");

            //grab the master org
            var org = await GetOrgRemoteAsync(orgName);
            if (org != null)
            {
                //org exists, so need to drop it with a remote API...
                await DropOrgRemoteAsync(org.Uuid);
                Console.WriteLine("Dropped a remote org!");
            }
            else
            {
                Console.WriteLine("No such org!");
            }

            //basically endpoint performs a clean destroy, so cleans up all the stuff, including users, connections
            //and <<local>> databases
            //local means local to the server, so pretty much same host / port
            //all the remote dbs need to be dropped at the discretion of specific apps!
            //this is because the core API may not even know if some remote ones use dbs or not!
            //for the registered apis, the core api issues 'drop org' calls, so the actual cleanup is performed at the discretion of each api!
            

            Console.WriteLine();
        }
    }
}
