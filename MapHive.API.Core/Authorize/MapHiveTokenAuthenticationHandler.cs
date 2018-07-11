using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using MapHive.Api.Core.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MapHive.Api.Core.Authorize
{
    public class MapHiveTokenAuthenticationOptions : AuthenticationSchemeOptions
    {
    }

    public class MapHiveTokenAuthenticationHandler : AuthenticationHandler<MapHiveTokenAuthenticationOptions>
    {
        public const string Scheme = "MapHiveApiPickLock";

        private readonly ILogger _logger;

        /// <inheritdoc />
        public MapHiveTokenAuthenticationHandler(IOptionsMonitor<MapHiveTokenAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
#if DEBUG
            _logger = logger.CreateLogger<MapHiveTokenAuthenticationHandler>();
#endif
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authToken = Context.ExtractAuthHeader();
            if (authToken?.scheme == Scheme)
            {
                //if api token presence has been detected via middleware
                //it should be now present in the request context properties dict
                var token = authToken?.parameter ?? string.Empty;
                var tokenIsAuthorized = (bool) Context.Items[token];

                if (tokenIsAuthorized)
                {
                    //impersonate token!
                    if (Guid.TryParse(token, out var parsedToken))
                    {
                        Cartomatic.Utils.Identity.ImpersonateUser(parsedToken);
                        Cartomatic.Utils.Identity.ImpersonateUserViaHttpContext(parsedToken);
                    }

                    return AuthenticateResult.Success(
                        new AuthenticationTicket(
                            Cartomatic.Utils.Identity.GetBasicClaimsPrincipal(parsedToken, Scheme),
                            Scheme
                        )
                    );
                }
                else
                {
                    return AuthenticateResult.Fail("Invalid token.");
                }
            }

            return AuthenticateResult.NoResult();
        }

        ///// <inheritdoc />
        //protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        //{
        //    await Context.ChallengeAsync("Bearer");
        //}
    }
}
