using Microsoft.EntityFrameworkCore;

namespace MapHive.API.Core.ApiControllers
{
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