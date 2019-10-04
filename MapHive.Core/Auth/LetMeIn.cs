using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Validators;
using IdentityModel.Client;
using MapHive.Core.IdentityServer.SerializableConfig;
using Microsoft.Extensions.Http.Logging;

namespace MapHive.Core
{
    public partial class Auth
    {
        /// <summary>
        /// Authenticates user based on his email and password
        /// </summary>
        /// <param name="email"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static async Task<AuthOutput> LetMeInAsync(string email, string pass)
        {
            return AuthOutput.FromTokenResponse(
                await AuthenticateUserAsync(email, pass)    
            );
        }

        /// <summary>
        /// Authenticates a user against the IdSrv
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private static async Task<TokenResponse> AuthenticateUserAsync(string email, string password)
        {
            var idSrvTokenClientOpts = IdSrvTokenClientOpts.InitDefault();

            //netcreapp2.2
            //var tokenClient = new TokenClient(
            //     $"{idSrvTokenClientOpts.Authority}/connect/token",
            //    idSrvTokenClientOpts.ClientId,
            //    idSrvTokenClientOpts.ClientSecret
            //);

            //try
            //{
            //    return
            //        await
            //            tokenClient.RequestResourceOwnerPasswordAsync(email, password,
            //                idSrvTokenClientOpts.RequiredScopes);
            //}
            //catch (Exception ex)
            //{

            //    return null;
            //}


            //netcore 3.0
            try
            {
                var client = new HttpClient();

                return await client.RequestPasswordTokenAsync(new PasswordTokenRequest
                    {
                        Address = $"{idSrvTokenClientOpts.Authority}/connect/token",

                        ClientId = idSrvTokenClientOpts.ClientId,
                        ClientSecret = idSrvTokenClientOpts.ClientSecret,
                        Scope = idSrvTokenClientOpts.RequiredScopes,

                        UserName = email,
                        Password = password
                    });
            }
            catch (Exception ex)
            {
                return null;
            }



        }
    }
}
