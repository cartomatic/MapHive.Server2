using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MapHive.Core.Identity.DataModel
{

    public class MapHiveIdentitySignInManager : SignInManager<MapHiveIdentityUser>
    {
        /// <inheritdoc />
        public MapHiveIdentitySignInManager(UserManager<MapHiveIdentityUser> userManager,
            IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<MapHiveIdentityUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<MapHiveIdentityUser>> logger,
            IAuthenticationSchemeProvider schemes, IUserConfirmation<MapHiveIdentityUser> confirmation) : base(userManager, contextAccessor, claimsFactory, optionsAccessor,
            logger, schemes, confirmation)
        {
        }
    }
}