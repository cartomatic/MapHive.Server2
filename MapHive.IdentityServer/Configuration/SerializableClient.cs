using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Cartomatic.Utils.Dto;
using IdentityServer4.Models;

namespace MapHive.IdentityServer.Configuration
{
    /// <summary>
    /// SerializableClient hides base interface type properties of Client with their implementation, so can deserialize object
    /// </summary>
    public class SerializableClient : Client
    {
        /// <summary>
        /// Specifies which external IdPs can be used with this client (if list is empty
        /// all IdPs are allowed). Defaults to empty.
        /// </summary>
        public new List<string> IdentityProviderRestrictions { get; set; }

        /// <summary>
        /// Allows settings claims for the client (will be included in the access token).
        /// </summary>
        public new List<Claim> Claims { get; set; }

        /// <summary>
        /// Specifies the api scopes that the client is allowed to request. If empty, the
        /// client can't access any scope
        /// </summary>
        public new List<string> AllowedScopes { get; set; }

        /// <summary>
        /// Gets or sets the custom properties for the client.
        /// </summary>
        public new Dictionary<string, string> Properties { get; set; }

        /// <summary>
        /// Client secrets - only relevant for flows that require a secret
        /// </summary>
        public new List<Secret> ClientSecrets { get; set; }

        /// <summary>
        /// Gets or sets the allowed CORS origins for JavaScript clients.
        /// </summary>
        public new List<string> AllowedCorsOrigins { get; set; }

        /// <summary>
        /// Specifies the allowed grant types (legal combinations of AuthorizationCode, Implicit,
        /// Hybrid, ResourceOwner, ClientCredentials).
        /// </summary>
        public new List<string> AllowedGrantTypes { get; set; }

        /// <summary>
        /// Specifies allowed URIs to return tokens or authorization codes to
        /// </summary>
        public new List<string> RedirectUris { get; set; }

        /// <summary>
        /// Specifies allowed URIs to redirect to after logout
        /// </summary>
        public new List<string> PostLogoutRedirectUris { get; set; }

        public Client ToClient()
        {
            var client = this.CopyPublicPropertiesToNew<Client>();

            //hidden properties need to be copied explicitly as dto utils will not matche them on type!
            client.IdentityProviderRestrictions = IdentityProviderRestrictions;
            client.Claims = Claims;
            client.AllowedScopes = AllowedScopes;
            client.Properties = Properties;
            client.ClientSecrets = ClientSecrets.Select(cs => new Secret(cs.Value.Sha256(), cs.Description, cs.Expiration)).ToList();
            client.AllowedCorsOrigins = AllowedCorsOrigins;
            client.AllowedGrantTypes = AllowedGrantTypes;
            client.RedirectUris = RedirectUris;
            client.PostLogoutRedirectUris = PostLogoutRedirectUris;

            return client;
        }
    }
}
