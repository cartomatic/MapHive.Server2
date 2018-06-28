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
        //WARNING - problem with MBR - not maintained anymore, asp identity 3 is suggested to be used instead...
        //bollocks...
        
//        public async Task ResendActivationLinkAsync(DbContext dbCtx,
//            IEmailAccount emailAccount = null,
//            IEmailTemplate emailTemplate = null, string password = null)
//        {
//            //Note: MBR seems to not like resending an activation link with a new password... It simply does not generate a new one
//            //
//            //because there is a problem with storing an initial password (we would have to know it and it's not good), another
//            //trick is used.
//            //
//            //basically a new account is created in order to get a new token and a new pass
//            //such user account is then destroyed but the pass and token is set on the account that we try to resend a link for.


//            if (Uuid == default(Guid))
//                throw new InvalidOperationException("You cannot resend an activation link - this user has not yet been created...");


//            var mbrCtx = MembershipRebootUtils.GetMembershipRebootDbctx();

//            //get the mbr user object
//            var mbrUser = await mbrCtx.Users.FirstOrDefaultAsync(u => u.ID == Uuid);

//            if (mbrUser == null)
//                throw new InvalidOperationException("User does not exist in MBR.");


//            //need user account service to properly create a MBR user
//            var userAccountService =
//                MembershipRebootUtils.GetUserAccountService(MembershipRebootUtils.GetMembershipRebootDbctx());

//            //wire up an evt listener - this is the way mbr talks
//            AccountCreatedEvent<CustomUserAccount> e = null;
//            userAccountService.Configuration.AddEventHandler(
//                new MembershipAccountCreatedEvent<CustomUserAccount>(evt => e = evt));

//            //rrnd email - after all need to avoid scenarios when two folks try the same resend activation procedure at once
//            var rndEmail = $"{DateTime.Now.Ticks}@somedomain.com";

//            //finally a new rnd user, so we can get a properly recreated verification key and a new pass...
//            var newMbrAccount = userAccountService.CreateAccount(rndEmail, Cartomatic.Utils.Crypto.Generator.GenerateRandomString(10), rndEmail);

//            //update the account in question with 
//            //mbrUser.VerificationKey = newMbrAccount.VerificationKey;
//            //mbrUser.VerificationPurpose = newMbrAccount.VerificationPurpose;
//            //mbrUser.HashedPassword = newMbrAccount.HashedPassword;
//            //
//            //because the properties are read only, we need to do some crazy hocus-pocus again

//            //note: looks like the type returned via mbrCtx.Users.FirstOrDefaultAsync is somewhat more dynamic and does not
//            //map properly. therefore need to use a 'barebone' object instance
//            var obj = new CustomUserAccount();

//            //Warning - this sql is postgresql specific!
//            var updateSql = $@"UPDATE
//    {mbrCtx.GetTableSchema(obj)}.""{mbrCtx.GetTableName(obj)}""
//SET
//    ""{mbrCtx.GetTableColumnName(obj, nameof(mbrUser.VerificationKey))}"" = '{newMbrAccount.VerificationKey}',
//    ""{mbrCtx.GetTableColumnName(obj, nameof(mbrUser.VerificationPurpose))}"" = {(int)newMbrAccount.VerificationPurpose},
//    ""{mbrCtx.GetTableColumnName(obj, nameof(mbrUser.HashedPassword))}"" = '{newMbrAccount.HashedPassword}'
//WHERE
//    ""{mbrCtx.GetTableColumnName(obj, nameof(mbrUser.ID))}"" = '{mbrUser.ID}';";

//            //get rid of the new account
//            mbrCtx.Users.Remove(await mbrCtx.Users.FirstAsync(u => u.ID == newMbrAccount.ID));

//            //and save da mess
//            await mbrCtx.SaveChangesAsync();
//            await mbrCtx.Database.ExecuteSqlCommandAsync(updateSql);

//            //send out the email
//            if (emailTemplate != null && emailAccount != null)
//            {
//                Cartomatic.Utils.Email.EmailSender.Send(
//                    emailAccount,
//                    emailTemplate.Prepare(new Dictionary<string, object>
//                    {
//                        {nameof(e.VerificationKey), e.VerificationKey},
//                        {nameof(e.InitialPassword), e.InitialPassword}
//                    }),
//                    Email
//                );
//            }
//        }
    }
}
