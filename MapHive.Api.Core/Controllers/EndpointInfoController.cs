using System;
using System.Threading.Tasks;
using Cartomatic.Utils.Email;
using MapHive.Core;
using MapHive.Core.Api.ApiControllers;
using MapHive.Core.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MapHive.Api.Core.Controllers
{
    /// <summary>
    /// Endpoint info APIs
    /// </summary>
    [Route("endpointinfo")]
    public class EndpointInfoController : DbCtxController<MapHiveDbContext>
    {
        /// <summary>
        /// Returns information on the deployed api version
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("version")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetVersionInfo()
        {
            try
            {
                return Ok(Cartomatic.Utils.Git.GetRepoVersion());
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}