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
        /// <param name="emailAccount"></param>
        /// <param name="emailTemplate"></param>
        /// <returns></returns>
        public static async Task ResendActivationLink(DbContext context, Guid userId, IEmailAccount emailAccount = null, IEmailTemplate emailTemplate = null)
        {
            //grab a user first
            var user = await Base.ReadObjAsync<MapHiveUser>(context, userId, true);

            //and use the user method - implemented at user lvl even though applies to auth - need to send email
            await user.ResendActivationLinkAsync(context, emailAccount, emailTemplate);
        }
    }
}
