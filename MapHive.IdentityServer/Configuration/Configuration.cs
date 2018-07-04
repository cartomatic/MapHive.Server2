using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;

namespace MapHive.IdentityServer
{
    public class Configuration
    {
        /// <summary>
        /// Gets confogured identity resources
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
            var apiResources = new List<ApiResource>();
            GetConfig().GetSection("ApiResources").Bind(apiResources);

            return apiResources;
        }

        /// <summary>
        /// Gets confogured api clients
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetApiClients()
        {
            var clients = new List<Client>();
            GetConfig().GetSection("Clients").Bind(clients);

            return clients;
        }
    }
}
