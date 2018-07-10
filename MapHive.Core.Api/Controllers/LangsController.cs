using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MapHive.Api.Core.ApiControllers;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MapHive.Core.Api.Controllers
{
    /// <summary>
    /// Lang APIs
    /// </summary>
    [Route("langs")]
    public class LangsController : CrudController<Lang, MapHiveDbContext>
    {
        /// <summary>
        /// Gets a collection of Langs
        /// </summary>
        /// <param name="sort"></param>
        /// <param name="filter"></param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(IEnumerable<Lang>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAsync([FromQuery] string sort = null, [FromQuery] string filter = null, [FromQuery] int start = 0,
            [FromQuery] int limit = 25)
        {
            return await base.GetAsync(sort, filter, start, limit);
        }

        /// <summary>
        /// Gets a Lang by id
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{uuid}")]
        [ProducesResponseType(typeof(Lang), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAsync([FromQuery] Guid uuid)
        {
            return await base.GetAsync(uuid);
        }

        /// <summary>
        /// Updates a Lang
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{uuid}")]
        [ProducesResponseType(typeof(Lang), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutAsync([FromBody] Lang obj, [FromRoute] Guid uuid)
        {
            return await base.PutAsync(obj, uuid);
        }

        /// <summary>
        /// Creates a new Lang
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(Lang), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostAsync(Lang obj)
        {
            return await base.PostAsync(obj);
        }

        /// <summary>
        /// Deletes a Lang
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{uuid}")]
        [ProducesResponseType(typeof(Lang), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteAsync(Guid uuid)
        {
            return await base.DeleteAsync(uuid);
        }

        /// <summary>
        /// Gets a default lang
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("default")]
        [ProducesResponseType(typeof(Lang), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetDefaultLangAsync()
        {
            try
            {
                return Ok(await Lang.GetDefaultLangAsync(_dbCtx as MapHiveDbContext));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Gets a default lang code
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("default/langcode")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetDefaultLangCodeAsync()
        {
            try
            {
                return Ok((await Lang.GetDefaultLangAsync(_dbCtx as MapHiveDbContext))?.LangCode);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Gets supported langs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("supported")]
        [ProducesResponseType(typeof(IEnumerable<Lang>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetSupportedLangsAsync()
        {
            try
            {
                return Ok(await Lang.GetSupportedLangsAsync(_dbCtx as MapHiveDbContext));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Returns supproted lang codes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("supported/langcodes")]
        [ProducesResponseType(typeof(IEnumerable<string>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetSupportedLangCodesAsync()
        {
            try
            {
                return Ok((await Lang.GetSupportedLangsAsync(_dbCtx as MapHiveDbContext)).Select(l => l.LangCode));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
