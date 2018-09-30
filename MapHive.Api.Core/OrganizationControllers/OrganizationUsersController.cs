﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MapHive.Core.Api;
using MapHive.Core.Api.ApiControllers;
using MapHive.Core.Api.Extensions;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using Microsoft.AspNetCore.Mvc;

namespace MapHive.Api.Core.Controllers
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
                //note:
                //org users and org roles are read from mh meta db!
                //This is where some env core objects are kept

                var users = await OrganizationContext.GetOrganizationAssetsAsync<MapHiveUser>(GetDefaultDbContext(), sort, filter, start, limit);
                if (users == null)
                    return NotFound();

                var roles2users = await OrganizationContext.GetOrgRoles2UsersMapAsync(GetDefaultDbContext());

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
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAsync([FromRoute] Guid organizationuuid, [FromQuery] Guid uuid)
        {
            //note:
            //org users and org roles are read from mh meta db!
            //This is where some env core objects are kept

            //ensure user belongs to an org
            if(await OrganizationContext.IsOrgMemberAsync(GetDefaultDbContext(), uuid))
                return await base.GetAsync(uuid, GetDefaultDbContext());

            return NotFound();
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

                //note:
                //org users and org roles are created against mh meta db!
                //This is where some env core objects are kept

                var createdUser = await MapHiveUser.CreateUserAccountAsync(GetDefaultDbContext(), user, email.emailAccount, email.emailTemplate.Prepare(replacementData));

                if (createdUser != null)
                {
                    //user has been created, so now need to add it to the org with an appropriate role
                    await this.OrganizationContext.AddOrganizationUserAsync(GetDefaultDbContext(), createdUser.User);

                    //at this stage user should have had a member role assigned
                    //mkae sure to assign the one that has been asked for
                    if(user.OrganizationRole.HasValue && user.OrganizationRole != Organization.OrganizationRole.Member)
                        await this.OrganizationContext.ChangeOrganizationUserRoleAsync(GetDefaultDbContext(), user);

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
                //note:
                //org users and org roles are modified against mh meta db!
                //This is where some env core objects are kept

                await this.OrganizationContext.AddOrganizationUserAsync(GetDefaultDbContext(), user);
                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }


        /// <summary>
        /// Updates an organization user
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
                //note:
                //org users and org roles are modified against mh meta db!
                //This is where some env core objects are kept

                //make sure user 'belongs' to an org
                if (await OrganizationContext.IsOrgMemberAsync(GetDefaultDbContext(), uuid))
                    return BadRequest("Not an org user.");

                var entity = await user.UpdateAsync(GetDefaultDbContext(), uuid);

                if (entity != null)
                {
                    //once the user has been updated, need to update its role within an org too
                    await this.OrganizationContext.ChangeOrganizationUserRoleAsync(GetDefaultDbContext(), user);

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
                //note:
                //org users and org roles are modified against mh meta db!
                //This is where some env core objects are kept

                //get a user an make sure he is not an org user!
                var user = await Base.ReadObjAsync<MapHiveUser>(GetDefaultDbContext(), uuid);

                if (user == null)
                    return BadRequest("No such user.");

                if (
                    (user.IsOrgUser && user.ParentOrganizationId == organizationuuid)
                    || user.UserOrgId == organizationuuid //also avoid removing own org of a user!
                )
                    throw MapHive.Core.DataModel.Validation.Utils.GenerateValidationFailedException(nameof(MapHiveUser), MapHive.Core.DataModel.Validation.ValidationErrors.OrgOwnerDestroyError);
                        
                //not providing a user role will effectively wipe out user assignment
                user.OrganizationRole = null;
                await this.OrganizationContext.ChangeOrganizationUserRoleAsync(GetDefaultDbContext(), user);

                OrganizationContext.RemoveLink(user);
                await OrganizationContext.UpdateAsync(GetDefaultDbContext());

                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }


        /// <summary>
        /// Deletes an organization user;
        /// this api performs a soft delete only - sets IsAccountClosed property to true, so user should not be able to authenticate anymore;
        /// user record is still present in both maphive meta and aspnet identity databases
        /// </summary>
        /// <param name="organizationuuid"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{uuid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteAsync(Guid organizationuuid, Guid uuid)
        {
            try
            {
                //note:
                //org users and org roles are modified against mh meta db!
                //This is where some env core objects are kept

                //make sure user 'belongs' to an org
                if (await OrganizationContext.IsOrgMemberAsync(GetDefaultDbContext(), uuid))
                    return BadRequest("Not an org user.");

                //make sure to prevent self deletes
                if (uuid == Cartomatic.Utils.Identity.GetUserGuid())
                    return BadRequest("Cannot remove self.");

                var isOrgOwner = await OrganizationContext.IsOrgOwnerAsync(GetDefaultDbContext(), uuid);

                if (isOrgOwner)
                    return BadRequest("Cannot remove org owner.");

                var isOrgAdmin = await OrganizationContext.IsOrgAdminAsync(GetDefaultDbContext(), uuid);

                //only owners and admins should be able to delete users!
                if (!(isOrgAdmin || isOrgOwner))
                    return NotAllowed();

                var user = await Base.ReadObjAsync<MapHiveUser>(GetDefaultDbContext(), uuid);

                if (user == null)
                    return BadRequest("No such user.");

                await user.DestroyAsync(GetDefaultDbContext());

                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Completely deletes an organization user;
        /// mind this operation only cleans up links in the core api;
        /// objects created by a user do not get deleted
        /// </summary>
        /// <param name="organizationuuid"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{uuid}/usetheforceluke")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ForceDeleteAsync(Guid organizationuuid, Guid uuid)
        {
            try
            {
                //note:
                //org users and org roles are modified against mh meta db!
                //This is where some env core objects are kept

                //make sure user 'belongs' to an org
                if (await OrganizationContext.IsOrgMemberAsync(GetDefaultDbContext(), uuid))
                    return BadRequest("Not an org user.");

                //make sure to prevent self deletes
                if (uuid == Cartomatic.Utils.Identity.GetUserGuid())
                    return BadRequest("Cannot remove self.");

                var isOrgOwner = await OrganizationContext.IsOrgOwnerAsync(GetDefaultDbContext(), uuid);

                if (isOrgOwner)
                    return BadRequest("Cannot remove org owner.");

                var isOrgAdmin = await OrganizationContext.IsOrgAdminAsync(GetDefaultDbContext(), uuid);

                //only owners and admins should be able to delete users!
                if (!(isOrgAdmin || isOrgOwner))
                    return NotAllowed();

                var user = await Base.ReadObjAsync<MapHiveUser>(GetDefaultDbContext(), uuid);

                if (user == null)
                    return BadRequest("No such user.");

                await user.ForceDestroyAsync<MapHiveUser>(GetDefaultDbContext());

                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

    }
}
