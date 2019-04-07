using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace MapHive.Core.Api
{
#pragma warning disable 1591
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration(AddAppConfiguration)
                .ConfigureLogging(
                    (hostingContext, logging) => { })
                .UseIISIntegration()
                .UseDefaultServiceProvider((context, options) => { })
                .UseStartup<Startup>()
                .Build();

        public static void AddAppConfiguration(WebHostBuilderContext hostingContext, IConfigurationBuilder config)
        {
            config.AddJsonFile("appsettings.json", optional: true);
            config.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true);
        }
    }
}
