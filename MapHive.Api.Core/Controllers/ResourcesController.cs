using MapHive.Core.Api.ApiControllers;
using MapHive.Core.DAL;
using MapHive.Core.DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace MapHive.Api.Core.Controllers
{
    /// <summary>
    /// Generic resource APIs
    /// </summary>
    [Route("resources")]
    public class ResourcesController : CrudController<Resource, MapHiveDbContext>
    {
        /// <summary>
        /// Returns a specified resource as a file stream result
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{uuid}/file")]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        //[CrudPrivilegeRequiredRead] //not that important as need to know resource uuid
        public async Task<IActionResult> GetAsWebResourceAsync([FromRoute] Guid uuid)
        {
            try
            {
                var res = await _dbCtx.Resources.AsNoTracking().FirstOrDefaultAsync(r => r.Uuid == uuid);

                if (res == null)
                    return NotFound();

                return new FileContentResult(res.Data, res.Mime)
                {
                    FileDownloadName = res.OriginalFileName
                };
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}