using MapHive.Core.Api.ApiControllers;
using MapHive.Core.Api.Result;
using MapHive.Core.Configuration;
using MapHive.Core.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MapHive.Api.Core.Controllers
{
    /// <summary>
    /// Configuration APIs
    /// </summary>
    [Route("configuration")]
    public class ConfigurationController : DbCtxController<MapHiveDbContext>
    {
        /// <summary>
        /// Returns a WebGIS webclient configuration script
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("webclient")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> GetWebClientConfigurationScriptAsync()
        {
            string scriptContent = string.Empty;

            try
            {
                scriptContent = await MapHive.Core.Configuration.WebClientConfiguration
                    .GetConfigurationScriptAsync(GetDefaultDbContext());

            }
            catch (Exception ex)
            {
                scriptContent = MapHive.Core.Configuration.WebClientConfiguration
                    .GetConfigurationScriptFromException(ex);
            }

            return new JavaScriptResult(scriptContent);

        }

        /// <summary>
        /// returns a user configuration for a user/token/etc with specified characterisitcs.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("user")]
        [ProducesResponseType(typeof(UserConfiguration), 200)]
        [ProducesResponseType(500)]
        [ApiExplorerSettings(IgnoreApi = true)] //make sure this api is not visible in docs!!! it's kinda private and while should be available it should not be freely used really
        public async Task<IActionResult> GetUserConfigurationAsync([FromQuery] UserConfigurationQuery input)
        {
            try
            {
                //TODO - secure this endpoint with a m2m token of some sort. so one cannot retrieve user orgs by simply knowing user id.
                //TODO - calling api needs to provide a token in order to call this api; see the user configuration attribute too as will have to provide the token there!

                return Ok(await MapHive.Core.Configuration.UserConfiguration.GetAsync(GetDefaultDbContext(), input));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}