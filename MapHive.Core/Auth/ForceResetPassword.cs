using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.Events;

namespace MapHive.Core
{
    public partial class Auth
    {

        public class ForceResetPasswordOutput
        {
            public bool Success { get; set; }

            public string FailureReason { get; set; }
        }

        /// <summary>
        /// Force resets a user password to a specified one
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPass"></param>
        /// <returns></returns>
        public static async Task<ForceResetPasswordOutput> ForceResetPasswordAsync(Guid userId, string newPass)
        {
            var output = new ForceResetPasswordOutput();

            if (string.IsNullOrWhiteSpace(newPass))
            {
                output.FailureReason = "new_pass_null";
                return output;
            }

            try
            {
                var userManager = MapHive.Core.Identity.UserManagerUtils.GetUserManager();
                var idUser = await userManager.FindByIdAsync(userId.ToString());

                if (idUser != null)
                {
                    if (await userManager.CheckPasswordAsync(idUser, newPass))
                    {
                        output.FailureReason = "new_pass_same_as_old_pass";
                        output.Success = false;
                    }
                    else
                    {
                        var passResetToken = await userManager.GeneratePasswordResetTokenAsync(idUser);
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
