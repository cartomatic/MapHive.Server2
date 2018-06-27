using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrockAllen.MembershipReboot;
using Cartomatic.Utils.Email;

#if NETFULL
using System.Data.Entity;
#endif
#if NETSTANDARD
using Microsoft.EntityFrameworkCore;
#endif

namespace MapHive.Core.DataModel
{
    /// <summary>
    /// creates a maphive user and his organisation if required (org is created by default)
    /// </summary>
    public partial class MapHiveUser
    {
        protected internal override async Task<T> CreateAsync<T, TAccount>(DbContext dbCtx, UserAccountService<TAccount> userAccountService, IEmailAccount emailAccount = null,
            IEmailTemplate emailTemplate = null)
        {
            EnsureSlug();

            var user = await base.CreateAsync<T, TAccount>(dbCtx, userAccountService, emailAccount, emailTemplate) as MapHiveUser;

            //unless user is marked as an org user, create an org for him
            if (!user.IsOrgUser)
            {
                await CreateUserOrganizationAsync(dbCtx, userAccountService);
            }

            return (T)(Base)user;
        }
    }
}
