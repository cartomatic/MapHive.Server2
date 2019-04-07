using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.Identity.DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MapHive.Core.Identity.DataModel
{
#pragma warning disable 1591
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
