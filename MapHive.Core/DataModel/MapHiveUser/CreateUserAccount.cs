﻿using System;
using System.Collections.Generic;
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
    public partial class MapHiveUser
    {
        /// <summary>
        /// Create user account output
        /// </summary>
        public class CreateUserAccountOutput
        {
            /// <summary>
            /// User object
            /// </summary>
            public MapHiveUser User { get; set; }

            /// <summary>
            /// Initial password
            /// </summary>
            public string InitialPassword { get; set; }

            /// <summary>
            /// verification key
            /// </summary>
            public string VerificationKey { get; set; }
        }

        /// <summary>
        /// Creates a user acount, sends out email, modifies pass if a custom pass is provided;
        /// this is a simple wrapper over the standard user.CreateAsync that adds an option to provide a specific password
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCtx"></param>
        /// <param name="user"></param>
        /// <param name="emailSender"></param>
        /// <param name="emailAccount"></param>
        /// <param name="emailTemplate"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static async Task<CreateUserAccountOutput> CreateUserAccountAsync(DbContext dbCtx,
            MapHiveUser user,
            IEmailSender emailSender,
            IEmailAccount emailAccount = null,
            IEmailTemplate emailTemplate = null, string password = null)
        {
            var output = new CreateUserAccountOutput
            {
                User = user
            };

            //see if a user has already been created. if so do not attempt to create it;
            if (user.Uuid != default(Guid))
                return output;

            //need to grab an initial pass to change it if a pass has been provided
            var initialPass = string.Empty;


            //wire up an evt listener, so can react to user created evt and send a confirmation email
            user.UserCreated += (sender, args) =>
            {
                initialPass = (string)args.OperationFeedback["InitialPassword"];

                //output, so can use it in the m2m tests
                output.InitialPassword = initialPass;
                output.VerificationKey =
                    Auth.MergeIdWithToken(
                        user.Uuid,
                        (string) args.OperationFeedback["VerificationKey"]
                    );
                    

                //prepare email if present
                emailTemplate?.Prepare(args.OperationFeedback);

                if (emailTemplate != null && emailAccount != null)
                {
                    emailSender.Send(
                        emailAccount,
                        emailTemplate,
                        user.Email
                    );
                }
            };

            //create user without auto email send here - it's customised and sent via evt handler above 
            var createdUser = await user.CreateAsync(dbCtx);

            //once user has been created adjust his pass if provided
            if (!string.IsNullOrEmpty(password))
            {
                //grab user manager
                var userManager = MapHive.Core.Identity.UserManagerUtils.GetUserManager();

                var idUser = await userManager.FindByIdAsync(user.Uuid.ToString());

                await userManager.ChangePasswordAsync(idUser, initialPass, password);
            }

            return output;
        }
    }
}
