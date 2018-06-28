using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DataModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.Configuration
{
    public partial class WebClientConfiguration
    {
        /// <summary>
        /// Gets a web client confoguration in a form of a dictionary
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        public static async Task<Dictionary<string, object>> GetConfigurationAsync(DbContext dbCtx)
        {
            var cfg = new Dictionary<string, object>();

            //output the app hash properties
            cfg[nameof(AppHashProperties)] = AppHashProperties;
            cfg[nameof(HashPropertyDelimiter)] = HashPropertyDelimiter;
            cfg[nameof(HashPropertyValueDelimiter)] = HashPropertyValueDelimiter;

            //do an 'auth preflight', so user gets better experience - app will prompt for authentication straight away.
            cfg["AuthRequiredAppIdentifiers"] = await Application.GetIdentifiersForAppsRequiringAuthAsync(dbCtx);


            //allowed urls for xwindow comm
            cfg["AllowedXWindowMsgBusOrigins"] = await Application.GetAllowedXWindowMsgBusOriginsAsync(dbCtx);

            //cookies!!!
            cfg[nameof(MhCookie)] = MhCookie;
            cfg[nameof(CookieValidSeconds)] = CookieValidSeconds;

            //lng related stuff
            //FIXME - move to localization module!
            cfg["DefaultLang"] = (await Lang.GetDefaultLangAsync(dbCtx))?.LangCode;
            cfg["SupportedLangs"] = await Lang.GetSupportedLangsAsync(dbCtx);
            cfg[nameof(LangParam)] = LangParam;
            cfg[nameof(HeaderLang)] = HeaderLang;


            //some other xtra headers
            //----------------------------------

            //total header used to pass info on the total count of a dataset (for clientside paging)
            cfg[nameof(HeaderTotal)] = HeaderTotal;

            //source header - used to pass a full uri of a client when issuing a request; url parts (anchors) are truncated by browsers
            cfg[nameof(HeaderSource)] = HeaderSource;

            return cfg;
        }

        /// <summary>
        /// Outputs the web client confoguration in a form of a js script
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        public static async Task<string> GetConfigurationScriptAsync(DbContext dbCtx)
        {
            return GetConfigurationScriptAsync(await GetConfigurationAsync(dbCtx));
        }



        /// <summary>
        /// Gets a web client cfg script with an exception msg appended
        /// </summary>
        /// <param name="propertyName">name of a property to hold the exception in the mhcfg object</param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetConfigurationScriptFromException(Exception ex, string propertyName = "ConfigurationError")
        {
            return GetConfigurationScriptAsync(new Dictionary<string, object>
            {
                { propertyName, ex.Message }
            });
        }

        /// <summary>
        /// Appends new data into a single mhapi cfg script, so can inject data to a mainf wgcfg script multiple times from multiple sources
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="camelCase">Whether or not the dictionary keys should be camelcased</param>
        /// <returns></returns>
        public static string GetConfigurationScriptAsync(Dictionary<string, object> cfg, bool camelCase = true)
        {
            //Note: need to output the camelised property names

            var script = $"var {MhCfgVariableName} = {MhCfgVariableName} || {{}};";

            foreach (var cfgKey in cfg.Keys)
            {
                script += $"{MhCfgVariableName}.{Camelise(cfgKey)} = {Serialize(cfg[cfgKey], camelCase)};";
            }

            return script;
        }

        /// <summary>
        /// camel case json serialiser
        /// </summary>
        protected static JsonSerializerSettings CamelCaseSerializerSettings  => new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() };

        /// <summary>
        /// serializes an object to JSON with camelised property names
        /// </summary>
        /// <param name="o"></param>
        /// <param name="camelCase">Whether or not the dictionary keys should be camelcased</param>
        /// <returns></returns>
        protected static string Serialize(object o, bool camelCase = true)
        {
            return camelCase ? JsonConvert.SerializeObject(o, Formatting.None, CamelCaseSerializerSettings) : JsonConvert.SerializeObject(o, Formatting.None);
        }

        /// <summary>
        /// camelises a property name - makes the first char lowercase
        /// </summary>
        /// <param name="pName"></param>
        /// <returns></returns>
        protected static string Camelise(string pName)
        {
            return $"{pName.Substring(0, 1).ToLower()}{pName.Substring(1)}";
        }
    }
}
