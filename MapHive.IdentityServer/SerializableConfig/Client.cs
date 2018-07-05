using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Cartomatic.Utils.Dto;
using IdentityServer4.Models;

namespace MapHive.IdentityServer.SerializableConfig
{
    /// <summary>
    /// Exposes some properties that seem to be configurable most often.
    /// Client is a complex obkect with stuff such as Claims, hence not inheriting from the original object but rather providing a facade
    /// based on http://docs.identityserver.io/en/release/topics/clients.html#defining-clients
    /// and https://github.com/cartomatic/MapHive.Identity/blob/master/IdentityServer/Configuration/SerialisableClient.cs
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Unique ID of the client
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Client display name(used for logging and consent screen)
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// URI to further information about client(used on consent screen)
        /// </summary>
        public string ClientUri { get; set; }

        /// <summary>
        /// URI to client logo(used on consent screen)
        /// </summary>
        public string LogoUri { get; set; }

        /// <summary>
        /// Client secrets - only relevant for flows that require a secret
        /// </summary>
        public List<Secret> ClientSecrets { get; set; }

        /// <summary>
        /// Specifies the api scopes that the client is allowed to request.If empty, the client can't access any scope
        /// </summary>
        public List<string> AllowedScopes { get; set; }

        /// <summary>
        /// Specifies the allowed grant types(legal combinations of AuthorizationCode, Implicit, Hybrid, ResourceOwner, ClientCredentials)
        /// </summary>
        public List<string> AllowedGrantTypes { get; set; }

        /// <summary>
        /// Controls whether access tokens are transmitted via the browser for this client
        /// (defaults to false). This can prevent accidental leakage of access tokens when
        /// multiple response types are allowed.
        /// </summary>
        public bool AllowAccessTokensViaBrowser { get; set; }

        /// <summary>
        /// Specifies allowed URIs to return tokens or authorization codes to
        /// </summary>
        public List<string> RedirectUris { get; set; }

        /// <summary>
        /// Specifies allowed URIs to redirect to after logout
        /// </summary>
        public List<string> PostLogoutRedirectUris { get; set; }

        /// <summary>
        /// Gets or sets the allowed CORS origins for JavaScript clients.
        /// </summary>
        public List<string> AllowedCorsOrigins { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether[allow offline access]. Defaults to false.
        /// </summary>
        public bool AllowOfflineAccess { get; set; }

        /// <summary>
        /// Specifies whether the access token is a reference token or a self contained JWT token (defaults to Jwt).
        /// </summary>
        public AccessTokenType AccessTokenType { get; set; }

        /// <summary>
        /// Lifetime of access token in seconds(defaults to 3600 seconds / 1 hour)
        /// </summary>
        public int AccessTokenLifetime { get; set; }

        /// <summary>
        /// Specifies if client is enabled(defaults to true)
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Specifies whether a consent screen is required(defaults to true)
        /// </summary>
        public bool RequireConsent { get; set; }

        public IdentityServer4.Models.Client ToClient()
        {
            return new IdentityServer4.Models.Client
            {
                ClientId = ClientId,
                ClientName = ClientName,
                ClientUri = ClientUri,
                LogoUri = LogoUri,
                ClientSecrets = ClientSecrets.Select(cs => new IdentityServer4.Models.Secret(cs.Value.Sha256(), cs.Description, cs.Expiration)).ToList(),
                AllowedScopes = AllowedScopes,
                AllowedGrantTypes = AllowedGrantTypes,
                AllowAccessTokensViaBrowser = AllowAccessTokensViaBrowser,
                RedirectUris = RedirectUris,
                PostLogoutRedirectUris = PostLogoutRedirectUris,
                AllowedCorsOrigins = AllowedCorsOrigins,
                AllowOfflineAccess = AllowOfflineAccess,
                AccessTokenType = AccessTokenType,
                AccessTokenLifetime = AccessTokenLifetime,
                Enabled = Enabled,
                RequireConsent = RequireConsent
            };
        }
    }
}
