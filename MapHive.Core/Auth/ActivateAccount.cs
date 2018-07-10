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
        /// <summary>
        /// Account activation output
        /// </summary>
        public class AccountActivationOutput
        {
            public bool Success { get; set; }

            public bool VerificationKeyStale { get; set; }

            public bool UnknownUser { get; set; }

            public string VerificationKey { get; set; }

            public string Email { get; set; }
        }

        /// <summary>
        /// Activates account 
        /// </summary>
        /// <param name="mergedToken">merged token - guid + token</param>
        /// <returns></returns>
        public static async Task<AccountActivationOutput> ActivateAccountAsync(string mergedToken)
        {
            return await ActivateAccountAsync(
                ExtractIdFromMergedToken(mergedToken),
                ExtractTokenFromMergedToken(mergedToken)
            );
        }
        

        /// <summary>
        /// Activates user account
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="emailConfirmationToken"></param>
        /// <returns></returns>
        public static async Task<AccountActivationOutput> ActivateAccountAsync(Guid? userId, string emailConfirmationToken)
        {
            var output = new AccountActivationOutput();

            if (!userId.HasValue)
            {
                output.UnknownUser = true;
                return output;
            }

            var userManager = MapHive.Identity.UserManagerUtils.GetUserManager();

            //extract user guid from token

            var idUser = await userManager.FindByIdAsync(userId.ToString());
            if (idUser != null)
            {
                await userManager.ConfirmEmailAsync(idUser, emailConfirmationToken);
                output.Success = true;
            }
            else
            {
                output.UnknownUser = true;
            }

            //Note:
            //TODO perhaps token can expire as in v1???

            
            return output;
        }
    }
}
