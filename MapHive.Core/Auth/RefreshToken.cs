using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using MapHive.Core.IdentityServer.SerializableConfig;

namespace MapHive.Core
{
    public partial class Auth
    {
        /// <summary>
        /// Refreshes auth tokens - access token + refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public static async Task<AuthOutput> RefreshTokensAsync(string refreshToken)
        {
            return AuthOutput.FromTokenResponse(
                await RequestRefreshTokenAsync(refreshToken)
            );
        }

        /// <summary>
        /// Refreshes auth tokens - auth token + refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        private static async Task<TokenResponse> RequestRefreshTokenAsync(string refreshToken)
        {
            var idSrvTokenClientOpts = IdSrvTokenClientOpts.InitDefault();

            //netcoreapp2.2
            //var tokenClient = new TokenClient(
            //    $"{idSrvTokenClientOpts.Authority}/connect/token",
            //    idSrvTokenClientOpts.ClientId,
            //    idSrvTokenClientOpts.ClientSecret
            //);

            //try
            //{
            //    return await tokenClient.RequestRefreshTokenAsync(refreshToken);
            //}
            //catch
            //{
            //    return null;
            //}


            try
            {
                var client = new HttpClient();

                return await client.RequestRefreshTokenAsync(new RefreshTokenRequest
                {
                    Address = $"{idSrvTokenClientOpts.Authority}/connect/token",

                    ClientId = idSrvTokenClientOpts.ClientId,
                    ClientSecret = idSrvTokenClientOpts.ClientSecret,

                    RefreshToken = refreshToken
                });
            }
            catch
            {
                return null;
            }
        }
    }
}
