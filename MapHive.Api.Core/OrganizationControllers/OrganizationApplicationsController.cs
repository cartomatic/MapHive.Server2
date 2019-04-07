using MapHive.Core.Api;
using MapHive.Core.Api.ApiControllers;
using MapHive.Core.Api.Extensions;
using MapHive.Core.DAL;
using MapHive.Core.DataModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MapHive.Api.Core.Controllers
{
    /// <summary>
    /// Organization applicatons APIs
    /// </summary>
    [Route("organizations/{" + OrganizationContextActionFilterAttribute.OrgIdPropertyName + "}/applications")]
    public class OrganizationApplicationsController : OrganizationCrudController<Application, MapHiveDbContext>
    {
        /// <summary>
        /// Reads applications accessible for an organisation
        /// </summary>
        /// <param name="organizationuuid"></param>
        /// <param name="sort"></param>
        /// <param name="filter"></param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("linkable")]
        [ProducesResponseType(typeof(IEnumerable<Application>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Get([FromRoute] Guid organizationuuid, [FromQuery] string sort = null, [FromQuery] string filter = null,
            [FromQuery] int start = 0, [FromQuery] int limit = 25)
        {
            try
            {
                //Note:
                //main mh env objects are kept in the maphive_meta db!

                var apps = await OrganizationContext.GetOrganizationLinkableAppsAsync(GetDefaultDbContext(), sort, filter, start, limit);

                if (apps != null)
                {
                    HttpContext.AppendTotalHeader(apps?.count ?? 0);
                    return Ok(apps?.applications);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return this.HandleException(ex);
            }
        }


    }
}