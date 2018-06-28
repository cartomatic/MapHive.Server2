using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Data;
using Cartomatic.Utils.Ef;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MapHive.Identity
{
    public class UserManagerUtils
    {
        private static UserManager<MapHiveIdentityUser> _userManager;

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

            services.AddScoped<UserStore<MapHiveIdentityUser, MapHiveIdentityRole, MapHiveIdentityDbContext, Guid, MapHiveIdentityUserClaim, MapHiveIdentityUserRole, MapHiveIdentityUserLogin, MapHiveIdentityUserToken, MapHiveIdentityRoleClaim>, MapHiveIdentityUserStore>();
            services.AddScoped<UserManager<MapHiveIdentityUser>, MapHiveIdentityUserManager>();
            services.AddScoped<RoleManager<MapHiveIdentityRole>, MapHiveIdentityRoleManager>();
            services.AddScoped<SignInManager<MapHiveIdentityUser>, MapHiveIdentitySignInManager>();
            services.AddScoped<RoleStore<MapHiveIdentityRole, MapHiveIdentityDbContext, Guid, MapHiveIdentityUserRole, MapHiveIdentityRoleClaim>, MapHiveIdentityRoleStore>();
            //services.AddScoped<IEmailSender, AuthMessageSender>();
            //services.AddScoped<ISmsSender, AuthMessageSender>();

            // Authentification
            services.AddIdentity<MapHiveIdentityUser, IdentityRole>(opt =>
                {
                    //TODO - make setup customisable!!!
                    // Configure identity options
                    opt.Password.RequireDigit = false;
                    opt.Password.RequireLowercase = false;
                    opt.Password.RequireUppercase = false;
                    opt.Password.RequireNonAlphanumeric = false;
                    opt.Password.RequiredLength = 6;
                    opt.User.RequireUniqueEmail = true;
                })
                .AddUserStore<MapHiveIdentityUserStore>()
                .AddUserManager<MapHiveIdentityUserManager>()
                .AddRoleStore<MapHiveIdentityRoleStore>()
                //.AddRoleManager<MapHiveIdentityRoleManager>()
                .AddSignInManager<MapHiveIdentitySignInManager>()
                
                // You **cannot** use .AddEntityFrameworkStores() when you customize everything
                //more here: https://github.com/aspnet/Identity/issues/1082
                //.AddEntityFrameworkStores<MapHiveIdentityDbContext>()
                .AddDefaultTokenProviders();


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
    }
}
