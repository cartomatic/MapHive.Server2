using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel
{
    public abstract partial class MapHiveUserBase
    {
        /// <summary>
        /// Destroys an object; returns destroyed object or null in a case it has not been found
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uuid"></param>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        protected internal override async Task<T> DestroyAsync<T>(DbContext dbCtx, Guid uuid)
        {

            //get a user
            var user = (await ReadAsync<T>(dbCtx, uuid)) as MapHiveUserBase;

            //and make sure user exists and has not been 'closed' before!
            if (user == null || user.IsAccountClosed)
                throw new ArgumentException(string.Empty);

            //Note: Not destroying the account really, just flagging it as closed. 
            //flag the user account as closed
            user.IsAccountClosed = true;

            //and simply update it
            return await user.UpdateAsync<T>(dbCtx, uuid);
        }
    }
}
