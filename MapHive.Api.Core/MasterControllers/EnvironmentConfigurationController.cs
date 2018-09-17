using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using MapHive.Core;
using MapHive.Core.Api.ApiControllers;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestSharp;

namespace MapHive.Api.Core.Controllers
{
    /// <summary>
    /// Env config controller exposes some management apis; this is a restricted access controller with apis only available to an env admin;
    /// An equivalent controller with same route should appear on the api apps so it is possible to distribute some cals, such as org cleanup (see droporg for details)
    /// </summary>
    [Route("envconfig")]
    [ApiExplorerSettings(IgnoreApi = true)] //make sure this api is not visible in docs!!! it's kinda private and while should be available it should not be freely used really
    public class EnvironmentConfigurationController : DbCtxController<MapHiveDbContext>
    {
        /// <summary>
        /// whether or not authorized user is an env admin
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("envadmin")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> IsEnvAdminAsync()
        {
            try
            {
                return Ok(await UserIsEnvAdminAsync());
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Gets an org by name or id
        /// </summary>
        /// <param name="identifier">slug or guid</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getorg")]
        [ProducesResponseType(typeof(Organization), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetMasterOrgAsync([FromQuery] string identifier)
        {
            try
            {
                if (!await UserIsEnvAdminAsync())
                    return new UnauthorizedResult();

                Organization org = null;

                if (Guid.TryParse(identifier, out var orgId))
                {
                    org = await _db.Organizations.FirstOrDefaultAsync(o => o.Uuid == orgId);
                }
                else
                {
                    org = await _db.Organizations.FirstOrDefaultAsync(o => o.Slug == identifier);
                }

                if (org == null)
                    return NotFound();


                //dbs
                await org.LoadDatabasesAsync(_db);

                return Ok(org);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// registers applications in the environment...
        /// </summary>
        /// <param name="apps"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("registerapps")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RegisterAppsAsync([FromBody] List<Application> apps)
        {
            try
            {
                if (!await UserIsEnvAdminAsync())
                    return new UnauthorizedResult();

                //pretend this is an 'automated' user...
                Cartomatic.Utils.Identity.ImpersonateGhostUserViaHttpContext();
                Cartomatic.Utils.Identity.ImpersonateGhostUser();

                var dbApps = await _db.Applications.ToListAsync();

                //good to go
                foreach (var app in apps)
                {
                    if(dbApps.All(a => a.Uuid != app.Uuid))
                        _db.Applications.Add(app);
                }
                await _db.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// checks if an app is registered in the system
        /// </summary>
        /// <param name="appShortName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("appregistered")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> IsAppRegisteredAsync([FromQuery] string appShortName)
        {
            try
            {
                if (!await UserIsEnvAdminAsync())
                    return new UnauthorizedResult();

                return Ok(await _db.Applications.AnyAsync(a => a.ShortName == appShortName));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// registers apps with org
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="appShortNames"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("registerappstoorg")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RegisterAppsToOrgAsync([FromQuery]string identifier, [FromQuery]string appShortNames)
        {
            try
            {
                if (!await UserIsEnvAdminAsync())
                    return new UnauthorizedResult();

                //pretend this is an 'automated' user...
                Cartomatic.Utils.Identity.ImpersonateGhostUserViaHttpContext();
                Cartomatic.Utils.Identity.ImpersonateGhostUser();

                Organization org = null;

                if (Guid.TryParse(identifier, out var orgId))
                {
                    org = await _db.Organizations.FirstOrDefaultAsync(o => o.Uuid == orgId);
                }
                else
                {
                    org = await _db.Organizations.FirstOrDefaultAsync(o => o.Slug == identifier);
                }

                if (org == null)
                    return BadRequest("Org not found");

                var shortNames = (appShortNames ?? string.Empty).Split(',');
                var apps = await _db.Applications.Where(app=> shortNames.Contains(app.ShortName)).ToListAsync();

                if (!apps.Any())
                    return BadRequest("Apps not found");

                foreach (var app in apps)
                {
                    org.AddLink(app);
                }
                await org.UpdateAsync(_db);

                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Creates an organization
        /// </summary>
        /// <param name="orgSlug"></param>
        /// <param name="orgDescription"></param>
        /// <param name="clean"></param>
        /// <param name="morg"></param>
        /// <param name="email"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createorg")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateOrgAsync([FromQuery]string orgSlug, [FromQuery]string orgDescription, [FromQuery]bool clean, [FromQuery]bool morg, [FromQuery]string email, [FromQuery]string pass)
        {
            try
            {
                if (!await UserIsEnvAdminAsync())
                    return new UnauthorizedResult();

                //pretend this is an 'automated' user...
                Cartomatic.Utils.Identity.ImpersonateGhostUserViaHttpContext();
                Cartomatic.Utils.Identity.ImpersonateGhostUser();

                var org = await _db.Organizations.FirstOrDefaultAsync(o => o.Slug == orgSlug);
                if (org != null && clean)
                {
                    await org.DestroyAsync(_db, clean);

                    //users - destroying here, as need to work out where a user comes from
                    var users = await org.GetChildrenAsync<Organization, MapHiveUser>(_db);
                    foreach (var u in users)
                    {
                        //can only destroy users that were created by this organization!
                        if (u.IsOrgUser && u.ParentOrganizationId == org.Uuid)
                        {
                            await u.DestroyAsync(_db);
                        }
                    }
                }

                //need a user first
                var user = await CreateUserInternalAsync(email, pass, true);

                if (org == null || clean)
                {
                    //now should be able to create an org

                    //now the org object
                    var newOrg = new Organization
                    {
                        Slug = orgSlug,
                        Description = orgDescription
                    };

                    //create an org with owner; if morg, then register masterofpuppets
                    var apps = new string[0];
                    if (morg)
                        apps = new[] {"masterofpuppets"};

                    await newOrg.CreateAsync(_db, user, apps);
                }

                if (org != null && !clean)
                {
                    await org.AddAdminAsync(_db, user);
                }
                
                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Drops an organisation
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="clean"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("droporg")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DropOrgAsync([FromUri]Guid orgId, bool clean)
        {
            try
            {
                if (!await UserIsEnvAdminAsync())
                    return new UnauthorizedResult();

                //pretend this is an 'automated' user...
                Cartomatic.Utils.Identity.ImpersonateGhostUserViaHttpContext();
                Cartomatic.Utils.Identity.ImpersonateGhostUser();

                var org = await _db.Organizations.FirstOrDefaultAsync(o => o.Uuid == orgId);
                if (org == null)
                    return BadRequest("No such org");

                //users - destorying here, as need to work out where a user comes from
                var users = await org.GetChildrenAsync<Organization, MapHiveUser>(_db);
                foreach (var u in users)
                {
                    //can only destroy users that were created by this organization!
                    if (u.IsOrgUser && u.ParentOrganizationId == org.Uuid)
                    {
                        await u.DestroyAsync(_db);
                    }
                }


                //should issue a call to other apis so they cleanup themselves too...
                //note this call is not expected to return a success result, as some apis may simply not expose
                //such functionality, some may not be accessible
                //so basically this is a send & forget call.
                var apiApps = await _db.Applications.Where(a => a.IsApi).ToListAsync();

                //urls are pipe delimited
                //can ignore .local really as this is pretty much only dev mode
                foreach (var app in apiApps)
                {
                    var urls = app.Urls.Split('|');
                    foreach (var url in urls)
                    {
                        //discard the local dev urls
                        var urlOk = url.IndexOf(".local") == -1;

#if DEBUG
                        //in debug mode use all urls. this will just make the code make some pointless calls
                        urlOk = true;
#endif

                        if(!urlOk)
                            continue;

                        //calls must be performed with proper scope - need to set the same authorization header!
                        //simply send DELETE to api endpoint
                        //organizations/uuid

                        //await, this should not be too extreme. usually...

                        var endpointUrl =
                            $"{url}{(url.EndsWith("/") ? "" : "/")}envconfig/";

                        //object so deserialization does not fail
                        await RestApiCall<object>(
                            endpointUrl, //assuming this path exists. does not have to though; wil silently fail in such case
                            "droporg",
                            Method.DELETE,
                            queryParams: new Dictionary<string, object>
                            {
                                { "orgId", orgId.ToString() }
                            },
                            //add auth token
                            authToken: $"{Request.Headers.Authorization.Scheme} {Request.Headers.Authorization.Parameter}"
                        );
                    }
                }



                //finally destroy an org
                //need to keep it alive until all the apis perform a cleanup because they may need to consult the core for specific org db details

                //this should destroy org, its dbs, roles and all the related stuff
                await org.DestroyAsync(_db, clean);


                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }


        /// <summary>
        /// creates a user; destroys it first if exists
        /// </summary>
        /// <param name="email"></param>
        /// <param name="pass"></param>
        /// <param name="destroy"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createuser")]
        [ProducesResponseType(typeof(MapHiveUser),200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateUserAsync([FromUri] string email, [FromUri] string pass, [FromUri]bool destroy)
        {
            try
            {
                if (!await UserIsEnvAdminAsync())
                    return new UnauthorizedResult();

                //pretend this is an 'automated' user...
                Cartomatic.Utils.Identity.ImpersonateGhostUserViaHttpContext();
                Cartomatic.Utils.Identity.ImpersonateGhostUser();

                return Ok(await CreateUserInternalAsync(email, pass, destroy));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// create a user
        /// </summary>
        /// <param name="email"></param>
        /// <param name="pass"></param>
        /// <param name="destroy">Whether or not a user should be destroyed if exists. If false and user exists no user destroy/recreation happens</param>
        /// <returns></returns>
        private async Task<MapHiveUser> CreateUserInternalAsync(string email, string pass, bool destroy)
        {
            //when creating org, need a user and if it exists, need to clean it up
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null && destroy)
            {
                await user.ForceDestroyAsync<MapHiveUser>(_db, user.Uuid);
            }

            if (user == null || destroy)
            {
                //recreate user
                user = new MapHiveUser
                {
                    Email = email,
                    IsAccountVerified = true
                };

                //recreate user
                IDictionary<string, object> op = null;
                user.UserCreated += (sender, eventArgs) =>
                {
                    op = eventArgs.OperationFeedback;
                };

                await user.CreateAsync(_db);

                var userManager = MapHive.Core.Identity.UserManagerUtils.GetUserManager();
                var idUser = await userManager.FindByEmailAsync(email);
                //once user is created, need to perform an update in order to set it as valid
                var confirmEmailToken = await userManager.GenerateEmailConfirmationTokenAsync(idUser);
                await userManager.ConfirmEmailAsync(idUser, confirmEmailToken);

                //also apply pass!
                await Auth.ForceResetPasswordAsync(idUser.Id, pass);
            }
            
            return user;
        }


        /// <summary>
        /// registers tokens in the environment...
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("registertokens")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RegisterTokensAsync(List<Token> tokens)
        {
            try
            {
                if (!await UserIsEnvAdminAsync())
                    return new UnauthorizedResult();

                //pretend this is an 'automated' user...
                Cartomatic.Utils.Identity.ImpersonateGhostUserViaHttpContext();
                Cartomatic.Utils.Identity.ImpersonateGhostUser();

                //good to go
                var tokenIds = tokens.Select(t => t.Uuid).ToArray();
                var dbTokens = await _db.Tokens.Where(t => tokenIds.Contains(t.Uuid)).ToListAsync();

                //good to go
                foreach (var token in tokens)
                {
                    if (dbTokens.All(t => t.Uuid != token.Uuid))
                        _db.Tokens.Add(token);
                }
                await _db.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }


        /// <summary>
        /// registers emails templates in the nevironment...
        /// </summary>
        /// <param name="emailTemplates"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("registeremailtemplates")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RegisterEmailTemplatesAsync(List<MapHive.Core.DataModel.EmailTemplateLocalization> emailTemplates)
        {
            try
            {
                if (!await UserIsEnvAdminAsync())
                    return new UnauthorizedResult();

                //pretend this is an 'automated' user...
                Cartomatic.Utils.Identity.ImpersonateGhostUserViaHttpContext();
                Cartomatic.Utils.Identity.ImpersonateGhostUser();

                //good to go
                var emailTemplateIds = emailTemplates.Select(et => et.Uuid).ToArray();
                var dbEmailTemplates = await _db.EmailTemplates.Where(et => emailTemplateIds.Contains(et.Uuid)).ToListAsync();

                //good to go
                foreach (var emailTemplate in emailTemplates)
                {
                    if (dbEmailTemplates.All(et => et.Uuid != emailTemplate.Uuid))
                        _db.EmailTemplates.Add(emailTemplate);
                }
                await _db.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}