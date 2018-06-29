using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.CmdPrompt.Core;
using MapHive.Core.DAL;
using MapHive.Core.Data;
using MapHive.Core.DataModel;

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
                Console.WriteLine("\t[s:slug] user's slug; if not provided will be worked out from email");
                Console.WriteLine();
                Console.WriteLine($"example: {cmd} e:someone@maphive.net p:test");
                return;
            }

            var email = ExtractParam("e", args);
            var pass = ExtractParam("p", args);
            var slug = ExtractParam("s", args);

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
            {
                ConsoleEx.WriteErr("User name and pass cannot be empty!");
            }

            var user = new MapHiveUser
            {
                Email = email,
                Slug = slug
            };

            ConsoleEx.WriteLine($"Creating user: '{email}' with the following pass: '{pass}'; slug: {user.GetSlug()}", ConsoleColor.DarkYellow);

            //need a valid user to create a Core.Base object
            Cartomatic.Utils.Identity.ImpersonateGhostUser();

            


            //Note: db context uses a connection defined in app cfg. 
            //TODO - make it somewhat dynamic!          
            try
            {
                //destroy a previous account if any
                await DestroyUserAsync<MapHiveUser>(email, new MapHiveDbContext("MapHiveMeta"));

                IDictionary<string, object> op = null;
                user.UserCreated += (sender, eventArgs) =>
                {
                    op = eventArgs.OperationFeedback;
                };

                await user.CreateAsync(new MapHiveDbContext("MapHiveMeta"));

                //once user is created, need to perform an update in order to set it as valid
                user.IsAccountVerified = true;
                await user.UpdateAsync(new MapHiveDbContext("MapHiveMeta"));

                //and also need to change the pass as the default procedure autogenerates a pass
                var userManager = MapHive.Identity.UserManagerUtils.GetUserManager();
                var idUser = await userManager.FindByEmailAsync(email);
                await userManager.ChangePasswordAsync(idUser, (string)op["InitialPassword"], pass);
                

                ConsoleEx.WriteOk($"User '{email}' with the following pass: '{pass}' has been created.");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return;
            }

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
                Console.WriteLine("\t[s:slug] user's slug; if not provided will be based on email");
                Console.WriteLine();
                Console.WriteLine($"example: {cmd} e:someone@maphive.net p:test");
                return;
            }

            var email = ExtractParam("e", args);
            var pass = ExtractParam("p", args);
            var slug = ExtractParam("s", args);

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
            {
                ConsoleEx.WriteErr("User name and pass cannot be empty!");
            }

            var tempUser = new MapHiveUser
            {
                Email = email,
                Slug = slug
            };

            ConsoleEx.Write($"Creating user: '{email}' with the following pass: '{pass}'; slug: {tempUser.GetSlug()}", ConsoleColor.DarkYellow);
            await CreateUserRemoteAsync(email, pass, slug, true);
            ConsoleEx.Write("Done !" + Environment.NewLine, ConsoleColor.DarkGreen);
        }


        /// <summary>
        /// Creates a maphive user account
        /// </summary>
        /// <param name="email"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        protected async Task<MapHiveUser> CreateUserAsync(string email, string pass)
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
                await DestroyUserAsync<MapHiveUser>(email, new MapHiveDbContext("MapHiveMetadata"));

                IDictionary<string, object> op = null;
                user.UserCreated += (sender, eventArgs) =>
                {
                    op = eventArgs.OperationFeedback;
                };

                await user.CreateAsync(new MapHiveDbContext("MapHiveMetadata"));

                
                //and also need to change the pass as the default procedure autogenerates a pass
                var userManager = MapHive.Identity.UserManagerUtils.GetUserManager();
                var idUser = await userManager.FindByEmailAsync(email);
                await userManager.ChangePasswordAsync(idUser, (string) op["InitialPassword"], pass);

                //once user is created, need to perform an update in order to set it as valid
                user.IsAccountVerified = true;
                await user.UpdateAsync(new MapHiveDbContext("MapHiveMetadata"), user.Uuid);

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
