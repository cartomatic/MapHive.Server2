using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.CmdPrompt.Core;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.Cmd
{
    public partial class CommandHandler
    {
        protected virtual async Task Handle_DestroyUser(Dictionary<string, string> args)
        {
            var cmd = GetCallerName();

            if (GetHelp(args))
            {
                Console.WriteLine($"'{cmd}' : destroys a user account");
                Console.WriteLine($"syntax: {cmd} space separated params: ");
                Console.WriteLine("\t[e:email]");
                Console.WriteLine($"\t[d:destroy the default user account ({MasterOrgEmail})]");
                Console.WriteLine();
                Console.WriteLine($"example: {cmd} e:{MasterOrgEmail}");
                Console.WriteLine($"example: {cmd} d:true");
                return;
            }

            //print remote mode, so it is explicitly communicated
            PrintRemoteMode();

            var email = ExtractParam("e", args);
            var dflt = ExtractParam<bool>("d", args);

            //use the default account if required
            if (dflt)
            {
                email = MasterOrgEmail;
            }

            if (string.IsNullOrEmpty(email))
            {
                ConsoleEx.WriteErr("You must either provide email of a user you're trying to wipe out or pass default:true; for more info type destroyuser help");
                Console.WriteLine();
                return;
            }

            //need a valid user to create a Core.Base object
            Cartomatic.Utils.Identity.ImpersonateGhostUser();


            //Note: db context uses a connection defined in app cfg. 
            if (RemoteMode)
            {
                ConsoleEx.Write($"Destroying user '{email}'... ", ConsoleColor.DarkRed);
                await DestroyUserRemoteAsync(email);
                ConsoleEx.WriteOk($"Done!");
            }
            else
            {
                using (var dbCtx = GetMapHiveDbContext())
                {
                    await DestroyUserAsync<MapHiveUser>(email, dbCtx);
                }
            }
            
            Console.WriteLine();
        }

        /// <summary>
        /// Destroys a user account
        /// </summary>
        /// <param name="email"></param>
        /// <param name="dbCtx"></param>
        protected virtual async Task DestroyUserAsync<T>(string email, DbContext dbCtx)
            where T : MapHive.Core.DataModel.MapHiveUser
        {
            try
            {
                var destroyed = false;
                //see if user exists and delete it if so
                var users = dbCtx.Set<T>();

                var mhUser =
                    users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
                if (mhUser != null)
                {
                    ConsoleEx.Write("Found mh user. Removing... ", ConsoleColor.DarkRed);
                    users.Remove(mhUser);
                    await dbCtx.SaveChangesAsync();
                    ConsoleEx.Write("Done! \n", ConsoleColor.DarkGreen);

                    destroyed = true;
                }

                var userManager = MapHive.Core.Identity.UserManagerUtils.GetUserManager();

                var idUser = await userManager.FindByEmailAsync(email.ToLower());
                if (idUser != null)
                {
                    ConsoleEx.Write("Found identity user. Removing... ", ConsoleColor.DarkRed);
                    await userManager.DeleteAsync(idUser);
                    ConsoleEx.Write("Done! \n", ConsoleColor.DarkGreen);

                    destroyed = true;
                }

                if (destroyed)
                {
                    ConsoleEx.WriteOk($"Destroyed user: '{email}'!");
                }

            }
            catch (Exception ex)
            {
                HandleException(ex);
                return;
            }
        }
    }
}
