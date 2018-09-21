using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MapHive.Core.Configuration
{
    /// <summary>
    /// Specifies the input for checking the allowed usage for a client.
    /// </summary>
    public class UserConfigurationQuery
    {
        /// <summary>
        /// When present specifies a user context - the configuration is pulled for a real user
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// When present, specifies an access token; this is an equivalent of a user and data returned take the same form 
        /// </summary>
        public Guid? TokenId { get; set; }

        /// <summary>
        /// Refferer of the original caller. This is tested to see if a token request has come from an allowed client
        /// </summary>
        public string Referrer { get; set; }

        /// <summary>
        /// Comma separated short names of the applications to scope the configuration retrieval to.
        /// <para />
        /// an org may be configured to just use one or more apis and one or more ui apps; this is to allow for retrieving a cfg for all the required stuff; thanks to this an api can provide a unified single entry point and bypass other api calls on behalf of a user
        /// </summary>
        public string AppNames { get; set; }

        /// <summary>
        /// Ip of the original caller making an initial config request
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Organization id for which the token tries to obtain credentials. this setting makes sense only for tokens and also when token is a master token
        /// </summary>
        public Guid? OrganizationId { get; set; }


        private string RawCacheKey => $"{UserId}_{TokenId}_{Referrer}_{AppNames}_{Ip}_{OrganizationId}";

        private (string rawKey, string hashedKey) _cacheKey;

        /// <summary>
        /// returns a cache key to be used to cache this objcect
        /// </summary>
        public string CacheKey
        {
            get
            {
                if (_cacheKey.rawKey == RawCacheKey && string.IsNullOrEmpty(_cacheKey.hashedKey))
                    return _cacheKey.hashedKey;

                _cacheKey.rawKey = RawCacheKey;

                //get an array of bytes off the string
                var sb = System.Text.Encoding.UTF8.GetBytes(_cacheKey.rawKey);

                //container for hashed string
                byte[] ob;

                using (SHA256CryptoServiceProvider sha = new SHA256CryptoServiceProvider())
                {
                    ob = sha.ComputeHash(sb);
                }

                //remove the dashes and make string lowercase
                _cacheKey.hashedKey = BitConverter.ToString(ob).Replace("-", "").ToLower();

                return _cacheKey.hashedKey;
            }
        }
    }
}
