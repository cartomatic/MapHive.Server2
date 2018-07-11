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
    /// Organisation users controller - allows for reading users scoped within an organisation
    /// </summary>
    [Route("organizations/{" + OrganizationContextActionFilterAttribute.OrgIdPropertyName + "}/users")]
    public class OrganisationUsersController : OrganizationCrudController<MapHiveUser, MapHiveDbContext>
    {
        /// <summary>
        /// returns a list of organisation users
        /// </summary>
        /// <param name="organizationuuid"></param>
        /// <param name="sort"></param>
        /// <param name="filter"></param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(IEnumerable<MapHiveUser>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAsync(Guid organizationuuid, string sort = null, string filter = null, int start = 0,
            int limit = 25)
        {
            try
            {
                var users = await OrganizationContext.GetOrganizationAssetsAsync<MapHiveUser>(_dbCtx, sort, filter, start, limit); //note: read from mh meta db!
                if (users == null)
                    return NotFound();

                var roles2users = await OrganizationContext.GetOrgRoles2UsersMapAsync(GetOrganizationDbContext());

                foreach (var user in users?.assets)
                {
                    user.OrganizationRole = OrganizationContext.GetUserOrgRole(roles2users, user.Uuid);
                }

                Context.AppendTotalHeader(users?.count ?? 0);
                return Ok(users?.assets);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
            
        }

        /// <summary>
        /// Gets an organization user by id
        /// </summary>
        /// <param name="organizationuuid"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{uuid}")]
        [ProducesResponseType(typeof(MapHiveUser), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAsync([FromRoute] Guid organizationuuid, [FromQuery] Guid uuid)
        {
            return await base.GetAsync(uuid);
        }


        /// <summary>
        /// Creates an OrgUser for the organization. Links the user to the org with the default org member role
        /// </summary>
        /// <param name="organizationuuid"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(MapHiveUser), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostAsync(Guid organizationuuid, MapHiveUser user)
        {
            //this is an org user, so needs to be flagged as such!
            user.IsOrgUser = true;
            user.ParentOrganizationId = organizationuuid;

            try
            {
                //initial email template customization
                var replacementData = new Dictionary<string, object>
                {
                    {"UserName", $"{user.GetFullUserName()} ({user.Email})"},
                    {"Email", user.Email},
                    {"RedirectUrl", this.GetRequestSource(Context).Split('#')[0]}
                };

                var email = await GetEmailStuffAsync("user_created");

                //note: user is created in mh meta db!
                var createdUser = await MapHiveUser.CreateUserAccountAsync(_dbCtx, user, email.emailAccount, email.emailTemplate.Prepare(replacementData));

                if (createdUser != null)
                {
                    //user has been created, so now need to add it to the org with an appropriate role
                    await this.OrganizationContext.AddOrganizationUserAsync(_dbCtx, createdUser.User);

                    return Ok(createdUser.User);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return this.HandleException(ex);
            }
        }

        /// <summary>
        /// Links a user to an organisation
        /// </summary>
        /// <param name="organizationuuid"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("link")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> LinkAsync(Guid organizationuuid, MapHiveUser user)
        {
            try
            {
                await this.OrganizationContext.AddOrganizationUserAsync(_dbCtx, user);
                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }


        /// <summary>
        /// Updates a user
        /// </summary>
        /// <param name="organizationuuid"></param>
        /// <param name="user"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{uuid}")]
        [ProducesResponseType(typeof(MapHiveUser), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutAsync(Guid organizationuuid, MapHiveUser user, Guid uuid)
        {
            try
            {
                var entity = await user.UpdateAsync(_dbCtx, uuid);

                if (entity != null)
                {
                    //once the user has been updated, need to update its role within an org too
                    await this.OrganizationContext.ChangeOrganizationUserRoleAsync(_dbCtx, user);

                    return Ok(entity);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return this.HandleException(ex);
            }
        }

        /// <summary>
        /// Removes and external user link from an Organization
        /// </summary>
        /// <param name="organizationuuid"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("link/{uuid}")]
        [ProducesResponseType(typeof(MapHiveUser), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UnLinkAsync(Guid organizationuuid, Guid uuid)
        {
            try
            {
                //get a user an make sure he is not an org user!
                var user = await Base.ReadObjAsync<MapHiveUser>(_dbCtx, uuid);
                if (
                    user == null || (user.IsOrgUser && user.ParentOrganizationId == organizationuuid)
                    || user.UserOrgId == organizationuuid //also avoid removing own org of a user!
                )
                    throw MapHive.Core.DataModel.Validation.Utils.GenerateValidationFailedException(nameof(MapHiveUser), MapHive.Core.DataModel.Validation.ValidationErrors.OrgOwnerDestroyError);
                        
                //not providing a user role will effectively wipe out user assignment
                user.OrganizationRole = null;
                await this.OrganizationContext.ChangeOrganizationUserRoleAsync(_dbCtx, user);

                OrganizationContext.RemoveLink(user);
                await OrganizationContext.UpdateAsync(_dbCtx);

                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
