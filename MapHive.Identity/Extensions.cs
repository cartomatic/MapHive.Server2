using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MapHive.Identity
{
    public static class Extensions
    {
        public static IdentityOptions ConfigureIdentityOptions(this IdentityOptions opts)
        {
            var cfg = Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig();

            var passOpts = new PasswordOptions();
            var userOpts = new UserOptions();
            var lockoutOpts = new LockoutOptions();
            var signinOpts = new SignInOptions();

            cfg.GetSection("IdentityOptions::PasswordOptions").Bind(passOpts);
            cfg.GetSection("IdentityOptions::UserOptions").Bind(userOpts);
            cfg.GetSection("IdentityOptions::LockoutOptions").Bind(lockoutOpts);
            cfg.GetSection("IdentityOptions::SignInOptions").Bind(signinOpts);

            opts.Password = passOpts;
            opts.SignIn = signinOpts;
            opts.User = userOpts;
            opts.Lockout = lockoutOpts;

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
