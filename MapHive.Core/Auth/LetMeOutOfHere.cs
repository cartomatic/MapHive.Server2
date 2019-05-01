using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapHive.Core
{
    public partial class Auth
    {
        /// <summary>
        /// Finalises a session for a particular accessToken
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static async Task LetMeOutOfHereAsync(string accessToken)
        {
            //TODO - use new IdentityModel and the token revocation endpoint or end session endpoint
        }
    }
}
