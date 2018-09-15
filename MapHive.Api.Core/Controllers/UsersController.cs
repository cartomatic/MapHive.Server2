﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MapHive.Core.Api.ApiControllers;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;

namespace MapHive.Api.Core.Controllers
{
    /// <summary>
    /// General User APIs
    /// </summary>
    [Route("users")]
    public class UsersController : CrudController<MapHiveUser, MapHiveDbContext>
    {
        /// <summary>
        /// Gets a list of users
        /// </summary>
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
        public async Task<IActionResult> GetAsync([FromQuery] string sort = null, [FromQuery] string filter = null, [FromQuery] int start = 0,
            [FromQuery] int limit = 25)
        {
            return await base.GetAsync(sort, filter, start, limit);
        }

        /// <summary>
        /// Gets a user by id
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{uuid}")]
        [ProducesResponseType(typeof(IEnumerable<MapHiveUser>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAsync([FromRoute] Guid uuid)
        {
            return await base.GetAsync(uuid);
        }

        /// <summary>
        /// Updates a user
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{uuid}")]
        [ProducesResponseType(typeof(MapHiveUser), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutAsync([FromBody] MapHiveUser obj, [FromRoute] Guid uuid)
        {
            return await base.PutAsync(obj, uuid);
        }


        /// <summary>
        /// returns details for a currently authenticated user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("owndetails")]
        [ProducesResponseType(typeof(MapHiveUser), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetOwnDetailsAsync()
        {
            var uuid = Cartomatic.Utils.Identity.GetUserGuid();

            //this should not happen really as otherwise the user would not be authenticated
            if (!uuid.HasValue)
            {
                return BadRequest();
            }

            try
            {
                var user = await Base.ReadObjAsync<MapHiveUser>(_dbCtx, uuid.Value);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Updates a record of a currently authenticated user
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("owndetails/{uuid}")]
        [ProducesResponseType(typeof(MapHiveUser), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateOwnDetailsAsync([FromBody] MapHiveUser obj, [FromRoute] Guid uuid)
        {
            var authUid = Cartomatic.Utils.Identity.GetUserGuid();

            //this should not happen really as otherwise the user would not be authenticated
            if (authUid != uuid)
            {
                return BadRequest();
            }

            try
            {
                await obj.UpdateAsync(_dbCtx, uuid);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                return this.HandleException(ex);
            }
        }

        /// <summary>
        /// Returns a list of organisations a user has an access to
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("userorgs")]
        [ProducesResponseType(typeof(IEquatable<Organization>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetUserOrgsAsync()
        {
            var uuid = Cartomatic.Utils.Identity.GetUserGuid();

            //this should not happen really as otherwise the user would not be authenticated
            if (!uuid.HasValue)
            {
                return BadRequest();
            }

            try
            {
                var orgs = await MapHiveUser.GetUserOrganizationsAsync(_dbCtx, uuid.Value);

                if (!orgs.Any())
                {
                    return NotFound();
                }

                return Ok(orgs);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Returns applications available to the current user; does not require auth, and for guests return a list of common apps.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("userapps")]
        [ProducesResponseType(typeof(IEnumerable<Application>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetUserAppsAsync()
        {
            try
            {
                return Ok(await MapHiveUser.GetUserAppsAsync(_dbCtx, Cartomatic.Utils.Identity.GetUserGuid(), GetRequestOrgIdentifier()));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}