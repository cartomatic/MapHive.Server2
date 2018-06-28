using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Relational;
using Cartomatic.Utils.Data;
using Cartomatic.Utils.Ef;
using Cartomatic.Utils.Email;
using MapHive.Core.DataModel.Validation;
using MapHive.Core.Events;
using MapHive.MembershipReboot;

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
        public class CreateUserAccountOutput
        {
            public MapHiveUser User { get; set; }

            public string InitialPassword { get; set; }

            public string VerificationKey { get; set; }
        }

        /// <summary>
        /// Creates a user acount, sends out email, modifies pass if a custom pass is provided
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCtx"></param>
        /// <param name="user"></param>
        /// <param name="emailAccount"></param>
        /// <param name="emailTemplate"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static async Task<CreateUserAccountOutput> CreateUserAccountAsync(DbContext dbCtx,
            MapHiveUser user,
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

            //looks like need to create a user
            using (var mbrDb = MembershipRebootUtils.GetMembershipRebootDbctx())
            {
                //need to grab an initial pass to change it if a pass has been provided
                var initialPass = string.Empty;

                //wire up an evt listener, so can react to user created evt and send a confirmation email
                user.UserCreated += (sender, args) =>
                {
                    initialPass = (string) args.OperationFeedback["InitialPassword"];

                    //output, so can use it in the m2m tests
                    output.InitialPassword = initialPass;
                    output.VerificationKey = (string) args.OperationFeedback["VerificationKey"];

                    //prepare email if present
                    emailTemplate?.Prepare(args.OperationFeedback);

                    if (emailTemplate != null && emailAccount != null)
                    {
                        Cartomatic.Utils.Email.EmailSender.Send(
                            emailAccount,
                            emailTemplate,
                            user.Email
                        );
                    }
                };

                //create user without auto email send here - it's customised and sent via evt handler above 
                var createdUser = await user.CreateAsync(dbCtx, MembershipRebootUtils.GetUserAccountService(mbrDb));

                //once user has been created adjust his pass if provided
                if (!string.IsNullOrEmpty(password))
                {
                    using (var mbrDbForPasswordChange = MembershipRebootUtils.GetMembershipRebootDbctx())
                    {
                        MembershipRebootUtils.GetUserAccountService(mbrDbForPasswordChange)
                            .ChangePassword(createdUser.Uuid, initialPass, password);

                        //user created with a specific pass
                        output.InitialPassword = password;
                    }
                }

                return output;
            }
        }
    }
}
