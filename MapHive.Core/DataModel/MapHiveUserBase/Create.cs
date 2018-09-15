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
using MapHive.Core.Identity;
using MapHive.Core.Identity.DataModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel
{
    public abstract partial class MapHiveUserBase
    {
        /// <inheritdoc />
        protected internal override async Task<T> CreateAsync<T>(DbContext dbCtx)
        {
            return await CreateAsync<T>(dbCtx, null, null);
        }

        /// <summary>
        /// Fired when user creation completes successfully
        /// </summary>
        /// <remarks>
        /// EvtHandler is a property so it is serializable by default. It contains some self refs, so serializers would go nuts. Need NonSerialized attribute. Not to mentions this property is not needeed on the user object anyway!
        /// </remarks>
        [NonSerialized]
        public EventHandler<IOpFeedbackEventArgs> UserCreated;

        
        /// <summary>
        /// Creates a new user account in both Identity database and in the MapHive meta database;
        /// sends out a confirmation email if email account and template are provided
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCtx"></param>
        /// <param name="emailAccount"></param>
        /// <param name="emailTemplate"></param>
        /// <returns></returns>
        protected internal virtual async Task<T> CreateAsync<T>(DbContext dbCtx, IEmailAccount emailAccount, IEmailTemplate emailTemplate)
            where T : Base
        {
            T output;

            //need to validate the model first
            await ValidateAsync(dbCtx);

            //make sure the email is ALWAYS lower case
            Email = Email.ToLower();

            //grab user manager
            var userManager = MapHive.Core.Identity.UserManagerUtils.GetUserManager();

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
                var idUser = new MapHiveIdentityUser
                {
                    Id = Guid.NewGuid(),
                    UserName = Email.ToLower(),
                    Email = Email.ToLower()
                };
                var result = await userManager.CreateAsync(idUser, rndPass);

                //so can next pass some data to the mh meta user object
                this.Uuid = idUser.Id;

                //identity work done, so can create the user within the mh metadata db
                output = await base.CreateAsync<T>(dbCtx);


                var opFeedback = new Dictionary<string, object>
                {
                    {"InitialPassword", rndPass},
                    {
                        "VerificationKey",
                        Auth.MergeIdWithToken(
                            idUser.Id,
                            await userManager.GenerateEmailConfirmationTokenAsync(idUser)
                        )
                    }
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
