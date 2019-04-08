using System;
using System.Collections.Generic;
using System.Text;
using MapHive.Core.Api.Extensions;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace MapHive.Core.Api.Logging
{
    /// <summary>
    /// Serilog configuration utils
    /// </summary>
    public class SerilogConfiguration
    {
        /// <summary>
        /// Configures serilog in a generic way for maphive apis
        /// </summary>
        public static void Configure()
        {
            var config = new ConfigurationBuilder()
                .ConfigureConfigSources()
                .Build();

            var logLvls = config.GetSection("SerilogConfiguration").Get<Dictionary<string, LogEventLevel?>>();

            Log.Logger = new LoggerConfiguration()

                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)

                .Enrich.WithProperty("App", "MapHive.Api.Core")
                .WriteTo.File(
                    "_logs\\log.txt",
                    restrictedToMinimumLevel: logLvls != null && logLvls.ContainsKey(nameof(Serilog.Sinks.File)) ? logLvls[nameof(Serilog.Sinks.File)] ?? LogEventLevel.Warning : LogEventLevel.Warning, rollingInterval: RollingInterval.Day, flushToDiskInterval: TimeSpan.FromMinutes(5)
                )
                .WriteTo.LiterateConsole(
                    restrictedToMinimumLevel: logLvls != null && logLvls.ContainsKey(nameof(Serilog.LoggerConfigurationLiterateExtensions.LiterateConsole)) ? logLvls[nameof(Serilog.LoggerConfigurationLiterateExtensions.LiterateConsole)] ?? LogEventLevel.Verbose : LogEventLevel.Verbose
                )
                .CreateLogger();
        }
    }
}
