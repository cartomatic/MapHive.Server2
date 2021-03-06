﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MapHive.Core.Api.ApiControllers;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using Microsoft.AspNetCore.Mvc;


namespace MapHive.Api.Core.Controllers
{
    /// <summary>
    /// LocalizationClass APIs
    /// </summary>
    [Route("localizationclasses")]
    public class LocalizationClassesController : CrudController<LocalizationClass, MapHiveDbContext>
    {
        /// <summary>
        /// Gets a collection of LocalizationClasses
        /// </summary>
        /// <param name="sort"></param>
        /// <param name="filter"></param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(IEnumerable<LocalizationClass>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [CrudPrivilegeRequiredRead]
        public async Task<IActionResult> GetAsync([FromQuery] string sort = null, [FromQuery] string filter = null, [FromQuery] int start = 0,
            [FromQuery] int limit = 25)
        {
            return await base.GetAsync(sort, filter, start, limit);
        }

        /// <summary>
        /// Gets a LocalizationClass by id
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{uuid}")]
        [ProducesResponseType(typeof(LocalizationClass), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [CrudPrivilegeRequiredRead]
        public async Task<IActionResult> GetAsync([FromRoute] Guid uuid)
        {
            return await base.GetAsync(uuid);
        }

        /// <summary>
        /// Updates a LocalizationClass
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{uuid}")]
        [ProducesResponseType(typeof(LocalizationClass), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [CrudPrivilegeRequiredUpdate]
        public async Task<IActionResult> PutAsync([FromBody] LocalizationClass obj, [FromRoute] Guid uuid)
        {
            return await base.PutAsync(obj, uuid);
        }

        /// <summary>
        /// Creates a new LocalizationClass
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(LocalizationClass), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [CrudPrivilegeRequiredCreate]
        public async Task<IActionResult> PostAsync([FromBody] LocalizationClass obj)
        {
            return await base.PostAsync(obj);
        }

        /// <summary>
        /// Deletes a LocalizationClass
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{uuid}")]
        [ProducesResponseType(typeof(LocalizationClass), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [CrudPrivilegeRequiredDestroy]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid uuid)
        {
            return await base.DeleteAsync(uuid);
        }

    }
}