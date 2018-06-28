using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel
{
    public static partial class MapHiveUserCrudExtensions
    {
        /// <summary>
        /// Destroys an object; returns destroyed object or null in a case it has not been found
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TIdentityUser"></typeparam>
        /// <param name="obj"></param>
        /// <param name="dbCtx"></param>
        /// <param name="userManager"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public static async Task<T> DestroyAsync<T, TIdentityUser>(this T obj, DbContext dbCtx, UserManager<IdentityUser<Guid>> userManager, Guid uuid)
            where T : MapHiveUserBase
            where TIdentityUser : IdentityUser<Guid>
        {
            return await obj.DestroyAsync<T, TIdentityUser>(dbCtx, userManager, uuid);
        }

        public static async Task<T> DestroyAsync<T, TIdentityUser>(this T obj, DbContext dbCtx, UserManager<IdentityUser<Guid>> userManager)
            where T : MapHiveUserBase
            where TIdentityUser : IdentityUser<Guid>
        {
            return await obj.DestroyAsync<T, TIdentityUser>(dbCtx, userManager, obj.Uuid);
        }

    }

    public abstract partial class MapHiveUserBase
    {
        /// <summary>
        /// Overrides the default Destroy method of Base and throws an exception. The default method cannot be used for a MapHive user object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCtx"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected internal override Task<T> DestroyAsync<T>(DbContext dbCtx, Guid uuid)
        {
            throw new InvalidOperationException(WrongCrudMethodErrorInfo);
        }

        /// <summary>
        /// Destroys an object; returns destroyed object or null in a case it has not been found
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TIdentityUser"></typeparam>
        /// <param name="uuid"></param>
        /// <param name="dbCtx"></param>
        /// <param name="userManager"></param>
        /// <returns></returns>
        protected internal virtual async Task<T> DestroyAsync<T, TIdentityUser>(DbContext dbCtx, UserManager<IdentityUser<Guid>> userManager, Guid uuid)
            where T : MapHiveUserBase
            where TIdentityUser : IdentityUser<Guid>
        {

            //get a user
            var user = await ReadAsync<T>(dbCtx, uuid);

            //and make sure user exists and has not been 'closed' before!
            if (user == null || user.IsAccountClosed)
                throw new ArgumentException(string.Empty);

            //Note: Not destroying the account really, just flagging it as closed. 
            //flag the user account as closed
            user.IsAccountClosed = true;

            //and simply update it
            return await user.UpdateAsync<T, TIdentityUser>(dbCtx, userManager, uuid);
        }
    }
}
