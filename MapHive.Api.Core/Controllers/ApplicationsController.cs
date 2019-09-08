using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MapHive.Core.Api.ApiControllers;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MapHive.Api.Core.Controllers
{
    /// <summary>
    /// Exposes Application APIs
    /// </summary>
    [Route("applications")]
    public class ApplicationsController : CrudController<Application, MapHiveDbContext>
    {
        /// <summary>
        /// Gets a collection of Applications
        /// </summary>
        /// <param name="sort"></param>
        /// <param name="filter"></param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(IEnumerable<Application>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAsync([FromQuery] string sort = null, [FromQuery] string filter = null, [FromQuery] int start = 0,
            [FromQuery] int limit = 25)
        {
            return await base.GetAsync(sort, filter, start, limit);
        }

        /// <summary>
        /// Gets an Application by id
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{uuid}")]
        [ProducesResponseType(typeof(Application), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAsync([FromRoute] Guid uuid)
        {
            return await base.GetAsync(uuid);
        }

        /// <summary>
        /// Updates an application
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{uuid}")]
        [ProducesResponseType(typeof(Application), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [CrudPrivilegeRequiredUpdate]
        public async Task<IActionResult> PutAsync([FromBody] Application obj, [FromRoute] Guid uuid)
        {
            return await base.PutAsync(obj, uuid);
        }

        /// <summary>
        /// Creates a new Application
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(Application), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [CrudPrivilegeRequiredCreate]
        public async Task<IActionResult> PostAsync([FromBody] Application obj)
        {
            return await base.PostAsync(obj);
        }

        /// <summary>
        /// Deletes an application
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{uuid}")]
        [ProducesResponseType(typeof(Application), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [CrudPrivilegeRequiredDestroy]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid uuid)
        {
            return await base.DeleteAsync(uuid);
        }

        /// <summary>
        /// Gets a list of identifiers of apps that do require authentication (uuids, short names, urls) for the apps 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("authidentifiers")]
        [ProducesResponseType(typeof(IEnumerable<string>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAppsWithAuthRequiredAsync()
        {
            try
            {
                return Ok(await Application.GetIdentifiersForAppsRequiringAuthAsync(_dbCtx));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }


        /// <summary>
        /// Gets x window origins for the xwindow msg bus
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("xwindoworigins")]
        [ProducesResponseType(typeof(IEnumerable<string>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetXWindowOriginsAsync()
        {
            try
            {
                return Ok(await Application.GetAllowedXWindowMsgBusOriginsAsync(_dbCtx as MapHiveDbContext));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
