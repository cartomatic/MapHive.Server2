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
        public class AccountActivationOutput
        {
            public bool Success { get; set; }

            public bool VerificationKeyStale { get; set; }

            public bool UnknownUser { get; set; }

            public string VerificationKey { get; set; }

            public string Email { get; set; }
        }

        public static async Task<AccountActivationOutput> ActivateAccountAsync(string email, string emailConfirmationToken)
        {
            var output = new AccountActivationOutput();

            var userManager = MapHive.Identity.UserManagerUtils.GetUserManager();
            var idUser = await userManager.FindByEmailAsync(email);
            if (idUser != null)
            {
                await userManager.ConfirmEmailAsync(idUser, emailConfirmationToken);
            }

            //Note:
            //perhaps token can expire???

            output.Success = true;
            return output;
        }
    }
}
