using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DAL;
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


        /// <summary>
        /// Force destroys a user record in the system. Since the MapHive core api is meant to be used in a distributed world, it is up to the consumer
        /// of this api to perform a necessary data cleanup. No relations will be deleted autmatically.
        /// Also, do be careful when using this method - a user may be registered in many places. so it is better to just disable it instead of erasing.
        /// Heck, you have been warned....
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        public async Task<bool> ForceDestroyAsync<T>(DbContext dbCtx)
            where T : MapHiveUserBase
        {
            return await ForceDestroyAsync<T>(dbCtx, Uuid);
        }

        /// <summary>
        /// Force destroys a user record in the system. Since the MapHive core api is meant to be used in a distributed world, it is up to the consumer
        /// of this api to perform a necessary data cleanup. No relations will be deleted autmatically.
        /// Also, do be careful when using this method - a user may be registered in many places. so it is better to just disable it instead of erasing.
        /// Heck, you have been warned....
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCtx"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public async Task<bool> ForceDestroyAsync<T>(DbContext dbCtx, Guid uuid)
            where T : MapHiveUserBase
        {

            var destroyed = false;
            //see if user exists and delete it if so
            var users = dbCtx.Set<T>();

            var user =
                users.FirstOrDefault(u => u.Uuid == uuid);

            if (user != null)
            {
                users.Remove(user);
                await dbCtx.SaveChangesAsync();
                destroyed = true;

                //make sure to destroy user links in the dbctx
                var dbRelations = (dbCtx as ILinksDbContext)?.Links;
                dbRelations?.RemoveRange(dbRelations.Where(x => x.ParentUuid == user.Uuid || x.ChildUuid == user.Uuid));
            }

            var userManager = MapHive.Core.Identity.UserManagerUtils.GetUserManager();
            var idUser = await userManager.FindByIdAsync(uuid.ToString());
            
            if (idUser != null)
            {
                await userManager.DeleteAsync(idUser);
                destroyed = true;
            }

            return destroyed;
        }
    }
}
