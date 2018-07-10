using System;
using System.Linq;
using System.Threading.Tasks;
using MapHive.Api.Core.ApiControllers;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.Api.Controllers
{
    /// <summary>
    /// General role APIs
    /// </summary>
    [Route("roles")]
    public class RolesController : CrudController<Role, MapHiveDbContext>
    {
        /// <summary>
        /// Gets organization role by id
        /// </summary>
        /// <param name="orgid"></param>
        /// <param name="identifier"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{orgid}/{identifier}")]
        [ProducesResponseType(typeof(Role), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> GetRoleByIdentifierAsync([FromRoute] Guid orgid, [FromRoute] string identifier)
        {
            try
            {
                var org = await _dbCtx.Organizations.FirstOrDefaultAsync(o => o.Uuid == orgid);
                if (org == null)
                    return BadRequest();

                var role =
                    (await org.GetOrganizationAssetsAsync<Role>(_dbCtx))?.assets?.FirstOrDefault(r => r.Identifier == identifier);

                if (role == null)
                    return NotFound();

                return Ok(role);

            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}