using System;
using System.Security.Claims;
using MapHive.Core.Identity.DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MapHive.Core.Identity.DataModel
{
#pragma warning disable 1591
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
}