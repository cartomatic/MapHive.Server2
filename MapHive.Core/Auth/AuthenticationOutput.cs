﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace MapHive.Core
{
    public partial class Auth
    {
        public class AuthOutput
        {
            /// <summary>
            /// whether or not the auth request was successful
            /// </summary>
            public bool Success { get; set; }

            /// <summary>
            /// Authentication scheme to be used when communicating using the supplied tokens 
            /// </summary>
            public string Scheme { get; set; } = "Bearer"; //Bearer is the default scheme!

            /// <summary>
            /// Access token
            /// </summary>
            public string AccessToken { get; set; }

            /// <summary>
            /// Refresh token
            /// </summary>
            public string RefreshToken { get; set; }

            /// <summary>
            /// Access token expiration coordinated universal time (UTC)
            /// </summary>
            public DateTime? AccessTokenExpirationTimeUtc { get; set; }

            public static AuthOutput FromTokenResponse(TokenResponse token)
            {
                if(token == null)
                    return new AuthOutput();

                return new AuthOutput()
                {
                    Success = !token.IsError,
                    AccessToken = token.AccessToken,
                    RefreshToken = token.RefreshToken,
                    AccessTokenExpirationTimeUtc = !token.IsError
                        ? DateTime.Now.AddSeconds(token.ExpiresIn).ToUniversalTime()
                        : (DateTime?) null
                };
            }
        }
    }
}
