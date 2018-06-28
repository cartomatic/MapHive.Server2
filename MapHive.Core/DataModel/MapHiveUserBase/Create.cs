using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Data;
using Cartomatic.Utils.Ef;
using Cartomatic.Utils.Email;
using MapHive.Core.DataModel.Validation;
using MapHive.Core.Events;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel
{

    public static partial class MapHiveUserCrudExtensions
    {
        /// <summary>
        /// Creates an object; returns a created object or null if it was not possible to create it due to the fact a uuid is already reserved
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TIdentityUser"></typeparam>
        /// <param name="obj"></param>
        /// <param name="dbCtx"></param>
        /// <param name="userManager"></param>
        /// <returns></returns>
        public static async Task<T> CreateAsync<T, TIdentityUser>(this T obj, DbContext dbCtx, UserManager<IdentityUser<Guid>> userManager)
            where T : MapHiveUserBase
            where TIdentityUser : IdentityUser<Guid>
        {
            return await obj.CreateAsync<T, TIdentityUser>(dbCtx, userManager);
        }

        public static async Task<T> CreateAsync<T, TIdentityUser>(this T obj, DbContext dbCtx, UserManager<IdentityUser<Guid>> userManager, IEmailAccount emailAccount, IEmailTemplate emailTemplate)
            where T : MapHiveUserBase
            where TIdentityUser : IdentityUser<Guid>
        {
            return await obj.CreateAsync<T, TIdentityUser>(dbCtx, userManager, emailAccount, emailTemplate);
        }
    }

    public abstract partial class MapHiveUserBase
    {
        /// <summary>
        /// Fired when user creation completes successfully
        /// </summary>
        /// <remarks>
        /// EvtHandler is a property so it is serializable by default. It contains some self refs, so serializers would go nuts. Need NonSerialized attribute. Not to mentions this property is not needeed on the user object anyway!
        /// </remarks>
        [NonSerialized]
        public EventHandler<IOpFeedbackEventArgs> UserCreated;

        /// <summary>
        /// Overrides the default Create method of Base and throws an exception. The default method cannot be used for a MapHive user object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        protected internal override Task<T> CreateAsync<T>(DbContext dbCtx)
        {
            throw new InvalidOperationException(WrongCrudMethodErrorInfo);
        }

        /// <summary>
        /// Creates a new user account in both MembershipReboot database and in the MapHive meta database;
        /// sends out a confirmation email if email account and template are provided
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TIdentityUser"></typeparam>
        /// <param name="userManager"></param>
        /// <param name="dbCtx"></param>
        /// <param name="emailAccount"></param>
        /// <param name="emailTemplate"></param>
        /// <returns></returns>
        protected internal virtual async Task<T> CreateAsync<T, TIdentityUser>(DbContext dbCtx, UserManager<IdentityUser<Guid>> userManager, IEmailAccount emailAccount = null, IEmailTemplate emailTemplate = null)
            where T : MapHiveUserBase
            where TIdentityUser : IdentityUser<Guid>
        {
            T output;

            //need to validate the model first
            await ValidateAsync(dbCtx);

            //make sure the email is ALWAYS lower case
            Email = Email.ToLower();

            //check if the email is already used or not; throw validation feedback exception if so
            //Note - could do it in the mh meta, but both dbs must be in sync anyway
            var emailInUse = await userManager.FindByEmailAsync(Email) != null;
            if (emailInUse)
            {
                throw Validation.Utils.GenerateValidationFailedException(nameof(Email), ValidationErrors.EmailInUse);
            }

            try
            {
                var rndPass = Cartomatic.Utils.Crypto.Generator.GenerateRandomString(10);
                var idUser = new IdentityUser<Guid>
                {
                    Id = Guid.NewGuid(),
                    UserName = Email.ToLower(),
                    Email = Email.ToLower()
                };
                var result = await userManager.CreateAsync(idUser, rndPass);

                //so can next pass some data to the mh meta user object
                this.Uuid = idUser.Id;

                //mbr work done, so can create the user within the mh metadata db
                output = await base.CreateAsync<T>(dbCtx);


                var opFeedback = new Dictionary<string, object>
                {
                    {"InitialPassword", rndPass},
                    {"VerificationKey", await userManager.GenerateEmailConfirmationTokenAsync(idUser)}
                };

                //if email related objects have been provided, send the account created email
                if (emailAccount != null && emailTemplate != null)
                {
                    EmailSender.Send(
                        emailAccount, emailTemplate.Prepare(opFeedback), Email
                    );
                }

                //finally the user created event
                UserCreated?.Invoke(
                    this,
                    new Events.OpFeedbackEventArgs
                    {
                        OperationFeedback = opFeedback
                    }
                );
            }
            catch (Exception ex)
            {
                throw Validation.Utils.GenerateValidationFailedException(ex);
            }

            return output;
        }
    }
}
