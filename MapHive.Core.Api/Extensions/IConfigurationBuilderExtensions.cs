using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace MapHive.Core.Api.Extensions
{

    public static class IConfigurationBuilderExtensions
    {
        /// <summary>
        /// Configures per env config input
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IConfigurationBuilder ConfigureConfigSources(this IConfigurationBuilder config)
        {
            config.AddJsonFile("appsettings.json", optional: true);
            config.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true);

            return config;
        }
    }

}
