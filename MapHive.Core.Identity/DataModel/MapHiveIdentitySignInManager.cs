﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MapHive.Core.Identity.DataModel
{
#pragma warning disable 1591
    public class MapHiveIdentitySignInManager : SignInManager<MapHiveIdentityUser>
    {
        /// <inheritdoc />
        public MapHiveIdentitySignInManager(UserManager<MapHiveIdentityUser> userManager,
            IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<MapHiveIdentityUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<MapHiveIdentityUser>> logger,
            IAuthenticationSchemeProvider schemes) : base(userManager, contextAccessor, claimsFactory, optionsAccessor,
            logger, schemes)
        {
        }
    }
}