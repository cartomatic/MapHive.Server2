using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrockAllen.MembershipReboot;

#if NETFULL
using System.Data.Entity;
#endif
#if NETSTANDARD
using Microsoft.EntityFrameworkCore;
#endif

namespace MapHive.Core.DataModel
{
    public partial class MapHiveUser
    {
        /// <summary>
        /// destroys a maphive user object and the user's organization if any
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TAccount"></typeparam>
        /// <param name="dbCtx"></param>
        /// <param name="userAccountService"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected internal override async Task<T> DestroyAsync<T, TAccount>(DbContext dbCtx, UserAccountService<TAccount> userAccountService, Guid uuid)
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

            return await base.DestroyAsync<T, TAccount>(dbCtx, userAccountService, uuid);
        }
    }
}
