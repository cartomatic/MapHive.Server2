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
        /// updates a maphive user and his org if required
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TIdentityUser"></typeparam>
        /// <param name="dbCtx"></param>
        /// <param name="userManager"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected internal override async Task<T> UpdateAsync<T, TIdentityUser>(DbContext dbCtx, UserManager<IdentityUser<Guid>> userManager, Guid uuid)
        {
            //before updating, get org!
            //this is because the slug may have changed and it is necessary to grab an org before the update as otherwise it would not be possible.
            var org = await GetUserOrganizationAsync(dbCtx);
            
            var user = await base.UpdateAsync<T, TIdentityUser>(dbCtx, userManager, uuid) as MapHiveUser;

            if (org != null && org.Slug != user.Slug)
            {
                await user.UpdateUserOrganizationAsync(dbCtx, org);
            }

            return (T)(Base)user;
        }
    }
}
