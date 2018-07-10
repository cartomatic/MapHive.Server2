using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MapHive.Api.Core.ApiControllers;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using MapHive.Server.Core.API;
using Microsoft.AspNetCore.Mvc;

namespace MapHive.Core.Api.Controllers
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
    }
}
