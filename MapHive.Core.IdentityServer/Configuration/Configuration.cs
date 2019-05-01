using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;

namespace MapHive.Core.IdentityServer
{
    /// <summary>
    /// Configuration 
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Gets configured identity resources
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            //docu: http://docs.identityserver.io/en/release/topics/resources.html

            return new List<IdentityResource>
            {
                //open id is the minimum - subject id (user id) must be emited
                new IdentityResources.OpenId()
            };
        }

        private static IConfiguration Config { get; set; }

        private static IConfiguration GetConfig()
        {
            if (Config == null)
            {
                Config = Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig();
            }

            return Config;
        }

        /// <summary>
        /// Gets configured API resources
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApiResources()
        {
            var apiResources = 
                GetConfig().GetSection("ApiResources")
                    .Get<List<SerializableConfig.ApiResource>>()
                    .Select(rs=>rs.ToApiResource())
                    .ToList();

            return apiResources;
        }

        /// <summary>
        /// Gets configured api clients
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetApiClients()
        {
            var clients = GetConfig().GetSection("Clients")
                .Get<List<SerializableConfig.Client>>()
                .Select(c=>c.ToClient())
                .ToList();

            return clients;
        }
    }
}
