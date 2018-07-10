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
        public class PassResetRequestOutput
        {
            public string VerificationKey { get; set; }
        }

        /// <summary>
        /// Generates a pass reset token
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static async Task<PassResetRequestOutput> RequestPassResetAsync(string email)
        {
            var passResetToken = string.Empty;

            var userManager = MapHive.Identity.UserManagerUtils.GetUserManager();
            var idUser = await userManager.FindByEmailAsync(email);
            if (idUser != null)
            {
                passResetToken = Auth.MergeIdWithToken(
                    idUser.Id,
                    await userManager.GeneratePasswordResetTokenAsync(idUser)
                );
            }

            return new PassResetRequestOutput
            {
                VerificationKey = passResetToken
            };
        }
    }
}
