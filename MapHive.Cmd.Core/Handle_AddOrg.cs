﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cartomatic.CmdPrompt.Core;
using MapHive.Core.Data;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;

namespace MapHive.Cmd.Core
{
    public partial class CommandHandler
    {
        public readonly string MasterOrgName = "THE HIVE";
        public readonly string MasterOrgDesc = "MapHIve env master organisation.";

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
                Console.WriteLine("\t[clean:bool=true] whether or not to drop an organisation (and its database) previously assigned to a user, if any; defaults to true");
                Console.WriteLine();
                Console.WriteLine($"example: {cmd} e:queen@maphive.net p:test");
                return;
            }
            
            var email = ExtractParam("e", args);
            var pass = ExtractParam("p", args);
            var clean = !args.ContainsKey("clean") || ExtractParam<bool>("clean", args);
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
                email = "queen@maphive.net";
                pass = "test";
            }

            //ensure the site admin app is present!!!
            await RegisterApps(new[] {"masterofpuppets"});

            await CreateOrganisation(orgName, orgDescription, email, pass, new [] { "masterofpuppets" }, clean);

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
        protected async Task<bool> CreateOrganisation(string orgName, string orgDescription, string email, string pass, IEnumerable<string> apps, bool clean)
        {
            ConsoleEx.WriteLine($"Creating organisation with the following owner - user: '{email}' with the following pass: '{pass}'...", ConsoleColor.DarkYellow);

            var dropOrg = await DropOrganization(orgName, clean);
            if (!dropOrg)
            {
                return false;
            }

            //add user
            var user = await CreateUser(email, pass);

            //and create an org
            ConsoleEx.Write("Creating organization database and stuff... ", ConsoleColor.DarkYellow);

            using (var dbCtx = new MapHiveDbContext())
            {
                //first create user account if not yet created
                if (user.Uuid == default(Guid))
                {
                    //use the default account_create email tpl here - basically it's null, so nothing is sent out
                    var userAccountOutput = await MapHiveUser.CreateUserAccountAsync(dbCtx, user, password: pass);
                    user = userAccountOutput.User;
                }

                //now the org object
                var newOrg = new Organization
                {
                    DisplayName = orgName,
                    Description = orgDescription
                };

                //create an org with owner and register webrouter app; wr is the app's short name here. btw short name is enforced unique in the db
                await newOrg.Create(dbCtx, user, apps);

                //This should be all for now. This way org creation is quick. App related db stuff should be handled by the app related services when they're
                //accessed for the first time.
            }

            ConsoleEx.Write("Done" + Environment.NewLine, ConsoleColor.DarkGreen);

            return true;
        }
    }
}
