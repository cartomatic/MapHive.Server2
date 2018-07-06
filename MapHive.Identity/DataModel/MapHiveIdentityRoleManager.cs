using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace MapHive.Identity.DataModel
{
    public class MapHiveIdentityRoleManager : RoleManager<MapHiveIdentityRole>
    {
        /// <inheritdoc />
        public MapHiveIdentityRoleManager(IRoleStore<MapHiveIdentityRole> store, IEnumerable<IRoleValidator<MapHiveIdentityRole>> roleValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<MapHiveIdentityRole>> logger) : base(store, roleValidators, keyNormalizer, errors, logger)
        {
        }
    }
}