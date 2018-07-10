using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MapHive.Api.Core.ApiControllers;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MapHive.Core.Api.Controllers
{
    /// <summary>
    /// App localization APIs
    /// </summary>
    [Route("applocalizations")]
    public class AppLocalizationsController : DbCtxController<MapHiveDbContext>
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
        public async Task<IActionResult> GetAppLocalisations([FromQuery] string langCodes, [FromQuery] string appIdentifiers)
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
        /// Bulk localisations save input
        /// </summary>
        public class BulkSaveInput
        {
            public bool? Overwrite { get; set; }
            public string[] LangsToImport { get; set; }
            public LocalizationClass[] AppLocalisations { get; set; }
        }

        /// <summary>
        /// Saves app localisations in bulk
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("bulksave")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> BulkSaveAppLocalisations([FromBody] BulkSaveInput data)
        {
            try
            {
                await
                    AppLocalization.SaveLocalizationsAsync(GetDefaultDbContext(), data.AppLocalisations, data.Overwrite, data.LangsToImport);

                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
