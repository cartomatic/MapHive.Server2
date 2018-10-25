using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Data;
using Cartomatic.Utils.Ef;
using Cartomatic.Utils.Email;
using MapHive.Core.DataModel.Validation;
using MapHive.Core.Events;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel
{
    public partial class MapHiveUser
    {
        /// <summary>
        /// Resends activation link for a user account
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <param name="emailSender"></param>
        /// <param name="emailAccount"></param>
        /// <param name="emailTemplate"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task ResendActivationLinkAsync(DbContext dbCtx,
            IEmailSender emailSender,
            IEmailAccount emailAccount = null,
            IEmailTemplate emailTemplate = null, string password = null)
        {
            if (Uuid == default(Guid))
                throw new InvalidOperationException("You cannot resend an activation link - this user has not yet been created...");

            //grab user manager
            var userManager = MapHive.Core.Identity.UserManagerUtils.GetUserManager();

            //get the id-user user object
            var idUser = await userManager.FindByIdAsync(Uuid.ToString());

            if (idUser == null)
                throw new InvalidOperationException("User does not exist in IdentityManager.");

            //reset the previous pass
            var passResetToken = await userManager.GeneratePasswordResetTokenAsync(idUser);
            var newRndPass = Cartomatic.Utils.Crypto.Generator.GenerateRandomString(10);
            await userManager.ResetPasswordAsync(idUser, passResetToken, newRndPass);

            //generate new email confirmation token
            var emailConfirmationToken = 
                Auth.MergeIdWithToken(
                    idUser.Id,
                    await userManager.GenerateEmailConfirmationTokenAsync(idUser)
                );
                

            //send out the email
            if (emailTemplate != null && emailAccount != null)
            {
                emailSender.Send(
                    emailAccount,
                    emailTemplate.Prepare(new Dictionary<string, object>
                    {
                        {"VerificationKey", emailConfirmationToken},
                        {"InitialPassword", newRndPass}
                    }),
                    Email
                );
            }
        }
    }
}
