using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MapHive.Core.Api.Extensions;
using Serilog;
using Serilog.Events;


namespace MapHive.Core.Api
{
#pragma warning disable 1591
    public class Program
    {
        public static int Main(string[] args)
        {
            //make sure to set the current dir to app's dir rather than worker process dir:
            //https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/aspnet-core-module?view=aspnetcore-2.2
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            MapHive.Core.Api.Logging.SerilogConfiguration.Configure();

            try
            {
                Log.Information("App start.");

                BuildWebHost(args).Run();

                Log.Information("App shut down.");

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return -1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            new WebHostBuilder()
                .UseSerilog()
                .UseKestrel()
                .ConfigureKestrel((context, options) => { })
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
            config.ConfigureConfigSources();
        }
    }
}
