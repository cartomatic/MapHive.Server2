﻿using MapHive.Core.Api.ApiControllers;
using MapHive.Core.DAL;
using MapHive.Core.DataModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace MapHive.Api.Core.Controllers
{
    /// <summary>
    /// General tokens APIs
    /// </summary>
    [Route("tokens")]
    public class TokensController : CrudController<Token, MapHiveDbContext>
    {
        /// <summary>
        /// Checks if a token is present in the system and its due date is ok
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("{tokenid}/validation")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> ValidateTokenAsync([FromRoute] Guid tokenid)
        {
            try
            {
                //grab a token
                var token = await _dbCtx.Tokens.FirstOrDefaultAsync(t => t.Uuid == tokenid);
                return Ok(token != null && (token.EndDateUtc == null || token.EndDateUtc >= DateTime.Now.ToUniversalTime()));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}