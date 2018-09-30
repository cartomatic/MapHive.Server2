using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MapHive.Core.Api.ApiControllers;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using Microsoft.AspNetCore.Mvc;

namespace MapHive.Api.Core.Controllers
{
    /// <summary>
    /// Organizaton APIs
    /// </summary>
    [Route("organizations")]
    public class OrganizationsController : CrudController<Organization, MapHiveDbContext>
    {

        /// <summary>
        /// Gets a collection of Organizations
        /// </summary>
        /// <param name="sort"></param>
        /// <param name="filter"></param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(IEnumerable<Organization>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAsync([FromQuery] string sort = null, [FromQuery] string filter = null, [FromQuery] int start = 0,
            [FromQuery] int limit = 25)
        {
            return await base.GetAsync(sort, filter, start, limit);
        }

        /// <summary>
        /// Gets an Organization by id
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{uuid}")]
        [ProducesResponseType(typeof(Organization), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAsync([FromQuery] Guid uuid)
        {
            return await base.GetAsync(uuid);
        }

        /// <summary>
        /// Updates an Organization
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{uuid}")]
        [ProducesResponseType(typeof(Organization), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutAsync([FromBody] Organization obj, [FromRoute] Guid uuid)
        {
            return await base.PutAsync(obj, uuid);
        }

        /// <summary>
        /// Creates a new Organization
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(Organization), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostAsync([FromBody] Organization obj)
        {
            return await base.PostAsync(obj);
        }

        /// <summary>
        /// Deletes an Organization
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{uuid}")]
        [ProducesResponseType(typeof(Organization), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid uuid)
        {
            return await base.DeleteAsync(uuid);
        }



        /// <summary>
        /// Checks whether or not an org allows an app usage
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{orgId}/allowsapplication/{appId}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CheckIOrgCanUseAppAsync([FromRoute] Guid orgId, [FromRoute] Guid appId)
        {
            try
            {
                return Ok(await Organization.CanUseAppAsync(_dbCtx as MapHiveDbContext, orgId, appId));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
