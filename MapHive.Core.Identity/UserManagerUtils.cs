using System;
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
        //private static UserManager<MapHiveIdentityUser> _userManager;
        //private static SignInManager<MapHiveIdentityUser> _signInManager;

        private static IServiceCollection _services;

        private static bool Configured { get; set; }

        /// <summary>
        /// Configures user manager utils
        /// </summary>
        /// <param name="connStrName"></param>
        /// <param name="isConnStr"></param>
        /// <param name="dbProvider"></param>
        public static void Configure(string connStrName, bool isConnStr = false, DataSourceProvider dbProvider = DataSourceProvider.Npgsql)
        {
            Configure(null, connStrName, isConnStr, dbProvider);
        }

        /// <summary>
        /// Configures user manager utils
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connStrName"></param>
        /// <param name="isConnStr"></param>
        /// <param name="dbProvider"></param>
        public static void Configure(IServiceCollection services, string connStrName, bool isConnStr = false, DataSourceProvider dbProvider = DataSourceProvider.Npgsql)
        {
            if(services == null)
                services = new ServiceCollection();

            _services = services;

            services.AddDbContext<MapHiveIdentityDbContext>(
                options =>
                {
                    //this should configure the user manager stuff properly...
                    options.ConfigureProvider(
                        dbProvider,
                        Cartomatic.Utils.Ef.DbContextFactory.GetConnStr(connStrName, isConnStr)
                    );

                }, ServiceLifetime.Scoped, ServiceLifetime.Scoped);

            //identity scopes
            services.ConfigureIdentityScopedServices();

            // Authentification
            services.AddIdentity()
                .AddEntityFrameworkStores<MapHiveIdentityDbContext>();



            //Note:
            //Stuff below works ok, when working with a single instance of service, and the identity db is not changed elswhere
            //in such scenario a new instance of user manager & sign in manager is required
            //otherwise it will not recognise changes applied externally due to the data chache
            //every try of modifying a user modified externally would result in ConcurrencyFailure: Optimistic concurrency failure, object has been modified.

            //Build the IoC from the service collection
            //var provider = services.BuildServiceProvider();

            //this does not seem to work
            //var userManagerService = provider.GetService<UserManagerService>();
            //_userManager = userManagerService.GetUserManager();

            ////but this does...
            //var userManagerService = (IUserManagerService)Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance(provider,
            //    typeof(UserManagerService));
            //_userManager = userManagerService.GetUserManager();

            //var signInManagerService = (ISignInManagerService)Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance(provider,
            //    typeof(SignInManagerService));
            //_signInManager = signInManagerService.GetSignInManager();

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

            var provider = _services.BuildServiceProvider();

            var userManagerService = (IUserManagerService)Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance(provider, typeof(UserManagerService));
            return userManagerService.GetUserManager();

            //see comments in Configure
            //return _userManager;
        }

        /// <summary>
        /// Returns a configured sign in manager
        /// </summary>
        /// <returns></returns>
        public static SignInManager<MapHiveIdentityUser> GetSignInManager()
        {
            if (!Configured)
                throw new InvalidOperationException($"In order to user {nameof(UserManagerUtils)} you need to call {nameof(UserManagerUtils)}.{nameof(UserManagerUtils.Configure)} first!");

            var provider = _services.BuildServiceProvider();

            var signInManagerService = (ISignInManagerService)Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance(provider, typeof(SignInManagerService));
            return signInManagerService.GetSignInManager();

            //see comments in Configure
            //return _signInManager;
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
