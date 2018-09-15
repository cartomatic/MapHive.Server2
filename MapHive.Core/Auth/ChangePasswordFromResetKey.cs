using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.Identity;
using MapHive.Core.Identity.DataModel;

namespace MapHive.Core
{
    public partial class Auth
    {
        public class ChangePasswordFromResetKeyOutput
        {
            public bool Success { get; set; }

            public string FailureReason { get; set; }
        }

        /// <summary>
        /// changes pass from a reset key
        /// </summary>
        /// <param name="newPass"></param>
        /// <param name="mergedToken"></param>
        /// <returns></returns>
        public static async Task<ChangePasswordFromResetKeyOutput> ChangePasswordFromResetKeyAsync(string newPass, string mergedToken)
        {
            return await ChangePasswordFromResetKeyAsync(
                ExtractIdFromMergedToken(mergedToken),
                newPass,
                ExtractTokenFromMergedToken(mergedToken)
            );
        }

        /// <summary>
        /// changes password from a reset key
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPass"></param>
        /// <param name="passResetToken"></param>
        /// <returns></returns>
        public static async Task<ChangePasswordFromResetKeyOutput> ChangePasswordFromResetKeyAsync(Guid? userId,
            string newPass, string passResetToken)
        {
            if (!userId.HasValue)
            {
                return new ChangePasswordFromResetKeyOutput
                {
                    FailureReason = "unknown_user"
                };
            }

            var userManager = MapHive.Core.Identity.UserManagerUtils.GetUserManager();
            

            var idUser = await userManager.FindByIdAsync(userId.ToString());

            return await ChangePasswordFromResetKeyAsync(idUser, newPass, passResetToken);
        }

        /// <summary>
        /// Changes pass from a areset key
        /// </summary>
        /// <param name="email"></param>
        /// <param name="newPass"></param>
        /// <param name="passResetToken"></param>
        /// <returns></returns>
        public static async Task<ChangePasswordFromResetKeyOutput> ChangePasswordFromResetKeyAsync(string email,
            string newPass, string passResetToken)
        {
            var userManager = MapHive.Core.Identity.UserManagerUtils.GetUserManager();
            var idUser = await userManager.FindByEmailAsync(email);

            return await ChangePasswordFromResetKeyAsync(idUser, newPass, passResetToken);
        }

        /// <summary>
        /// Changes pass from reset key
        /// </summary>
        /// <param name="idUser"></param>
        /// <param name="newPass"></param>
        /// <param name="passResetToken"></param>
        /// <returns></returns>
        public static async Task<ChangePasswordFromResetKeyOutput> ChangePasswordFromResetKeyAsync(MapHiveIdentityUser idUser, string newPass, string passResetToken)
        {
            var output = new ChangePasswordFromResetKeyOutput();
            try
            {
                var userManager = MapHive.Core.Identity.UserManagerUtils.GetUserManager();

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
