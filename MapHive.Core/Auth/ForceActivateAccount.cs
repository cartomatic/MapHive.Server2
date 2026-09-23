using System;
using System.Threading.Tasks;

namespace MapHive.Core
{
    public partial class Auth
    {
        /// <summary>
        /// Force activates account 
        /// </summary>
        /// <param name="mergedToken">merged token - guid + token</param>
        /// <returns></returns>
        public static async Task<AccountActivationOutput> ForceActivateAccountAsync(Guid userId)
        {
            var newActivationDetails = await Auth.GetNewAccountActivationDetailsAsync(userId);

            return await ActivateAccountAsync(userId, newActivationDetails.newAccountActivationToken);
        }
    }
}
