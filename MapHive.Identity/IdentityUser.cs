using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MapHive.Identity
{
    //Note: not only user, but more customizations, as they are required in order to hook into user manager...
    //not very elegant, but maybe will manage to make it better later... ;)
    //more details:
    //https://github.com/aspnet/Identity/issues/1082
    //https://www.codepoc.io/blog/net-core/5291/create-aspnetcore-identity-users-using-console-application

    public class MapHiveIdentityUser : IdentityUser<Guid>
    {
    }

    public class MapHiveIdentityRole : IdentityRole<Guid>
    {
    }

    public class MapHiveIdentityUserRole : IdentityUserRole<Guid>
    {
    }

    public class MapHiveIdentityRoleClaim : IdentityRoleClaim<Guid>
    {
    }

    public class MapHiveIdentityUserClaim : IdentityUserClaim<Guid>
    {
    }

    public class MapHiveIdentityUserLogin : IdentityUserLogin<Guid>
    {
    }

    public class MapHiveIdentityUserToken : IdentityUserToken<Guid>
    {
    }

    

    public class MapHiveIdentityRoleManager : RoleManager<MapHiveIdentityRole>
    {
        /// <inheritdoc />
        public MapHiveIdentityRoleManager(IRoleStore<MapHiveIdentityRole> store, IEnumerable<IRoleValidator<MapHiveIdentityRole>> roleValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<MapHiveIdentityRole>> logger) : base(store, roleValidators, keyNormalizer, errors, logger)
        {
        }
    }

    public class MapHiveIdentityRoleStore : RoleStore<MapHiveIdentityRole, MapHiveIdentityDbContext, Guid, MapHiveIdentityUserRole, MapHiveIdentityRoleClaim>
    {
        /// <inheritdoc />
        public MapHiveIdentityRoleStore(MapHiveIdentityDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
        }

        /// <inheritdoc />
        protected override MapHiveIdentityRoleClaim CreateRoleClaim(MapHiveIdentityRole role, Claim claim)
        {
            return new MapHiveIdentityRoleClaim
            {
                RoleId = role.Id,
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            };
        }
    }

    public class MapHiveIdentitySignInManager : SignInManager<MapHiveIdentityUser>
    {
        /// <inheritdoc />
        public MapHiveIdentitySignInManager(UserManager<MapHiveIdentityUser> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<MapHiveIdentityUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<MapHiveIdentityUser>> logger, IAuthenticationSchemeProvider schemes) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes)
        {
        }
    }

    public class MapHiveIdentityUserManager : UserManager<MapHiveIdentityUser>
    {
        /// <inheritdoc />
        public MapHiveIdentityUserManager(IUserStore<MapHiveIdentityUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<MapHiveIdentityUser> passwordHasher, IEnumerable<IUserValidator<MapHiveIdentityUser>> userValidators, IEnumerable<IPasswordValidator<MapHiveIdentityUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<MapHiveIdentityUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }
    }

    public class MapHiveIdentityUserStore : UserStore<MapHiveIdentityUser, MapHiveIdentityRole, MapHiveIdentityDbContext, Guid, MapHiveIdentityUserClaim, MapHiveIdentityUserRole, MapHiveIdentityUserLogin, MapHiveIdentityUserToken, MapHiveIdentityRoleClaim>
    {
        /// <inheritdoc />
        public MapHiveIdentityUserStore(MapHiveIdentityDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
        }

        /// <inheritdoc />
        protected override MapHiveIdentityUserClaim CreateUserClaim(MapHiveIdentityUser user, Claim claim)
        {
            var userClaim = new MapHiveIdentityUserClaim { UserId = user.Id };
            userClaim.InitializeFromClaim(claim);
            return userClaim;
        }

        protected override MapHiveIdentityUserLogin CreateUserLogin(MapHiveIdentityUser user, UserLoginInfo login)
        {
            return new MapHiveIdentityUserLogin
            {
                UserId = user.Id,
                ProviderKey = login.ProviderKey,
                LoginProvider = login.LoginProvider,
                ProviderDisplayName = login.ProviderDisplayName
            };
        }

        protected override MapHiveIdentityUserRole CreateUserRole(MapHiveIdentityUser user, MapHiveIdentityRole role)
        {
            return new MapHiveIdentityUserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            };
        }

        protected override MapHiveIdentityUserToken CreateUserToken(MapHiveIdentityUser user, string loginProvider, string name, string value)
        {
            return new MapHiveIdentityUserToken
            {
                UserId = user.Id,
                LoginProvider = loginProvider,
                Name = name,
                Value = value
            };
        }
    }

}
