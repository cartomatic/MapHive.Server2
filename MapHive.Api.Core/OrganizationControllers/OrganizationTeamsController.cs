using MapHive.Core.Api;
using MapHive.Core.Api.ApiControllers;
using MapHive.Core.DAL;
using MapHive.Core.DataModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapHive.Api.Core.Controllers
{
    /// <summary>
    /// Organization Teams APIs
    /// </summary>
    [Route("organizations/{" + OrganizationContextActionFilterAttribute.OrgIdPropertyName + "}/teams")]
    public class OrganisationTeamsController : OrganizationCrudController<Team, MapHiveDbContext>
    {
        /// <summary>
        /// Gets a collection of organization Teams
        /// </summary>
        /// <param name="organizationuuid"></param>
        /// <param name="sort"></param>
        /// <param name="filter"></param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(IEnumerable<Team>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAsync([FromRoute] Guid organizationuuid, [FromQuery] string sort = null, [FromQuery] string filter = null, [FromQuery] int start = 0,
            [FromQuery] int limit = 25)
        {
            //Note:
            //main mh env objects are kept in the maphive_meta db!

            return await base.GetAsync(sort, filter, start, limit, GetDefaultDbContext());
        }

        /// <summary>
        /// Gets an organization Team by id
        /// </summary>
        /// <param name="organizationuuid"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{uuid}")]
        [ProducesResponseType(typeof(Team), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAsync([FromRoute] Guid organizationuuid, [FromQuery] Guid uuid)
        {
            //Note:
            //main mh env objects are kept in the maphive_meta db!

            return await base.GetAsync(uuid, GetDefaultDbContext());
        }

        /// <summary>
        /// Updates an organization Team
        /// </summary>
        /// <param name="organizationuuid"></param>
        /// <param name="obj"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{uuid}")]
        [ProducesResponseType(typeof(Team), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [CrudPrivilegeRequiredUpdate]
        public async Task<IActionResult> PutAsync([FromRoute] Guid organizationuuid, [FromBody] Team obj, [FromRoute] Guid uuid)
        {
            //Note:
            //main mh env objects are kept in the maphive_meta db!

            return await base.PutAsync(obj, uuid, GetDefaultDbContext());
        }

        /// <summary>
        /// Creates a new organization Lang
        /// </summary>
        /// <param name="organizationuuid"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(Team), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [CrudPrivilegeRequiredCreate]
        public async Task<IActionResult> PostAsync([FromRoute] Guid organizationuuid, [FromBody] Team obj)
        {
            //Note:
            //main mh env objects are kept in the maphive_meta db!

            return await base.PostAsync(obj, GetDefaultDbContext());
        }

        /// <summary>
        /// Deletes an organization Team
        /// </summary>
        /// <param name="organizationuuid"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{uuid}")]
        [ProducesResponseType(typeof(Team), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [CrudPrivilegeRequiredDestroy]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid organizationuuid, [FromRoute] Guid uuid)
        {
            //Note:
            //main mh env objects are kept in the maphive_meta db!

            return await base.DeleteAsync(uuid, GetDefaultDbContext());
        }



        /// <summary>
        /// Gets users linked to an organization team
        /// </summary>
        /// <param name="organizationuuid"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{uuid}/users")]
        [ProducesResponseType(typeof(IEnumerable<MapHiveUser>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetTeamUsers([FromRoute] Guid organizationuuid, [FromRoute] Guid uuid)
        {
            try
            {
                //Note:
                //main mh env objects are kept in the maphive_meta db!

                //grab a team and its users
                var team = await new Team().ReadAsync(GetDefaultDbContext(), uuid);
                if (team == null)
                    return NotFound();

                var users = await team.GetChildrenAsync<Team, MapHiveUser>(GetDefaultDbContext());
                if (users.Any())
                    return Ok(users);

                return NotFound();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Gets applications linked to an orgaznization team
        /// </summary>
        /// <param name="organizationuuid"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{uuid}/applications")]
        [ProducesResponseType(typeof(IEnumerable<Application>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetTeamApplications([FromRoute] Guid organizationuuid, [FromRoute] Guid uuid)
        {
            try
            {
                //Note:
                //main mh env objects are kept in the maphive_meta db!

                //grab a team and its apps
                var team = await new Team().ReadAsync(GetDefaultDbContext(), uuid);
                if (team == null)
                    return NotFound();

                var apps = await team.GetChildrenAsync<Team, Application>(GetDefaultDbContext());
                if (apps.Any())
                {
                    //re-read the links now to obtain the extra links info!
                    var appLinks = await team.GetChildLinksAsync<Team, Application>(GetDefaultDbContext());

                    foreach (var app in apps)
                    {
                        app.LinkData = appLinks.First(l => l.ChildUuid == app.Uuid).LinkData;
                    }

                    return Ok(apps);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}