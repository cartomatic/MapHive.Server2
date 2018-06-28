using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel
{
    /// <summary>
    /// creates a maphive user and his organization if required (org is created by default)
    /// </summary>
    public partial class MapHiveUser
    {
        protected internal override async Task<T> CreateAsync<T, TIdentityUser>(DbContext dbCtx, UserManager<IdentityUser<Guid>> userManager, IEmailAccount emailAccount = null,
            IEmailTemplate emailTemplate = null)
        {
            EnsureSlug();

            var user = await base.CreateAsync<T, TIdentityUser>(dbCtx, userManager, emailAccount, emailTemplate) as MapHiveUser;

            //unless user is marked as an org user, create an org for him
            if (!user.IsOrgUser)
            {
                await CreateUserOrganizationAsync<TIdentityUser>(dbCtx, userManager);
            }

            return (T)(Base)user;
        }
    }
}
