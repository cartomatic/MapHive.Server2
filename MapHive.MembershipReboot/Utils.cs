using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Relational;
#if NETFULL
using System.Data.Entity;
#endif
#if NETSTANDARD
using Microsoft.EntityFrameworkCore;
#endif

namespace MapHive.MembershipReboot
{
    public static class MembershipRebootUtils
    {
        private static CustomConfig mbrCfg { get; set; }

        static MembershipRebootUtils()
        {
            //this will suck in the mbr cfg defined in web.config if present
            mbrCfg = CustomConfig.Get();
        }

        /// <summary>
        /// Gets an instance of user account to interact with
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        public static CustomUserAccountService GetUserAccountService(CustomDbContext dbCtx)
        {
            return new CustomUserAccountService(
                new CustomConfig
                {
                    EmailIsUsername = mbrCfg.EmailIsUsername,
                    VerificationKeyLifetime = mbrCfg.VerificationKeyLifetime,
                    AccountLockoutDuration = mbrCfg.AccountLockoutDuration,
                    AccountLockoutFailedLoginAttempts = mbrCfg.AccountLockoutFailedLoginAttempts,
                    AllowAccountDeletion = mbrCfg.AllowAccountDeletion,
                    RequireAccountApproval = mbrCfg.RequireAccountApproval,
                    DefaultTenant = mbrCfg.DefaultTenant,
                    EmailIsUnique = mbrCfg.EmailIsUnique,
                    MultiTenant = mbrCfg.MultiTenant,
                    PasswordHashingIterationCount = mbrCfg.PasswordHashingIterationCount,
                    PasswordResetFrequency = mbrCfg.PasswordResetFrequency,
                    RequireAccountVerification = mbrCfg.RequireAccountVerification,
                    UsernamesUniqueAcrossTenants = mbrCfg.UsernamesUniqueAcrossTenants
                }, //clone self, otherwise will have a hell of problems with events piling up!
                new CustomUserAccountRepository(dbCtx)
            );
        }

        /// <summary>
        /// Gets an instance of custom MemebrshipReboot db context
        /// </summary>
        /// <returns></returns>
        public static CustomDbContext GetMembershipRebootDbctx()
        {
            return new CustomDbContext("MapHiveMembershipReboot");
        }

        /// <summary>
        /// Extracts a db context off the MembershipReboot's UserAccountService;
        /// uses reflection to grab a private db property of the Query property. May get nasty if mbr stuff changes internally. Oh well...
        /// </summary>
        /// <param name="userAccountService"></param>
        /// <returns></returns>
        public static DbContext GetMembershipRebootDbCtx<TAccount>(UserAccountService<TAccount> userAccountService)
            where TAccount : RelationalUserAccount
        {
            return userAccountService.Query.GetType()
                    .GetField("db", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(userAccountService.Query)
                as DbContext;
        }
    }

    public class MembershipAccountCreatedEvent<TAccount> : IEventHandler<AccountCreatedEvent<TAccount>>
    {
        private readonly Action<AccountCreatedEvent<TAccount>> _a;

        public MembershipAccountCreatedEvent(Action<AccountCreatedEvent<TAccount>> a)
        {
            _a = a;
        }

        public void Handle(AccountCreatedEvent<TAccount> evt)
        {
            _a?.Invoke(evt);
        }
    }

    public class MembershipPasswordResetRequestedEvent<TAccount> : IEventHandler<PasswordResetRequestedEvent<TAccount>>
    {
        private readonly Action<PasswordResetRequestedEvent<TAccount>> _a;

        public MembershipPasswordResetRequestedEvent(Action<PasswordResetRequestedEvent<TAccount>> a)
        {
            _a = a;
        }

        public void Handle(PasswordResetRequestedEvent<TAccount> evt)
        {
            _a?.Invoke(evt);
        }
    }
}
