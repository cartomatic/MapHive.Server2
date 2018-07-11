using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MapHive.Api.Core;
using MapHive.Api.Core.ApiControllers;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using Microsoft.AspNetCore.Mvc;

namespace MapHive.Core.Api.Controllers
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
            return await base.GetAsync(sort, filter, start, limit);
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
            return await base.GetAsync(uuid);
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
        public async Task<IActionResult> PutAsync([FromRoute] Guid organizationuuid, [FromBody] Team obj, [FromRoute] Guid uuid)
        {
            return await base.PutAsync(obj, uuid);
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
        public async Task<IActionResult> PostAsync([FromRoute] Guid organizationuuid, Team obj)
        {
            return await base.PostAsync(obj);
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
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid organizationuuid, Guid uuid)
        {
            return await base.DeleteAsync(uuid);
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
        public async Task<IActionResult> GetTeamUsers(Guid organizationuuid, Guid uuid)
        {
            try
            {
                //grab a team and its users
                var team = await new Team().ReadAsync(GetOrganizationDbContext(), uuid);
                if (team == null)
                    return NotFound();

                var users = await team.GetChildrenAsync<Team, MapHiveUser>(GetOrganizationDbContext());
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
        public async Task<IActionResult> GetTeamApplications(Guid organizationuuid, Guid uuid)
        {
            try
            {
                //grab a team and its apps
                var team = await new Team().ReadAsync(GetOrganizationDbContext(), uuid);
                if (team == null)
                    return NotFound();

                var apps = await team.GetChildrenAsync<Team, Application>(GetOrganizationDbContext());
                if (apps.Any())
                {
                    //re-read the links now to obtain the extra links info!
                    var appLinks = await team.GetChildLinksAsync<Team, Application>(GetOrganizationDbContext());

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