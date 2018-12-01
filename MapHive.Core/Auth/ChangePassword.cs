using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapHive.Core
{
    public partial class Auth
    {
        public class ChangePassOutput
        {
            public bool Success { get; set; }

            public string FailureReason { get; set; }
        }

        /// <summary>
        /// Changes user's password
        /// </summary>
        /// <param name="newPass"></param>
        /// <param name="oldPass"></param>
        /// <returns></returns>
        public static async Task<ChangePassOutput> ChangePasswordAsync(string newPass, string oldPass)
        {
            var output = new ChangePassOutput {Success = true};

            //need to verify the user pass first and in order to do so, need to simulate user auth
            var uuid = Cartomatic.Utils.Identity.GetUserGuid();
            if (!uuid.HasValue)
                //this shouldn't happen really as the service should only allow authenticated access, but...
            {
                output.Success = false;
                output.FailureReason = "unknown_user";
            }
            else
            {
                try
                {
                    var userId = Cartomatic.Utils.Identity.GetUserGuid();;

                    var userManager = MapHive.Core.Identity.UserManagerUtils.GetUserManager();
                    var idUser = await userManager.FindByIdAsync(userId.ToString());

                    

                    if (idUser != null)
                    {
                        if (!await userManager.CheckPasswordAsync(idUser, oldPass))
                        {
                            output.FailureReason = "invalid_old_pass";
                            output.Success = false;
                        }
                        else if (await userManager.CheckPasswordAsync(idUser, newPass))
                        {
                            output.FailureReason = "new_pass_same_as_old_pass";
                            output.Success = false;
                        }
                        else
                        {
                            var passResetToken = await userManager.GeneratePasswordResetTokenAsync(idUser);


                            var result = await userManager.ResetPasswordAsync(idUser, passResetToken, newPass);

                            output.Success = result.Succeeded;

                        }
                    }
                }
                catch (Exception ex)
                {
                    output.Success = false;
                    output.FailureReason = "unknown_error";
                }
            }

            return output;
        }
    }
}
