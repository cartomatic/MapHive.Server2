using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MapHive.Api.Core;
using MapHive.Api.Core.ApiControllers;
using MapHive.Api.Core.Extensions;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using Microsoft.AspNetCore.Mvc;

namespace MapHive.Core.Api.Controllers
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
        public async Task<IActionResult> Get(Guid organizationuuid, string sort = null, string filter = null,
            int start = 0,
            int limit = 25)
        {
            try
            {
                //Note:
                //main mh env objects are kept in the maphive_meta db!

                var apps = await OrganizationContext.GetOrganizationLinkableAppsAsync(GetDefaultDbContext(), sort, filter, start, limit);

                if (apps != null)
                {
                    Context.AppendTotalHeader(apps?.count ?? 0);
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