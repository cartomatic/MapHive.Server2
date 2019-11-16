using Cartomatic.Utils.Email;
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
    /// Organisation users controller - allows for reading users scoped within an organisation
    /// </summary>
    [Route("organizations/{" + OrganizationContextActionFilterAttribute.OrgIdPropertyName + "}/users")]
    public class OrganisationUsersController : OrganizationCrudController<MapHiveUser, MapHiveDbContext>
    {
        private IEmailSender EmailSender { get; set; }


        public OrganisationUsersController(IEmailSender emailSender)
        {
            EmailSender = emailSender;
        }

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
        [CrudPrivilegeRequiredRead]
        public async Task<IActionResult> GetAsync([FromRoute]Guid organizationuuid, [FromQuery]string sort = null, [FromQuery]string filter = null, [FromQuery]int start = 0, [FromQuery] int limit = 25)
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

                HttpContext.AppendTotalHeader(users?.count ?? 0);
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
        [CrudPrivilegeRequiredRead]
        public async Task<IActionResult> GetAsync([FromRoute] Guid organizationuuid, [FromRoute] Guid uuid)
        {
            //note:
            //org users and org roles are read from mh meta db!
            //This is where some env core objects are kept

            //ensure user belongs to an org
            if (await OrganizationContext.IsOrgMemberAsync(GetDefaultDbContext(), uuid))
            {
                var user = (await base.GetAsync(uuid, GetDefaultDbContext())).GetContent<MapHiveUser>();
                if (user == null)
                    return NotFound();

                //just check for owner / admin roles; it is an org member anyway
                if (await OrganizationContext.IsOrgOwnerAsync(GetDefaultDbContext(), user))
                    user.OrganizationRole = Organization.OrganizationRole.Owner;
                else if (await OrganizationContext.IsOrgAdminAsync(GetDefaultDbContext(), user))
                    user.OrganizationRole = Organization.OrganizationRole.Admin;
                else
                    user.OrganizationRole = Organization.OrganizationRole.Member;

                return Ok(user);
            }

            return NotFound();
        }


        /// <summary>
        /// Creates an OrgUser for the organization. Links the user to the org with the default org member role
        /// </summary>
        /// <param name="organizationuuid"></param>
        /// <param name="user"></param>
        /// <param name="applicationContext">Application context to be used when sending out emails; when not provided, default emails are sent out; if no emails should be sent out simply provide a non-existent context</param>
        /// <param name="ea">Email account details if need to send out emails using a custom account</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{applicationContext?}")]
        [ProducesResponseType(typeof(MapHiveUser), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [CrudPrivilegeRequiredCreate]
        public async Task<IActionResult> PostAsync([FromRoute] Guid organizationuuid, [FromBody] MapHiveUser user, string applicationContext = null, [FromQuery] EmailAccount ea = null)
        {
            //only owners or admins should be allowed to perform this action
            var callerId = Cartomatic.Utils.Identity.GetUserGuid();
            if (!(UserIsOrgOwner(callerId) || UserIsOrgAdmin(callerId)))
                return NotAllowed();

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
                    {"RedirectUrl", this.GetRequestSource(HttpContext).Split('#')[0]}
                };

                var (emailAccount, emailTemplate) = await GetEmailStuffAsync("user_created", applicationContext);
                //use custom email account if provided
                if (ea != null && ea.SeemsComplete())
                    emailAccount = ea;

                //note:
                //org users and org roles are created against mh meta db!
                //This is where some env core objects are kept

                var createdUser = await MapHiveUser.CreateUserAccountAsync(GetDefaultDbContext(), user, EmailSender, emailAccount, emailTemplate?.Prepare(replacementData));

                if (createdUser != null)
                {
                    //user has been created, so now need to add it to the org with an appropriate role
                    await this.OrganizationContext.AddOrganizationUserAsync(GetDefaultDbContext(), createdUser.User);

                    //at this stage user should have had a member role assigned
                    //mkae sure to assign the one that has been asked for
                    if (user.OrganizationRole.HasValue && user.OrganizationRole != Organization.OrganizationRole.Member)
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
        [CrudPrivilegeRequiredCreate]
        public async Task<IActionResult> LinkAsync([FromRoute] Guid organizationuuid, [FromBody] MapHiveUser user)
        {
            //only owners or admins should be allowed to perform this action
            var callerId = Cartomatic.Utils.Identity.GetUserGuid();
            if (!(UserIsOrgOwner(callerId) || UserIsOrgAdmin(callerId)))
                return NotAllowed();

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
        [CrudPrivilegeRequiredUpdate]
        public async Task<IActionResult> PutAsync([FromRoute] Guid organizationuuid, [FromBody] MapHiveUser user, [FromRoute] Guid uuid)
        {
            try
            {
                //note:
                //org users and org roles are modified against mh meta db!
                //This is where some env core objects are kept


                //can modify user only if org admin or owner or self
                var callerId = Cartomatic.Utils.Identity.GetUserGuid();
                if (!(UserIsOrgOwner(callerId) || UserIsOrgAdmin(callerId) || UserIsSelf(uuid)))
                    return NotAllowed();


                //make sure user 'belongs' to an org
                if (!await OrganizationContext.IsOrgMemberAsync(GetDefaultDbContext(), uuid))
                    return BadRequest("Not an org user.");

                

                //note: perhaps should disallow editing some properties such as parentOrgId, etc. 
                var entity = await user.UpdateAsync(GetDefaultDbContext(), uuid);

                if (entity != null)
                {
                    //once the user has been updated, need to update his role within an org too
                    //making sure though not to change own roles.
                    //this is so only other users with enough credentials are allowed to modify stuff such as roles
                    if (!UserIsSelf(uuid))
                    {
                        await this.OrganizationContext.ChangeOrganizationUserRoleAsync(GetDefaultDbContext(), user);
                    }

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
        /// Updates a role of a linked user
        /// </summary>
        /// <param name="organizationuuid"></param>
        /// <param name="user"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("link/{uuid}")]
        [ProducesResponseType(typeof(MapHiveUser), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [CrudPrivilegeRequiredUpdate]
        public async Task<IActionResult> UpdateLinkAsync([FromRoute] Guid organizationuuid, [FromBody] MapHiveUser user, [FromRoute] Guid uuid)
        {
            try
            {
                //only owners or admins should be allowed to perform this action
                var callerId = Cartomatic.Utils.Identity.GetUserGuid();
                if (!(UserIsOrgOwner(callerId) || UserIsOrgAdmin(callerId)))
                    return NotAllowed();


                //make sure user 'belongs' to an org
                if (!await OrganizationContext.IsOrgMemberAsync(GetDefaultDbContext(), uuid))
                    return BadRequest("Not an org user.");

                
                //just need to update its role within an org too
                user.Uuid = uuid; //in put no uuid in the model!
                await this.OrganizationContext.ChangeOrganizationUserRoleAsync(GetDefaultDbContext(), user);
                return Ok(user);
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
        [CrudPrivilegeRequiredDestroy]
        public async Task<IActionResult> UnLinkAsync([FromRoute] Guid organizationuuid, [FromRoute] Guid uuid)
        {
            try
            {
                //only owners or admins should be allowed to perform this action
                var callerId = Cartomatic.Utils.Identity.GetUserGuid();
                if (!(UserIsOrgOwner(callerId) || UserIsOrgAdmin(callerId)))
                    return NotAllowed();

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
        [CrudPrivilegeRequiredDestroy]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid organizationuuid, [FromRoute] Guid uuid)
        {
            try
            {
                //note:
                //org users and org roles are modified against mh meta db!
                //This is where some env core objects are kept


                //only owners or admins should be allowed to perform this action
                var callerId = Cartomatic.Utils.Identity.GetUserGuid();
                var callerIsOrgOwner = UserIsOrgOwner(callerId);
                var callerIsOrgAdmin = UserIsOrgAdmin(callerId);

                //only owners and admins should be able to delete users!
                if (!(callerIsOrgOwner || callerIsOrgAdmin))
                    return NotAllowed();


                //make sure user 'belongs' to an org
                if (!await OrganizationContext.IsOrgMemberAsync(GetDefaultDbContext(), uuid))
                    return BadRequest("Not an org user.");

                //make sure to prevent self deletes
                if (uuid == Cartomatic.Utils.Identity.GetUserGuid())
                    return BadRequest("Cannot remove self.");

                var isOrgOwner = await OrganizationContext.IsOrgOwnerAsync(GetDefaultDbContext(), uuid);

                if (isOrgOwner)
                    return BadRequest("Cannot remove org owner.");

                

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
        [CrudPrivilegeRequiredDestroy]
        public async Task<IActionResult> ForceDeleteAsync([FromRoute] Guid organizationuuid, [FromRoute] Guid uuid)
        {
            try
            {
                //note:
                //org users and org roles are modified against mh meta db!
                //This is where some env core objects are kept
                

                //only owners or admins should be allowed to perform this action
                var callerId = Cartomatic.Utils.Identity.GetUserGuid();
                var callerIsOrgOwner = UserIsOrgOwner(callerId);
                var callerIsOrgAdmin = UserIsOrgAdmin(callerId);

                //only owners and admins should be able to delete users!
                if (!(callerIsOrgOwner || callerIsOrgAdmin))
                    return NotAllowed();


                //make sure user 'belongs' to an org
                if (!await OrganizationContext.IsOrgMemberAsync(GetDefaultDbContext(), uuid))
                    return BadRequest("Not an org user.");

                //make sure to prevent self deletes
                if (uuid == Cartomatic.Utils.Identity.GetUserGuid())
                    return BadRequest("Cannot remove self.");

                var isOrgOwner = await OrganizationContext.IsOrgOwnerAsync(GetDefaultDbContext(), uuid);

                if (isOrgOwner)
                    return BadRequest("Cannot remove org owner.");


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
