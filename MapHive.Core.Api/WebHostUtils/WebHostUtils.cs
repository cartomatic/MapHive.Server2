using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using MapHive.Core.Api.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace MapHive.Core.Api
{

    /// <summary>
    /// maphive specific web host utils
    /// </summary>
    public class WebHostUtils
    {
        /// <summary>
        /// Provides generic MapHive web host builder and runner
        /// </summary>
        /// <typeparam name="TStartup">Startup class</typeparam>
        /// <param name="args">args as received by the main program of aspnet core app</param>
        /// <returns></returns>
        public static int BuildAndRunWebHost<TStartup>(string[] args)
            where TStartup: class
        {
            //make sure to set the current dir to app's dir rather than worker process dir:
            //https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/aspnet-core-module?view=aspnetcore-2.2
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            MapHive.Core.Api.Logging.SerilogConfiguration.Configure();

            try
            {
                Log.Information("App start.");

                BuildWebHost<TStartup>(args).Run();

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

        public static IWebHost BuildWebHost<TStartup>(string[] args) 
            where TStartup: class
            =>
            new WebHostBuilder()
                .UseSerilog()

                .UseKestrel()
                .ConfigureKestrel((context, opts) =>
                {
                    opts.AddServerHeader = false;
                    opts.Limits.MaxRequestBodySize = null;
                    opts.Limits.MaxResponseBufferSize = null;
                }) //needed for IIS in-process
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration(AddAppConfiguration)
                .ConfigureLogging(
                    (hostingContext, logging) => { })

                .UseDefaultServiceProvider((context, options) => { })
                .UseStartup<TStartup>()

                .UseIIS() // needs to be there for iis in process integration
                .UseIISIntegration() // needs to be there too for iis in process integration

                .Build();

        public static void AddAppConfiguration(WebHostBuilderContext hostingContext, IConfigurationBuilder config)
        {
            config.ConfigureConfigSources();
        }
    }

}
