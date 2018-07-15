using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MapHive.Api.Core.ApiControllers;
using MapHive.Api.Core.Result;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MapHive.Core.Api.Controllers
{
    /// <summary>
    /// App localization APIs
    /// </summary>
    [Route("applocalization")]
    public class AppLocalizationController : DbCtxController<MapHiveDbContext>
    {
        /// <summary>
        /// Gets an app localization - all the translations retrieved from a db, for a given app.
        /// </summary>
        /// <param name="langCodes"></param>
        /// <param name="appIdentifiers"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("localizeit")]
        [MapHive.Api.Core.Serialization.UnmodifiedDictKeyCasingOutputFormatter]
        [ProducesResponseType(typeof(Dictionary<string, Dictionary<string, Dictionary<string, string>>>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAppLocalizations([FromQuery] string langCodes, [FromQuery] string appIdentifiers)
        {
            try
            {
                return Ok(await AppLocalization.GetAppLocalizationsAsync(GetDefaultDbContext(), (langCodes ?? string.Empty).Split(','), (appIdentifiers ?? string.Empty).Split(',')));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Gets app loclaizations in a form of a script that can be consumed by web clients
        /// </summary>
        /// <param name="langCodes"></param>
        /// <param name="appIdentifiers"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("localizeit/script")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAppLocalizationsScript([FromQuery] string langCodes, [FromQuery] string appIdentifiers)
        {
            string scriptContent;

            try
            {
                var localizationData = await AppLocalization.GetAppLocalizationsAsync(GetDefaultDbContext(),
                    (langCodes ?? string.Empty).Split(','), (appIdentifiers ?? string.Empty).Split(','));

                //Important - localization data MUST be serialized with casing intact as class namespaces are the keys to solving translations on the clientside
                var localizationDataSerialized = JsonConvert.SerializeObject(localizationData, Formatting.None);

                var replacementToken = "{HERE_GOES_LOCALE_DATA_DUDE}";

                var scriptStub = MapHive.Core.Configuration.WebClientConfiguration.GetConfigurationScript(
                    new Dictionary<string, object>
                    {
                        {"localization", replacementToken}
                    });

                scriptContent = scriptStub.Replace($"\"{replacementToken}\"", localizationDataSerialized);
            }
            catch (Exception ex)
            {
                scriptContent = MapHive.Core.Configuration.WebClientConfiguration
                    .GetConfigurationScriptFromException(ex);
            }

            return new JavaScriptResult(scriptContent);
        }

        /// <summary>
        /// Bulk localizations save input
        /// </summary>
        public class BulkSaveInput
        {
            /// <summary>
            /// Whether or not data in db should be overwritten; set to false when this is a differential import!
            /// </summary>
            public bool? Overwrite { get; set; }
            
            /// <summary>
            /// What languages should be imported; in a case localization data contains more langs, it will filter out specified langs;
            /// comma delimited list
            /// </summary>
            public string[] LangsToImport { get; set; }

            /// <summary>
            /// Localization data
            /// </summary>
            public LocalizationClass[] AppLocalizations { get; set; }
        }

        /// <summary>
        /// Saves app localizations in bulk
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("bulksave")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> BulkSaveAppLocalizations([FromBody] BulkSaveInput data)
        {
            try
            {
                await
                    AppLocalization.SaveLocalizationsAsync(GetDefaultDbContext(), data.AppLocalizations, data.Overwrite, data.LangsToImport);

                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
