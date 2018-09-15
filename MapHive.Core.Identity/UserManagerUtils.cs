﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Data;
using Cartomatic.Utils.Ef;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using MapHive.Core.Identity.DAL;
using MapHive.Core.Identity.DataModel;

namespace MapHive.Core.Identity
{
    public class UserManagerUtils
    {
        private static UserManager<MapHiveIdentityUser> _userManager;
        private static SignInManager<MapHiveIdentityUser> _signInManager;

        private static bool Configured { get; set; }

        public static void Configure(string connStrName, bool isConnStr = false, DataSourceProvider dbProvider = DataSourceProvider.Npgsql)
        {
            var services = new ServiceCollection();

            services.AddDbContext<MapHiveIdentityDbContext>(
                options =>
                {
                    //this should configure the user manager stuff properly...
                    options.ConfigureProvider(
                        dbProvider,
                        Cartomatic.Utils.Ef.DbContextFactory.GetConnStr(connStrName, isConnStr)
                    );
                });
            //identity scopes
            services.ConfigureIdentityScopedServices();
            // Authentification
            services.AddIdentity();


            //services.AddScoped<IUserManagerService, UserManagerService>();

            // Build the IoC from the service collection
            var provider = services.BuildServiceProvider();

            //this does not seem to work
            //var userManagerService = provider.GetService<UserManagerService>();
            //_userManager = userManagerService.GetUserManager();

            //but this does...
            var userManagerService = (IUserManagerService)Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance(provider,
                typeof(UserManagerService));
            _userManager = userManagerService.GetUserManager();

            var signInManagerService = (ISignInManagerService)Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance(provider,
                typeof(SignInManagerService));

            _signInManager = signInManagerService.GetSignInManager();

            Configured = true;
        }

        /// <summary>
        /// Returns a configured user manager
        /// </summary>
        /// <returns></returns>
        public static UserManager<MapHiveIdentityUser> GetUserManager()
        {
            if (!Configured)
                throw new InvalidOperationException($"In order to user {nameof(UserManagerUtils)} you need to call {nameof(UserManagerUtils)}.{nameof(UserManagerUtils.Configure)} first!");

            return _userManager;
        }

        public static SignInManager<MapHiveIdentityUser> GetSignInManager()
        {
            if (!Configured)
                throw new InvalidOperationException($"In order to user {nameof(UserManagerUtils)} you need to call {nameof(UserManagerUtils)}.{nameof(UserManagerUtils.Configure)} first!");

            return _signInManager;
        }

        private interface IUserManagerService
        {
            UserManager<MapHiveIdentityUser> GetUserManager();
        }

        /// <summary>
        /// User manager service - so can use dependancy injection to obtain it! :)
        /// </summary>
        private class UserManagerService : IUserManagerService
        {
            private readonly UserManager<MapHiveIdentityUser> _userManager;

            public UserManagerService(UserManager<MapHiveIdentityUser> userManager)
            {
                this._userManager = userManager;
            }

            public UserManager<MapHiveIdentityUser> GetUserManager()
            {
                return _userManager;
            }
        }

        private interface ISignInManagerService
        {
            SignInManager<MapHiveIdentityUser> GetSignInManager();
        }

        /// <summary>
        /// SignIn manager service - so can use dependancy injection to obtain it! :)
        /// </summary>
        private class SignInManagerService : ISignInManagerService
        {
            private readonly SignInManager<MapHiveIdentityUser> _signInManager;

            public SignInManagerService(SignInManager<MapHiveIdentityUser> signInManager)
            {
                this._signInManager = signInManager;
            }

            public SignInManager<MapHiveIdentityUser> GetSignInManager()
            {
                return _signInManager;
            }
        }
    }
}