using System;
using System.Threading.Tasks;
using Cartomatic.Utils.Email;
using MapHive.Core;
using MapHive.Core.Api.ApiControllers;
using MapHive.Core.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MapHive.Api.Core.Controllers
{
    /// <summary>
    /// Account APIs
    /// </summary>
    [Route("account")]
    [ApiExplorerSettings(IgnoreApi = true)] //make sure this api is not visible in docs!!! it's kinda private and while should be available it should not be freely used really
    public class AccountController : DbCtxController<MapHiveDbContext>
    {
        private IEmailSender EmailSender { get; set; }

        public AccountController(IEmailSender emailSender)
        {
            EmailSender = emailSender;
        }

        /// <summary>
        /// Creates an organization account
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("create")]
        [ProducesResponseType(typeof(AccountCreateOutput), 200)]
        public async Task<IActionResult> CreateAccountAsync([FromBody] AccountCreateInput input)
        {
            try
            {
                Cartomatic.Utils.Identity.ImpersonateGhostUser();
                var output = await MapHive.Core.Account.CreateAccountAsync(GetDefaultDbContext(), input, EmailSender);
                return Ok(output);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}