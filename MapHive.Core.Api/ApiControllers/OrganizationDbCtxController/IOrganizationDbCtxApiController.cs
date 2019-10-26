using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.Api.ApiControllers
{
    /// <summary>
    /// enforces ability to provide org db ctx
    /// </summary>
    public interface IOrganizationDbCtxApiController
    {
        /// <summary>
        /// Returns org db context
        /// </summary>
        /// <param name="dbIdentifier"></param>
        /// <returns></returns>
        DbContext GetOrganizationDbContext(string dbIdentifier = null);
    }
}