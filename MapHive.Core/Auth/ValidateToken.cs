using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using IdentityServer4.Models;
using MapHive.Core.IdentityServer.SerializableConfig;
using RestSharp;

namespace MapHive.Core
{
    public partial class Auth
    {
        public static async Task<AuthOutput> ValidateTokenAsync(string accessToken)
        {
            var idSrvTokenBearerOpts = IdSrvTokenBearerOpts.InitDefault();

            //using IdentityModel for token introspection
            //it provides anice functionality extension set over http client

            using (var client = new HttpClient())
            {
                var response = await client.IntrospectTokenAsync(new TokenIntrospectionRequest
                {
                    Address = $"{idSrvTokenBearerOpts.Authority}/connect/introspect",
                    ClientId = idSrvTokenBearerOpts.ApiName,
                    ClientSecret = idSrvTokenBearerOpts.ApiSecret,

                    Token = accessToken
                });

                return new AuthOutput
                {
                    Success = !response.IsError && response.IsActive,
                    AccessToken = accessToken,

                    //Note: expiration in seconds since epoch.
                    //Code below should give time in UTC 
                    AccessTokenExpirationTimeUtc = !response.IsError && response.IsActive
                        ? new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc).AddSeconds(
                            long.Parse(response.Claims.FirstOrDefault(x => x.Type == "exp")?.Value ?? "0"))
                        : (DateTime?) null
                };
            }
        }
    }
}
