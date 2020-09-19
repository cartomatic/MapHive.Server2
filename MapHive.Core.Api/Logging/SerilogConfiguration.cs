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
            var appName = $"[{(string.IsNullOrEmpty(logCfg.AppName) ? "Uknown MapHive API" : logCfg.AppName)}]";

            //https://github.com/serilog/serilog/wiki/Formatting-Output
            //var outputTpl = "{Timestamp:HH:mm:ss}\t[{Level:u3}]\t{App}\t{Message}\t{Exception}\t{Properties:j}{NewLine}";
            //var outputTpl = "{Timestamp:HH:mm:ss} [{Level:u3}] {App} {Message}{NewLine}{Exception}";
            var outputTpl = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";


            Log.Logger = new LoggerConfiguration()

                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.WithProperty("App", appName)
                .MinimumLevel.Verbose()
                .WriteTo.File(
                    $"_logs\\{DateTime.Now:yyyyMMdd}.serilog.log".SolvePath(),
                    restrictedToMinimumLevel: logCfg?.Sinks != null && logCfg.Sinks.ContainsKey(nameof(Serilog.Sinks.File)) ? logCfg.Sinks[nameof(Serilog.Sinks.File)] ?? LogEventLevel.Warning : LogEventLevel.Warning, rollingInterval: RollingInterval.Day, flushToDiskInterval: TimeSpan.FromMinutes(5),
                    outputTemplate: outputTpl
                )
                .WriteTo.LiterateConsole(
                    restrictedToMinimumLevel: logCfg?.Sinks != null && logCfg.Sinks.ContainsKey(nameof(Serilog.LoggerConfigurationLiterateExtensions.LiterateConsole)) ? logCfg.Sinks[nameof(Serilog.LoggerConfigurationLiterateExtensions.LiterateConsole)] ?? LogEventLevel.Verbose : LogEventLevel.Verbose,
                    outputTemplate: outputTpl
                )
                .CreateLogger();
        }
    }
}
