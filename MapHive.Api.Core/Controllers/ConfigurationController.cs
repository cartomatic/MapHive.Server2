using MapHive.Core.Api.ApiControllers;
using MapHive.Core.Api.Result;
using MapHive.Core.Configuration;
using MapHive.Core.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

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
        /// returns a user configuration for a user/token/etc with specified characteristics.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("user")]
        [ProducesResponseType(typeof(UserConfiguration), 200)]
        [ProducesResponseType(500)]
        [ApiExplorerSettings(IgnoreApi = true)] //make sure this api is not visible in docs!!! it's kinda private and while should be available it should not be freely used really
        public async Task<IActionResult> GetUserConfigurationAsync([FromQuery] UserConfigurationQuery input, [FromQuery] string token)
        {
            try
            {
                var cfg = Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig();

                if (token != cfg.GetSection("AccessTokens:Auth").Get<string>())
                    return Unauthorized();

                return Ok(await MapHive.Core.Configuration.UserConfiguration.GetAsync(GetDefaultDbContext(), input));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// returns a organization configuration for an org id.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("organization")]
        [ProducesResponseType(typeof(UserConfiguration), 200)]
        [ProducesResponseType(500)]
        [ApiExplorerSettings(IgnoreApi = true)] //make sure this api is not visible in docs!!! it's kinda private and while should be available it should not be freely used really
        public async Task<IActionResult> GetUserConfigurationAsync([FromQuery] Guid organizationId, [FromQuery] string token)
        {
            try
            {
                var cfg = Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig();

                if (token != cfg.GetSection("AccessTokens:Auth").Get<string>())
                    return Unauthorized();

                return Ok(await MapHive.Core.Configuration.OrganizationConfiguration.GetAsync(GetDefaultDbContext(), organizationId));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}