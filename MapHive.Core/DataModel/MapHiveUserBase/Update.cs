﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Data;
using Cartomatic.Utils.Ef;
using MapHive.Core.DataModel.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel
{
    public static partial class MapHiveUserCrudExtensions
    {
        /// <summary>
        /// Updates an object; returns an updated object or null if the object does not exist
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TIdentityUser"></typeparam>
        /// <param name="obj"></param>
        /// <param name="dbCtx"></param>
        /// <param name="userManager"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public static async Task<T> UpdateAsync<T, TIdentityUser>(this T obj, DbContext dbCtx, UserManager<IdentityUser<Guid>> userManager, Guid uuid)
            where T : MapHiveUserBase
            where TIdentityUser : IdentityUser<Guid>
        {
            return await obj.UpdateAsync<T, TIdentityUser>(dbCtx, userManager, uuid);
        }

        public static async Task<T> UpdateAsync<T, TIdentityUser>(this T obj, DbContext dbCtx, UserManager<IdentityUser<Guid>> userManager)
            where T : MapHiveUserBase
            where TIdentityUser : IdentityUser<Guid>
        {
            return await obj.UpdateAsync<T, TIdentityUser>(dbCtx, userManager, obj.Uuid);
        }
    }

    public abstract partial class MapHiveUserBase
    {
        /// <summary>
        /// Overrides the default Update method of Base and throws an exception. The default method cannot be used for a MapHive user object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCtx"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected internal override Task<T> UpdateAsync<T>(DbContext dbCtx, Guid uuid)
        {
            throw new InvalidOperationException(WrongCrudMethodErrorInfo);
        }

        /// <summary>
        /// Updates an object; returns an updated object or null if the object does not exist
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TIdentityUser"></typeparam>
        /// <param name="dbCtx"></param>
        /// <param name="userManager"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected internal virtual async Task<T> UpdateAsync<T, TIdentityUser>(DbContext dbCtx, UserManager<IdentityUser<Guid>> userManager, Guid uuid)
            where T : MapHiveUserBase
            where TIdentityUser : IdentityUser<Guid>
        {
            T output;

            //reassign guid - it usually comes through rest api, so not always present on the object itself
            //this is usually done at the base lvl, but need proper id for the user validation!
            Uuid = uuid;

            //need to validate the model first
            await ValidateAsync(dbCtx);


            //make sure the email is ALWAYS lower case
            Email = Email.ToLower();


            //Note: user account resides in two places - MembershipReboot and the MapHive metadata database.
            //therefore need t manage it in tow places and obviously make sure the ops are properly wrapped into transactions

       
            //first get the user as saved in the db
            var idUser = await userManager.FindByIdAsync(uuid.ToString());
            if (idUser == null)
                throw new ArgumentException(string.Empty);


            //in order to check if some mbr ops are needed need to compare the incoming data with the db equivalent
            var currentStateOfUser = await ReadAsync<T>(dbCtx, uuid);


            //work out if email is being updated and make sure to throw if it is not possible!
            var updateEmail = false;
            if (currentStateOfUser.Email != Email)
            {
                //looks like email is about to be changed, so need to check if it is possible to proceed
                var idUserWithSameEmail = await userManager.FindByEmailAsync(Email);
                if (idUserWithSameEmail != null && idUserWithSameEmail.Id != uuid)
                    throw Validation.Utils.GenerateValidationFailedException(nameof(Email), ValidationErrors.EmailInUse);

                //looks like we're good to go.
                updateEmail = true;
            }


            try
            {

                //check if mbr email related work is needed at all...
                if (updateEmail)
                {
                    var emailChangeToken = await userManager.GenerateChangeEmailTokenAsync(idUser, Email);
                    await userManager.ChangeEmailAsync(idUser, Email, emailChangeToken);

                    var confirmEmailToken = await userManager.GenerateEmailConfirmationTokenAsync(idUser);
                    await userManager.ConfirmEmailAsync(idUser, confirmEmailToken);
                }

                //also check the IsAccountClosed, as this may be modified via update too, not only via Destroy
                //btw. destroy just adjust the model's property and delegates the work to update
                if (currentStateOfUser.IsAccountClosed != IsAccountClosed)
                {
                    if (IsAccountClosed)
                    {
                        await userManager.SetLockoutEnabledAsync(idUser, true);
                    }
                    else
                    {
                        await userManager.SetLockoutEnabledAsync(idUser, false);
                    }
                }

                //check the account verification status
                if (!await userManager.IsEmailConfirmedAsync(idUser))
                {
                    if (IsAccountVerified)
                    {
                        var confirmEmailToken = await userManager.GenerateEmailConfirmationTokenAsync(idUser);
                        await userManager.ConfirmEmailAsync(idUser, confirmEmailToken);
                    }
                }
                else
                {
                    //force one way changes only
                    IsAccountVerified = true;
                }

                //mbr work done, so can update the user within the mh metadata db
                output = await base.UpdateAsync<T>(dbCtx, uuid);
            }
            catch (Exception ex)
            {
                throw Validation.Utils.GenerateValidationFailedException(ex);
            }

            return output;
        }

        /// <summary>
        /// Updates a user without changing the identity critical data - it's pretty much the email MUST stau intact
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCtx"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected internal virtual async Task<T> UpdateWithNoIdentityChangesAsync<T>(DbContext dbCtx)
            where T : MapHiveUserBase
        {
            T output;

            //need to validate the model first
            await ValidateAsync(dbCtx);


            //make sure the email is ALWAYS lower case
            Email = Email.ToLower();


            //Note: user account resides in two places - MembershipReboot and the MapHive metadata database.
            //therefore need t manage it in tow places and obviously make sure the ops are properly wrapped into transactions


            //first get the user as saved in the db
            var currentUser = await ReadAsync<T>(dbCtx, Uuid);
            if (currentUser == null)
                throw new ArgumentException("No such user");

            //make sure to maintain the email, the rest is as came here
            Email = currentUser.Email;

            output = await base.UpdateAsync<T>(dbCtx, Uuid);

            return output;
        }
    }
}
