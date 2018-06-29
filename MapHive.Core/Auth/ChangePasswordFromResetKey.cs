﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Identity;

namespace MapHive.Core
{
    public partial class Auth
    {
        public class ChangePasswordFromResetKeyOutput
        {
            public bool Success { get; set; }

            public string FailureReason { get; set; }
        }

        public static async Task<ChangePasswordFromResetKeyOutput> ChangePasswordFromResetKeyAsync(Guid userId,
            string newPass, string passResetToken)
        {
            var userManager = MapHive.Identity.UserManagerUtils.GetUserManager();
            var idUser = await userManager.FindByIdAsync(userId.ToString());

            return await ChangePasswordFromResetKeyAsync(idUser, newPass, passResetToken);
        }

        public static async Task<ChangePasswordFromResetKeyOutput> ChangePasswordFromResetKeyAsync(string email,
            string newPass, string passResetToken)
        {
            var userManager = MapHive.Identity.UserManagerUtils.GetUserManager();
            var idUser = await userManager.FindByEmailAsync(email);

            return await ChangePasswordFromResetKeyAsync(idUser, newPass, passResetToken);
        }

        public static async Task<ChangePasswordFromResetKeyOutput> ChangePasswordFromResetKeyAsync(MapHiveIdentityUser idUser, string newPass, string passResetToken)
        {
            var output = new ChangePasswordFromResetKeyOutput();
            try
            {
                var userManager = MapHive.Identity.UserManagerUtils.GetUserManager();

                if (idUser != null)
                {
                    if (await userManager.CheckPasswordAsync(idUser, newPass))
                    {
                        output.FailureReason = "new_pass_same_as_old_pass";
                        output.Success = false;
                    }
                    else
                    {
                        await userManager.ResetPasswordAsync(idUser, passResetToken, newPass);
                        output.Success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                output.FailureReason = "unknown_error";
            }

            return output;
        }
    }
}
