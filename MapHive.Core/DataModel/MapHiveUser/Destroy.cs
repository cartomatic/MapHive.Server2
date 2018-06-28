using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel
{
    public partial class MapHiveUser
    {
        /// <summary>
        /// destroys a maphive user object and the user's organization if any
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TIdentityUser"></typeparam>
        /// <param name="dbCtx"></param>
        /// <param name="userManager"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected internal override async Task<T> DestroyAsync<T, TIdentityUser>(DbContext dbCtx, UserManager<IdentityUser<Guid>> userManager, Guid uuid)
        {
            if (!IsOrgUser)
            {
                var org = await GetUserOrganizationAsync(dbCtx);
                //assume there may be some orphans floating around. at east during the dev...
                if (org != null)
                {
                    await org.DestroyAsync(dbCtx);
                }
            }

            return await base.DestroyAsync<T, TIdentityUser>(dbCtx, userManager, uuid);
        }
    }
}
