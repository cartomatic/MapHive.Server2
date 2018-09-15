using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MapHive.Core.Api.ApiControllers;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using Microsoft.AspNetCore.Mvc;


namespace MapHive.Api.Core.Controllers
{
    /// <summary>
    /// TranslationKey APIs
    /// </summary>
    [Route("translationkeys")]
    public class TranslationKeysController : CrudController<TranslationKey, MapHiveDbContext>
    {
        /// <summary>
        /// Gets a collection of TranslationKeys
        /// </summary>
        /// <param name="sort"></param>
        /// <param name="filter"></param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(IEnumerable<TranslationKey>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAsync([FromQuery] string sort = null, [FromQuery] string filter = null, [FromQuery] int start = 0,
            [FromQuery] int limit = 25)
        {
            return await base.GetAsync(sort, filter, start, limit);
        }

        /// <summary>
        /// Gets a TranslationKey by id
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{uuid}")]
        [ProducesResponseType(typeof(TranslationKey), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAsync([FromQuery] Guid uuid)
        {
            return await base.GetAsync(uuid);
        }

        /// <summary>
        /// Updates a TranslationKey
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{uuid}")]
        [ProducesResponseType(typeof(TranslationKey), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutAsync([FromBody] TranslationKey obj, [FromRoute] Guid uuid)
        {
            return await base.PutAsync(obj, uuid);
        }

        /// <summary>
        /// Creates a new TranslationKey
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(TranslationKey), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostAsync(TranslationKey obj)
        {
            return await base.PostAsync(obj);
        }

        /// <summary>
        /// Deletes a TranslationKey
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{uuid}")]
        [ProducesResponseType(typeof(TranslationKey), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteAsync(Guid uuid)
        {
            return await base.DeleteAsync(uuid);
        }

    }
}