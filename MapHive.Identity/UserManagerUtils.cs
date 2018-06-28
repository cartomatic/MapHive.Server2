using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Data;
using Cartomatic.Utils.Ef;
using Microsoft.AspNetCore.Identity;
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
                .AddEntityFrameworkStores<MapHiveIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IUserManagerService, UserManagerService>();

            // Build the IoC from the service collection
            var provider = services.BuildServiceProvider();

            var userManagerService = provider.GetService<UserManagerService>();

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
