using System;
using System.Collections.Generic;
using System.Text;
using MapHive.Core.Identity.DataModel;
using MapHive.Core.Identity.DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MapHive.Core.Identity
{

    public static class Extensions
    {
        /// <summary>
        /// Configures identity opts
        /// </summary>
        /// <param name="opts"></param>
        /// <returns></returns>
        public static IdentityOptions ConfigureIdentityOptions(this IdentityOptions opts)
        {
            var cfg = Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig();

            opts.Password = cfg.GetSection("IdentityOptions:PasswordOptions").Get<PasswordOptions>() ?? new PasswordOptions();
            opts.SignIn = cfg.GetSection("IdentityOptions:SignInOptions").Get<SignInOptions>() ?? new SignInOptions();
            opts.User = cfg.GetSection("IdentityOptions:UserOptions").Get<UserOptions>() ?? new UserOptions();
            opts.Lockout = cfg.GetSection("IdentityOptions:LockoutOptions").Get<LockoutOptions>() ?? new LockoutOptions();

            return opts;
        }

        /// <summary>
        /// adds identity scoped services
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureIdentityScopedServices(this IServiceCollection services)
        {
            services.AddScoped<UserStore<MapHiveIdentityUser, MapHiveIdentityRole, MapHiveIdentityDbContext, Guid, MapHiveIdentityUserClaim, MapHiveIdentityUserRole, MapHiveIdentityUserLogin, MapHiveIdentityUserToken, MapHiveIdentityRoleClaim>, MapHiveIdentityUserStore>();
            services.AddScoped<UserManager<MapHiveIdentityUser>, MapHiveIdentityUserManager>();
            services.AddScoped<RoleManager<MapHiveIdentityRole>, MapHiveIdentityRoleManager>();
            services.AddScoped<SignInManager<MapHiveIdentityUser>, MapHiveIdentitySignInManager>();
            services.AddScoped<RoleStore<MapHiveIdentityRole, MapHiveIdentityDbContext, Guid, MapHiveIdentityUserRole, MapHiveIdentityRoleClaim>, MapHiveIdentityRoleStore>();
            //services.AddScoped<IEmailSender, AuthMessageSender>();
            //services.AddScoped<ISmsSender, AuthMessageSender>();
        }

        /// <summary>
        /// Adds and configures the identity system
        /// </summary>
        /// <param name="services"></param>
        public static IdentityBuilder AddIdentity(this IServiceCollection services)
        {
            // Authentification
            return services.AddIdentity<MapHiveIdentityUser, MapHiveIdentityRole>(opts =>
                    opts.ConfigureIdentityOptions())

                .AddUserStore<MapHiveIdentityUserStore>()
                .AddUserManager<MapHiveIdentityUserManager>()
                .AddRoleStore<MapHiveIdentityRoleStore>()
                //.AddRoleManager<MapHiveIdentityRoleManager>()
                .AddSignInManager<MapHiveIdentitySignInManager>()

                // You **cannot** use .AddEntityFrameworkStores() when you customize everything
                //more here: https://github.com/aspnet/Identity/issues/1082
                //.AddEntityFrameworkStores<MapHiveIdentityDbContext>()
                .AddDefaultTokenProviders();
        }
    }
}
