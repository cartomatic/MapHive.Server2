using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.CmdPrompt.Core;
using MapHive.Core.DAL;
using MapHive.Core.Data;
using MapHive.Core.DataModel;
using MapHive.MembershipReboot;

namespace MapHive.Cmd.Core
{
    public partial class CommandHandler
    {
        protected virtual async Task Handle_AddUser(Dictionary<string, string> args)
        {
            var cmd = GetCallerName();

            if (GetHelp(args))
            {
                Console.WriteLine($"'{cmd}' : adds a user to the system");
                Console.WriteLine($"syntax: {cmd} space separated params: ");
                Console.WriteLine("\t[e:email]");
                Console.WriteLine("\t[p:pass]");
                Console.WriteLine();
                Console.WriteLine($"example: {cmd} e:queen@maphive.net p:test");
                return;
            }

            var email = ExtractParam("e", args);
            var pass = ExtractParam("p", args);


            //use the default account if email and pass not provided
            if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(pass))
            {
                email = "queen@maphive.net";
                pass = "test";
            }

            ConsoleEx.WriteLine($"Creating user: '{email}' with the following pass: '{pass}'", ConsoleColor.DarkYellow);


            await CreateUser(email, pass);
        }

        /// <summary>
        /// Creates a user via remote core api call
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual async Task Handle_AddUserRemote(Dictionary<string, string> args)
        {
            var cmd = GetCallerName();

            if (GetHelp(args))
            {
                Console.WriteLine($"'{cmd}' : adds a user to the system");
                Console.WriteLine($"syntax: {cmd} space separated params: ");
                Console.WriteLine("\t[e:email]");
                Console.WriteLine("\t[p:pass]");
                Console.WriteLine();
                Console.WriteLine($"example: {cmd} e:queen@maphive.net p:test");
                return;
            }

            var email = ExtractParam("e", args);
            var pass = ExtractParam("p", args);


            //use the default account if email and pass not provided
            if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(pass))
            {
                email = "queen@maphive.net";
                pass = "test";
            }

            ConsoleEx.Write($"Creating user: '{email}' with the following pass: '{pass}'... ", ConsoleColor.DarkYellow);
            await CreateUserRemoteAsync(email, pass, true);
            ConsoleEx.Write("Done !" + Environment.NewLine, ConsoleColor.DarkGreen);
        }


        /// <summary>
        /// Creates a web gis user account
        /// </summary>
        /// <param name="email"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        protected async Task<MapHiveUser> CreateUser(string email, string pass)
        {
            //need a valid user to create a Core.Base object
            Cartomatic.Utils.Identity.ImpersonateGhostUser();

            var user = new MapHiveUser
            {
                Email = email
            };

            //Note: db context uses a connection defined in app cfg. 
            //TODO - make it somewhat dynamic!          
            try
            {
                //destroy a previous account if any
                await DestroyUser<MapHiveUser>(email, new MapHiveDbContext("MapHiveMetadata"), 
                    CustomUserAccountService.GetInstance("WebGisMembershipReboot"));

                IDictionary<string, object> op = null;
                user.UserCreated += (sender, eventArgs) =>
                {
                    op = eventArgs.OperationFeedback;
                };

                await user.CreateAsync(new MapHiveDbContext("MapHiveMetadata"),
                    CustomUserAccountService.GetInstance("WebGisMembershipReboot"));

                //once user is created, need to perform an update in order to set it as valid
                user.IsAccountVerified = true;
                await user.UpdateAsync(new MapHiveDbContext("MapHiveMetadata"),
                    CustomUserAccountService.GetInstance("WebGisMembershipReboot"), user.Uuid);

                //and also need to change the pass as the default procedure autogenerates a pass
                CustomUserAccountService.GetInstance("WebGisMembershipReboot")
                    .ChangePassword(user.Uuid, (string) op["InitialPassword"], pass);

                ConsoleEx.WriteOk($"User '{email}' with the following pass: '{pass}' has been created.");
                Console.WriteLine();

                return user;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return null;
            }
        }
    }
}
