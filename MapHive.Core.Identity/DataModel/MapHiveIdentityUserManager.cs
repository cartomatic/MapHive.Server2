using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MapHive.Core.Identity.DataModel
{

    public class MapHiveIdentityUserManager : UserManager<MapHiveIdentityUser>
    {
        /// <inheritdoc />
        public MapHiveIdentityUserManager(IUserStore<MapHiveIdentityUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<MapHiveIdentityUser> passwordHasher, IEnumerable<IUserValidator<MapHiveIdentityUser>> userValidators, IEnumerable<IPasswordValidator<MapHiveIdentityUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<MapHiveIdentityUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }
    }
}