using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace MapHive.Core.IdentityServer.SerializableConfig
{
    public class IdSrvTokenBearerOpts
    {
        /// <summary>
        /// IdSrv authority - where the server is located
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// Name of API resource 
        /// </summary>
        public string ApiName { get; set; }

        /// <summary>
        /// Client secret
        /// </summary>
        public string ApiSecret { get; set; }

        /// <summary>
        /// Default ctor needed for serialisation
        /// </summary>
        public IdSrvTokenBearerOpts()
        {
        }

        /// <summary>
        /// Extracts IdSrvTokenBearerOpts from app settings via using the specified key; vakue of this key should be a json serialised IdSrvTokenBearerOpts object
        /// </summary>
        /// <param name="cfgKey"></param>
        public IdSrvTokenBearerOpts(string cfgKey)
        {
            var cfg = 
                Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig()
                    .GetSection(cfgKey)
                    .Get<IdSrvTokenBearerOpts>();

            Authority = cfg?.Authority;
            ApiName = cfg?.ApiName;
            ApiSecret = cfg?.ApiSecret;
        }

        /// <summary>
        /// Creates an instance using the cfg supplied as a dictionary; dictionary keys should be equivalent to the oject properties
        /// </summary>
        /// <param name="cfg"></param>
        public IdSrvTokenBearerOpts(Dictionary<string, string> cfg)
        {
            Authority = cfg.ContainsKey(nameof(Authority)) ? cfg[nameof(Authority)] : null;
            ApiName = cfg.ContainsKey(nameof(ApiName)) ? cfg[nameof(ApiName)] : null;
            ApiSecret = cfg.ContainsKey(nameof(ApiSecret)) ? cfg[nameof(ApiSecret)] : null;
        }

        /// <summary>
        /// Expects the IdSrvTokenBearerOpts configuration to be available via app settings under a "IdSrvTokenBearerOpts" key; value of this key should be 
        /// a json serialised IdSrvTokenBearerOpts object
        /// </summary>
        /// <returns></returns>
        public static IdSrvTokenBearerOpts InitDefault(bool silent = true)
        {
            return new IdSrvTokenBearerOpts("IdSrvTokenBearerOpts");
        }
    }
}
