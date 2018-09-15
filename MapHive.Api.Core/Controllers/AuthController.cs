using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cartomatic.Utils.Email;
using MapHive.Core;
using MapHive.Core.Api.ApiControllers;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Api.Core.Controllers
{
    /// <summary>
    /// Auth APIs
    /// </summary>
    [Produces("application/json")]
    [Route("auth")]
    public class AuthController : DbCtxController<MapHiveDbContext>
    {
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
            var accessToken = Request.Headers.Authorization.Parameter.Replace("Bearer ", "");

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
        /// <returns></returns>
        [Route("accountactivation/{app?}")]
        [HttpPut]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Auth.AccountActivationOutput), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> ActivateAccountAsync([FromBody] AccountActivationInput activationInput, [FromRoute] string app = null)
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
                var user = await GetDefaultDbContext().Users.FirstOrDefaultAsync(u => u.Uuid == Auth.ExtractIdFromMergedToken(activationOutput.VerificationKey));

                //since got an email off mbr, user should not be null, but just in a case...
                if (user == null)
                {
                    return BadRequest();
                }

                //need to resend email with new verification key, as the previous one was stale
                if (activationOutput.VerificationKeyStale)
                {

                    var (emailAccount, emailTemplate) = await GetEmailStuffAsync("activate_account_stale", app);

                    //prepare the email template tokens
                    var tokens = new Dictionary<string, object>
                    {
                        {"UserName", $"{user.GetFullUserName()} ({user.Email})"},
                        {"Email", user.Email},
                        {"RedirectUrl", this.GetRequestSource(Context).Split('#')[0]},
                        {"VerificationKey", activationOutput.VerificationKey},
                        {"InitialPassword", ""}
                    };

                    //prepare and send the email
                    EmailSender.Send(emailAccount, emailTemplate.Prepare(tokens), user.Email);
                }

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
        /// <returns></returns>
        [HttpPost]
        [Route("resendactivation/{uuid}/{app?}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> ResendActivationLinkAsync([FromRoute] Guid uuid, [FromRoute] string app = null)
        {
            try
            {
                var dbCtx = GetDefaultDbContext();

                //grab a user first
                var u = await new MapHiveUser().ReadAsync(dbCtx, uuid);

                //and make sure there is point in resending a link
                if (u.IsAccountVerified)
                    return BadRequest("User has already activated his account.");

                //get the request lang without re-setting a cookie. will be used to get a proper email template later
                var (emailAccount, emailTemplate) = await GetEmailStuffAsync("user_created", app);

                var initialEmailData = new Dictionary<string, object>
                {
                    {"UserName", $"{u.GetFullUserName()} ({u.Email})"},
                    {"Email", u.Email},
                    {"RedirectUrl", GetRequestSource(Context).Split('#')[0]}
                };

                await u.ResendActivationLinkAsync(
                    dbCtx,
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
        /// <returns></returns>
        [Route("passresetrequest/{app?}")]
        [HttpPut]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        public async Task<IActionResult> PassResetRequestAsync([FromBody] PassResetRequestInput input, [FromRoute] string app = null)
        {
            //Note: basically this is a pass reset request, so NO need to inform a potential attacker about exceptions - always return ok!

            if (input == null)
                return BadRequest();

            try
            {
                var requestPassResetOutput = await Auth.RequestPassResetAsync(input.Email);

                
                var (emailAccount, emailTemplate) = await GetEmailStuffAsync("pass_reset_request", app);

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
                        {"RedirectUrl", this.GetRequestSource(Context).Split('#')[0]},
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
    }
}
