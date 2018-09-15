using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MapHive.Core.Api.ApiControllers;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;

namespace MapHive.Api.Core.Controllers
{
    /// <summary>
    /// EmailTemplateLocalization APIs
    /// </summary>
    [Route("emailtemplatelocalizations")]
    public class EmailTemplateLocalizationsController : CrudController<EmailTemplateLocalization, MapHiveDbContext>
    {
        /// <summary>
        /// Gets a collection of EmailTemplateLocalizations
        /// </summary>
        /// <param name="sort"></param>
        /// <param name="filter"></param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(IEnumerable<EmailTemplateLocalization>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAsync([FromQuery] string sort = null, [FromQuery] string filter = null, [FromQuery] int start = 0,
            [FromQuery] int limit = 25)
        {
            return await base.GetAsync(sort, filter, start, limit);
        }

        /// <summary>
        /// Gets an EmailTemplateLocalization by id
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{uuid}")]
        [ProducesResponseType(typeof(EmailTemplateLocalization), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAsync([FromQuery] Guid uuid)
        {
            return await base.GetAsync(uuid);
        }

        /// <summary>
        /// Updates an EmailTemplateLocalization
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{uuid}")]
        [ProducesResponseType(typeof(EmailTemplateLocalization), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutAsync([FromBody] EmailTemplateLocalization obj, [FromRoute] Guid uuid)
        {
            return await base.PutAsync(obj, uuid);
        }

        /// <summary>
        /// Creates a new EmailTemplateLocalization
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(EmailTemplateLocalization), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostAsync(EmailTemplateLocalization obj)
        {
            return await base.PostAsync(obj);
        }

        /// <summary>
        /// Deletes an EmailTemplateLocalization
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{uuid}")]
        [ProducesResponseType(typeof(EmailTemplateLocalization), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteAsync(Guid uuid)
        {
            return await base.DeleteAsync(uuid);
        }

        /// <summary>
        /// Gets app specific email template in given language;
        /// when template for the language specified is not found it defaults to a template in a default enf language and if it is not defined a first template translation (if any!) is returned
        /// </summary>
        /// <param name="emailIdentifier"></param>
        /// <param name="appIdentifier"></param>
        /// <param name="langCode"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("emailtemplate")]
        [ProducesResponseType(typeof(EmailTemplate), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetEmailTemplate([FromQuery] string emailIdentifier, [FromQuery] string appIdentifier, [FromQuery] string langCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(emailIdentifier))
                    return BadRequest();

                var et = await EmailTemplateLocalization.GetEmailTemplate(GetDefaultDbContext(), emailIdentifier, appIdentifier, langCode);
                
                if(et != null)
                    return Ok(et);

                return NotFound();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
