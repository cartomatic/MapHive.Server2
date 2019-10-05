using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Cartomatic.Utils.Email;
using IdentityServer4.Models;
using MapHive.Core;
using MapHive.Core.Api.ApiControllers;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MapHive.Api.Core.Controllers
{
    /// <summary>
    /// Auth APIs
    /// </summary>
    [Produces("application/json")]
    [Route("auth")]
    public class AuthController : DbCtxController<MapHiveDbContext>
    {
        private IEmailSender EmailSender { get; set; }

#pragma warning disable 1591
        public AuthController(IEmailSender emailSender)
        {
            EmailSender = emailSender;
        }

        /// <summary>
        /// Authenticates user; output returned, if successful contains access and refresh tokens
        /// </summary>
        /// <param name="email"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("letmein")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Auth.AuthOutput), 200)]
        public async Task<IActionResult> LetMeInAsync(
            [FromQuery] string email,
            [FromQuery] string pass
            )
        {
            return Ok(await Auth.LetMeInAsync(email, pass));
        }

        /// <summary>
        /// Finalises user session on the idsrv
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("letmeoutofhere")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> LetMeOutOfHereAsync()
        {
            //extract access token off the request
            var accessToken = (Request.Headers.ContainsKey("Authorization") ? Request.Headers["Authorization"].First() : string.Empty).Replace("Bearer ", "");

            await Auth.LetMeOutOfHereAsync(accessToken);
            return Ok();
        }

        /// <summary>
        /// Validates access token
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        [Route("tokenvalidation")]
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Auth.AuthOutput), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ValidateTokenAsync([FromQuery] string accessToken)
        {
            var tokenValidationOutput = await Auth.ValidateTokenAsync(accessToken);
            if (tokenValidationOutput.Success)
            {
                return Ok(tokenValidationOutput);
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Refreshes auth tokens - access token and refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("refresh")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Auth.AuthOutput), 200)]
        public async Task<IActionResult> RefreshTokensAsync([FromQuery] string refreshToken)
        {
            return Ok(await Auth.RefreshTokensAsync(refreshToken));
        }

        /// <summary>
        /// Account activation input dto
        /// </summary>
        public class AccountActivationInput
        {
            /// <summary>
            /// email confiration token issued upon account creation
            /// </summary>
            public string VerificationKey { get; set; }
        }

        /// <summary>
        /// account activation handler
        /// </summary>
        /// <param name="activationInput"></param>
        /// <param name="app"></param>
        /// <param name="ea">Email account details if need to send out emails using a custom account</param>
        /// <returns></returns>
        [Route("accountactivation/{app?}")]
        [HttpPut]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Auth.AccountActivationOutput), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> ActivateAccountAsync([FromBody] AccountActivationInput activationInput, [FromRoute] string app = null, [FromQuery] EmailAccount ea = null)
        {
            try
            {
                //work out user id from token
                var activationOutput = await Auth.ActivateAccountAsync(activationInput.VerificationKey);

                //aspnet identity has not found a user, so bad, bad, bad request it was
                if (activationOutput.UnknownUser)
                {
                    return BadRequest();
                }

                //basically need to send an email the verification key has expired and send a new one
                var user = await GetDefaultDbContext().Users.FirstOrDefaultAsync(u => u.Uuid == Auth.ExtractIdFromMergedToken(activationInput.VerificationKey));

                //since got an email off mbr, user should not be null, but just in a case...
                if (user == null)
                {
                    return BadRequest();
                }

                //need to resend email with new verification key, as the previous one was stale
                if (activationOutput.VerificationKeyStale)
                {

                    var (emailAccount, emailTemplate) = await GetEmailStuffAsync("activate_account_stale", app);

                    //use custom email account if provided
                    if (ea != null && ea.SeemsComplete())
                        emailAccount = ea;

                    //prepare the email template tokens
                    var tokens = new Dictionary<string, object>
                    {
                        {"UserName", $"{user.GetFullUserName()} ({user.Email})"},
                        {"Email", user.Email},
                        {"RedirectUrl", this.GetRequestSource(HttpContext).Split('#')[0]},
                        {"VerificationKey", activationOutput.VerificationKey},
                        {"InitialPassword", ""}
                    };

                    //prepare and send the email
                    EmailSender.Send(emailAccount, emailTemplate.Prepare(tokens), user.Email);
                }

                //going to save, so need to impersonate
                Cartomatic.Utils.Identity.ImpersonateGhostUser();

                //mark user rec as activated
                if (activationOutput.Success)
                {
                    user.IsAccountVerified = true;
                    Cartomatic.Utils.Identity.ImpersonateUserViaHttpContext(user.Uuid); //nee to impersonate, as otherwise dbctx will fail to save changes!
                    await user.UpdateAsync(GetDefaultDbContext());
                }

                //wipe out some potentially sensitive data
                activationOutput.Email = null;
                activationOutput.VerificationKey = null;

                return Ok(activationOutput);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Resends an activation email for a user
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="app">application (app short name) to use for the email localization</param>
        /// <param name="ea">Email account details if need to send out emails using a custom account</param>
        /// <returns></returns>
        [HttpPost]
        [Route("resendactivation/{uuid}/{app?}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> ResendActivationLinkAsync([FromRoute] Guid uuid, [FromRoute] string app = null, [FromQuery] EmailAccount ea = null)
        {
            try
            {
                var dbCtx = GetDefaultDbContext();

                //grab a user first
                var u = await new MapHiveUser().ReadAsync(dbCtx, uuid);

                if (u == null)
                    return BadRequest("Unknown user.");

                //and make sure there is point in resending a link
                if (u.IsAccountVerified)
                    return BadRequest("User has already activated his account.");

                //get the request lang without re-setting a cookie. will be used to get a proper email template later
                var (emailAccount, emailTemplate) = await GetEmailStuffAsync("user_created", app);

                //use custom email account if provided
                if (ea != null && ea.SeemsComplete())
                    emailAccount = ea;

                var initialEmailData = new Dictionary<string, object>
                {
                    {"UserName", $"{u.GetFullUserName()} ({u.Email})"},
                    {"Email", u.Email},
                    {"RedirectUrl", GetRequestSource(HttpContext).Split('#')[0]}
                };

                await u.ResendActivationLinkAsync(
                    dbCtx,
                    EmailSender,
                    emailAccount,
                    emailTemplate.Prepare(initialEmailData)
                );

                return Ok();
            }
            catch (Exception ex)
            {
                return this.HandleException(ex);
            }
        }


        /// <summary>
        /// pass reset request input dto
        /// </summary>
        public class PassResetRequestInput
        {
            /// <summary>
            /// Email
            /// </summary>
            public string Email { get; set; }
        }

        /// <summary>
        /// pass reset request handler
        /// </summary>
        /// <param name="input"></param>
        /// <param name="app">Application context to send appropriately translated emails</param>
        /// <param name="ea">Email account details if need to send out emails using a custom account</param>
        /// <returns></returns>
        [Route("passresetrequest/{app?}")]
        [HttpPut]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        public async Task<IActionResult> PassResetRequestAsync([FromBody] PassResetRequestInput input, [FromRoute] string app = null, [FromQuery] EmailAccount ea = null)
        {
            //Note: basically this is a pass reset request, so NO need to inform a potential attacker about exceptions - always return ok!

            if (input == null)
                return BadRequest();

            try
            {
                var requestPassResetOutput = await Auth.RequestPassResetAsync(input.Email);

                
                var (emailAccount, emailTemplate) = await GetEmailStuffAsync("pass_reset_request", app);

                //use custom email account if provided
                if (ea != null && ea.SeemsComplete())
                    emailAccount = ea;

                //basically need to send an email the verification key has expired and send a new one
                var user = await GetDefaultDbContext().Users.FirstOrDefaultAsync(u => u.Email == input.Email);

                //since got here and email off mbr, user should not be null, but just in a case...
                if (user == null)
                {
                    //return BadRequest();
                    return Ok();
                }

                //prepare the email template tokens
                var tokens = new Dictionary<string, object>
                {
                        {"UserName", $"{user.GetFullUserName()} ({user.Email})"},
                        {"Email", user.Email},
                        {"RedirectUrl", this.GetRequestSource(HttpContext).Split('#')[0]},
                        {"VerificationKey", requestPassResetOutput.VerificationKey}
                    };

                //prepare and send the email
                EmailSender.Send(emailAccount, emailTemplate.Prepare(tokens), user.Email);

                return Ok();
            }
            catch //(Exception ex)
            {
                //return HandleException(ex);
                return Ok();
            }
        }

        /// <summary>
        /// pass reset input dto
        /// </summary>
        public class PassResetInput
        {
            /// <summary>
            /// New password
            /// </summary>
            public string NewPass { get; set; }

            /// <summary>
            /// password reset verification key
            /// </summary>
            public string VerificationKey { get; set; }
        }

        /// <summary>
        /// Resets user password
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Route("resetpass")]
        [HttpPut]
        [AllowAnonymous]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> ChangePasswordFromResetKeyAsync([FromBody] PassResetInput input)
        {
            //Note: basically this is a pass reset request, so NO need to inform a potential attacker about exceptions - always return ok!

            try
            {
                var resetPassSuccess =
                    await Auth.ChangePasswordFromResetKeyAsync(input.NewPass, input.VerificationKey);

                //Note: if handle to get here, then pass should be reset

                return Ok(resetPassSuccess);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// pass change input dto
        /// </summary>
        public class ChangePassInput
        {
            /// <summary>
            /// Old pass
            /// </summary>
            public string OldPass { get; set; }

            /// <summary>
            /// new pass
            /// </summary>
            public string NewPass { get; set; }
        }


        /// <summary>
        /// Changes user password
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Route("changepass")]
        [HttpPut]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePassInput input)
        {
            //Note: basically this is a pass reset request, so NO need to inform a potential attacker about exceptions - always return ok!

            try
            {
                var resetPassSuccess =
                    await Auth.ChangePasswordAsync(input.NewPass, input.OldPass);

                return Ok(resetPassSuccess);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public class ForceChangePassInput
        {
            /// <summary>
            /// Security token that makes it possible to call this api
            /// </summary>
            public string Token { get; set; }

            /// <summary>
            /// Identifier of a user to change the pass for
            /// </summary>
            public Guid UserId { get; set;}

            /// <summary>
            /// New password to be set for a user
            /// </summary>
            public string NewPass { get; set; }
        }

        /// <summary>
        /// Allows changing password for an arbitrary user. Requires an extra security token in order to be used.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Route("forcechangepass")]
        [HttpPut]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> ForceChangePasswordAsync([FromBody] ForceChangePassInput input)
        {
            try
            {
                //IMPORTANT
                //make sure the extra security token is allowed, as this api can change pass for any user
                var cfg = Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig();
                var token = cfg.GetSection("AccessTokens:Auth").Get<string>();

                if (token != input.Token)
                    return StatusCode((int) HttpStatusCode.Unauthorized);


                var output = await Auth.ForceResetPasswordAsync(input.UserId, input.NewPass);

                return Ok(output);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
