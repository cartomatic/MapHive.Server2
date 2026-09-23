using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cartomatic.Utils.Email;
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

            var newActivationDetails = await Auth.GetNewAccountActivationDetailsAsync(Uuid);

            //generate new email confirmation token
            var emailConfirmationToken = 
                Auth.MergeIdWithToken(
                    Uuid,
                    newActivationDetails.newAccountActivationToken
                );
                

            //send out the email
            if (emailTemplate != null && emailAccount != null)
            {
                emailSender.Send(
                    emailAccount,
                    emailTemplate.Prepare(new Dictionary<string, object>
                    {
                        {"VerificationKey", emailConfirmationToken},
                        {"InitialPassword", newActivationDetails.newPass}
                    }),
                    Email
                );
            }
        }
    }
}
