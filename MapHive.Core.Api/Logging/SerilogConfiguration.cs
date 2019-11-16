using System;
using System.Collections.Generic;
using System.Text;
using Cartomatic.Utils;
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
        class SerilogCfg
        {
            public string AppName { get; set; }
            public Dictionary<string, LogEventLevel?> Sinks { get; set; }
        }

        /// <summary>
        /// Configures serilog in a generic way for maphive apis
        /// </summary>
        public static void Configure()
        {
            var config = new ConfigurationBuilder()
                .ConfigureConfigSources()
                .Build();

            var logCfg = config.GetSection("SerilogConfiguration").Get<SerilogCfg>();

            Log.Logger = new LoggerConfiguration()

                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)

                .Enrich.WithProperty("App", string.IsNullOrEmpty(logCfg.AppName) ? "Uknown MapHiveApi" : logCfg.AppName)
                
                .WriteTo.File(
                    $"_logs\\{DateTime.Now:yyyyMMdd}.serilog.log".SolvePath(),
                    restrictedToMinimumLevel: logCfg?.Sinks != null && logCfg.Sinks.ContainsKey(nameof(Serilog.Sinks.File)) ? logCfg.Sinks[nameof(Serilog.Sinks.File)] ?? LogEventLevel.Warning : LogEventLevel.Warning, rollingInterval: RollingInterval.Day, flushToDiskInterval: TimeSpan.FromMinutes(5)
                )
                .WriteTo.LiterateConsole(
                    restrictedToMinimumLevel: logCfg?.Sinks != null && logCfg.Sinks.ContainsKey(nameof(Serilog.LoggerConfigurationLiterateExtensions.LiterateConsole)) ? logCfg.Sinks[nameof(Serilog.LoggerConfigurationLiterateExtensions.LiterateConsole)] ?? LogEventLevel.Verbose : LogEventLevel.Verbose
                )
                .CreateLogger();
        }
    }
}
