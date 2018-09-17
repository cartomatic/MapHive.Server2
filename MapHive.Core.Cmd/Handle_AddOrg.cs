using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cartomatic.CmdPrompt.Core;
using MapHive.Core.DataModel;
using MapHive.Core.DataModel.Validation;
using MapHive.Core.DAL;

namespace MapHive.Core.Cmd
{
    public partial class CommandHandler
    {

        /// <summary>
        /// Adds an org to the env
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual async Task Handle_AddOrg(Dictionary<string, string> args)
        {
            var cmd = GetCallerName();

            if (GetHelp(args))
            {
                Console.WriteLine($"'{cmd}' : adds an organisation to the system and registers it to the specified owner user. Org initiall does not have any apps assigned, so in order to make apps accessible to an organization, one needs to register them separately");
                Console.WriteLine($"syntax: {cmd} space separated params: ");
                Console.WriteLine("\t[e:email] email of an owner user");
                Console.WriteLine("\t[p:pass] password of an owner user");
                Console.WriteLine("\t[n:orgname] name of the organisation.");
                Console.WriteLine("\t[d:orgdesc] description of the organisation.");
                Console.WriteLine("\t[morg:bool presence] whether or not this should be a master organization; defaults to false");
                Console.WriteLine("\t[clean:bool presence] whether or not to drop an organisation (and its database) previously assigned to a user, if any; defaults to true");
                Console.WriteLine();
                Console.WriteLine($"example: {cmd} e:someone@maphive.net p:test");
                return;
            }

            var email = ExtractParam("e", args);
            var pass = ExtractParam("p", args);
            var clean = ContainsParam("clean", args);
            var orgName = ExtractParam<string>("n", args);
            var orgDescription = ExtractParam<string>("d", args);
            var morg = ContainsParam("morg", args);

            if (string.IsNullOrWhiteSpace(orgName))
            {
                ConsoleEx.WriteErr("Organization needs a name.");
                Console.WriteLine();
                return;
            }

            //use the default account if email and pass not provided
            if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(pass))
            {
                ConsoleEx.WriteErr("Organization needs a valid owner email & pass.");
                Console.WriteLine();
                return;
            }

            //ensure the site admin app is present!!!
            await RegisterAppsAsync(new[] { "masterofpuppets" });

            var apps = new List<string>();
            if(morg)
                apps.Add("masterofpuppets");

            //todo - allow specifying apps via param??? dunno, maybe so

            await CreateOrganisationAsync(orgName, orgDescription, email, pass, apps, clean);

            Console.WriteLine();
        }

        /// <summary>
        /// Adds master org to the env
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual async Task Handle_AddMasterOrg(Dictionary<string, string> args)
        {
            var cmd = GetCallerName();

            if (GetHelp(args))
            {
                Console.WriteLine($"'{cmd}' : adds a master organisation to the system and registers it to the specified owner user. Master org is granted access to the SiteAdmin application.");
                Console.WriteLine($"syntax: {cmd} space separated params: ");
                Console.WriteLine("\t[e:email] email of an owner user");
                Console.WriteLine("\t[p:pass] password of an owner user");
                Console.WriteLine("\t[org:orgname] name of the organisation. Defaults to 'THE HIVE'");
                Console.WriteLine("\t[clean:bool presence] whether or not to drop an organisation (and its database) previously assigned to a user, if any; defaults to true");
                Console.WriteLine();
                Console.WriteLine($"example: {cmd} e:someone@maphive.net p:test");
                return;
            }
            
            var email = ExtractParam("e", args);
            var pass = ExtractParam("p", args);
            var clean = ContainsParam("clean", args);
            var orgName = ExtractParam<string>("org", args);
            var orgDescription = string.Empty;
            if (string.IsNullOrWhiteSpace(orgName))
            {
                orgName = MasterOrgName;
                orgDescription = MasterOrgDesc;
            }

            //use the default account if email and pass not provided
            if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(pass))
            {
                email = MasterOrgEmail;
                pass = MasterOrgPass;
            }

            //ensure the site admin app is present!!!
            await RegisterAppsAsync(new[] {"masterofpuppets"});

            await CreateOrganisationAsync(orgName, orgDescription, email, pass, new [] { "masterofpuppets" }, clean);

            Console.WriteLine();
        }

        /// <summary>
        /// Creates an organisation with a specified owner and specified apps linked to the org
        /// </summary>
        /// <param name="orgName"></param>
        /// <param name="orgDescription"></param>
        /// <param name="email"></param>
        /// <param name="pass"></param>
        /// <param name="apps"></param>
        /// <param name="clean">whether or not existing org db should be dropped</param>
        /// <returns></returns>
        protected async Task<bool> CreateOrganisationAsync(string orgName, string orgDescription, string email, string pass, IEnumerable<string> apps, bool clean)
        {
            ConsoleEx.WriteLine($"Creating organisation with the following owner - user: '{email}' with the following pass: '{pass}'...", ConsoleColor.DarkYellow);

            var dropOrg = await DropOrganizationAsync(orgName, clean);
            if (!dropOrg)
            {
                return false;
            }

            //and create an org
            ConsoleEx.Write("Creating organization database and stuff... ", ConsoleColor.DarkYellow);

            //org slug
            var slug = MapHive.Core.Utils.Slug.GetOrgSlug(orgName, email);

            //create an org with owner and register apps
            if (RemoteMode)
            {
                //while remote org creation auto adds an owner user
                await CreateOrgRemoteAsync(orgName, orgDescription, slug, true, apps.Contains("masterofpuppets"), email, pass);

                //it does not register apps, so need to do it in a separate go
                var newOrg = await GetOrgRemoteAsync(slug);
                await RegisterAppsWithOrgRemoteAsync(newOrg, apps);
            }
            else
            {
                //add user
                var user = await CreateUserAsync(email, pass, email);

                //now the org object
                var newOrg = new Organization
                {
                    DisplayName = orgName,
                    Description = orgDescription,
                    Slug = slug
                };

                await newOrg.CreateAsync(new MapHiveDbContext(), user, apps);
            }
            
            //This should be all for now. This way org creation is quick. App related db stuff should be handled by the app related services when they're
            //accessed for the first time.


            ConsoleEx.Write("Done" + Environment.NewLine, ConsoleColor.DarkGreen);

            return true;
        }
    }
}
