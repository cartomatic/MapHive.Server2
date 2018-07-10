using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using MapHive.Api.Core.ApiControllers;
using MapHive.Core.Configuration;
using MapHive.Core.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MapHive.Core.Api.Controllers
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
        public async Task<HttpResponseMessage> GetWebClientConfigurationScript()
        {
            string content = string.Empty;
            var statuscode = HttpStatusCode.OK;

            try
            {
                content = await MapHive.Core.Configuration.WebClientConfiguration
                    .GetConfigurationScriptAsync(GetDefaultDbContext());
                
            }
            catch (Exception ex)
            {
                content = "//err";

#if DEBUG
                content = MapHive.Core.Configuration.WebClientConfiguration
                    .GetConfigurationScriptFromException(ex);
#endif
            }

            return new HttpResponseMessage
            {
                StatusCode = statuscode,
                Content = new StringContent(content, Encoding.UTF8, "text/javascript")
            };
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
        public async Task<IActionResult> GetUserConfiguration([FromUri] UserConfigurationQuery input)
        {
            try
            {
                return Ok(await MapHive.Core.Configuration.UserConfiguration.GetAsync(GetDefaultDbContext(), input));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}