﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.CmdPrompt.Core;
using MapHive.Core.Data;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using MapHive.MembershipReboot;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Cmd.Core
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
                Console.WriteLine("\t[d:destroy the default user account (queen@maphive.net)]");
                Console.WriteLine();
                Console.WriteLine($"example: {cmd} e:queen@maphive.net");
                Console.WriteLine($"example: {cmd} d:true");
                return;
            }

            var email = ExtractParam("e", args);
            var dflt = ExtractParam<bool>("d", args);

            //use the default account if required
            if (dflt)
            {
                email = "queen@maphive.net";
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
            await DestroyUser<MapHiveUser>(email, new MapHiveDbContext("MapHiveMetadata"), CustomUserAccountService.GetInstance("WebGisMembershipReboot"));
            Console.WriteLine();
        }

        /// <summary>
        /// Destroys a user account
        /// </summary>
        /// <param name="email"></param>
        /// <param name="users"></param>
        /// <param name="mbrUserAccountService"></param>
        protected virtual async Task DestroyUser<T>(string email, DbContext dbCtx, CustomUserAccountService mbrUserAccountService)
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

                var mbrUser = mbrUserAccountService.GetByEmail(email.ToLower());
                if (mbrUser != null)
                {
                    ConsoleEx.Write("Found mbr user. Removing... ", ConsoleColor.DarkRed);
                    mbrUserAccountService.DeleteAccount(mbrUser.ID);
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
