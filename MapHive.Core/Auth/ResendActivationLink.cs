using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Email;
using MapHive.Core.DataModel;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core
{
    public partial class Auth
    {
        /// <summary>
        /// Resends an activation link for a user; expects a user_created email template and a valid user identifier
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userId"></param>
        /// <param name="emailSender"></param>
        /// <param name="emailAccount"></param>
        /// <param name="emailTemplate"></param>
        /// <returns></returns>
        public static async Task ResendActivationLink(DbContext context, Guid userId, IEmailSender emailSender, IEmailAccount emailAccount = null, IEmailTemplate emailTemplate = null)
        {
            //grab a user first
            var user = await Base.ReadObjAsync<MapHiveUser>(context, userId, true);

            //and use the user method - implemented at user lvl even though applies to auth - need to send email
            await user.ResendActivationLinkAsync(context, emailSender, emailAccount, emailTemplate);
        }

        /// <summary>
        /// Gets new details required to activate an account. New details are required when trying to resend activation link or force activating account
        /// </summary>
        /// <returns></returns>
        public static async Task<(string newAccountActivationToken, string newPass)> GetNewAccountActivationDetailsAsync(Guid userId)
        {
            //grab user manager
            var userManager = MapHive.Core.Identity.UserManagerUtils.GetUserManager();

            //get the id-user user object
            var idUser = await userManager.FindByIdAsync(userId.ToString());

            if (idUser == null)
                throw new InvalidOperationException("User does not exist in IdentityManager.");

            //reset the previous pass
            var passResetToken = await userManager.GeneratePasswordResetTokenAsync(idUser);
            var newRndPass = Cartomatic.Utils.Crypto.Generator.GenerateRandomString(10);
            await userManager.ResetPasswordAsync(idUser, passResetToken, newRndPass);

            return (await userManager.GenerateEmailConfirmationTokenAsync(idUser), newRndPass);
        }
    }
}
