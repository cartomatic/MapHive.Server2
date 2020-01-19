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
