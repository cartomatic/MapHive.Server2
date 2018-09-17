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

namespace MapHive.Core.Cmd
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
 Console.WriteLine("\t[org:orguuid] identifier of an organisation to drop.");
                Console.WriteLine("\t[clean:bool=true] whether or not to drop an organisation (and its database) previously assigned to a user, if any; defaults to true");
                Console.WriteLine();
                Console.WriteLine($"example: {cmd} org:someuuid");
                return;
            }

            //print remote mode, so it is explicitly communicated
            PrintRemoteMode();

            var clean = !args.ContainsKey("clean") || ExtractParam<bool>("clean", args);

            if (Guid.TryParse(ExtractParam<string>("org", args), out var orgId))
            {
                await DropOrganizationAsync(orgId, clean);
            }
            else
            {
                ConsoleEx.WriteErr($"Invalid org id: {orgId}");
            };

            Console.WriteLine();
        }
        
        /// <summary>
        /// Drops an organization by slug
        /// </summary>
        /// <param name="orgSlug"></param>
        /// <param name="clean"></param>
        /// <returns></returns>
        protected async Task<bool> DropOrganizationAsync(string orgSlug, bool clean)
        {
            Guid? orgId = null;

            if (RemoteMode)
            {
                //grab an org remotely
                var org = await GetOrgRemoteAsync(orgSlug);
                orgId = org?.Uuid;
            }
            else
            {
                using (var dbCtx = GetMapHiveDbContext())
                {
                    var org = await dbCtx.Organizations.Where(x => x.Slug == orgSlug).FirstOrDefaultAsync();
                    orgId = org?.Uuid;
                }
            }

            if (orgId.HasValue)
            {
                return await DropOrganizationAsync(orgId.Value, clean);
            }

            //no such org, pretend successful drop
            return true;
        }


        /// <summary>
        /// Drops an organization by id
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="clean"></param>
        /// <returns>Whether or not an org drop procedure has been performed</returns>
        protected async Task<bool> DropOrganizationAsync(Guid orgId, bool clean)
        {
            var output = false;

            //remote mode
            if (RemoteMode)
            {
                var org = await GetOrgRemoteAsync(orgId);
                if (org != null)
                {
                    Console.WriteLine();
                    if (clean &&
                        !PromptUser(
                            "A database previously registered to the specified user already exists and you are about to delete it. Proceed?"))
                        return false;

                    //org exists, so need to drop it with a remote API...
                    ConsoleEx.Write($"Dropping organisation '{org.DisplayName}' ({org.Slug}) with its db... ", ConsoleColor.DarkYellow);
                    await DropOrgRemoteAsync(orgId, clean);
                    ConsoleEx.Write("Done!" + Environment.NewLine, ConsoleColor.DarkGreen);

                    //assume it's been dropped. otherwise api will throw (at least it should...)
                    output = true;
                }
            }
            //and local
            else
            {
                //if an organisation for a specified user exists, destroy it
                using (var dbCtx = GetMapHiveDbContext())
                {
                    var org = await dbCtx.Organizations.Where(x => x.Uuid == orgId).FirstOrDefaultAsync();

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
                    }
                }

                output = true;
            }

            Console.WriteLine();
            return output;
        }
    }
}
