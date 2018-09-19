using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Ef;
using MapHive.Core.DataModel;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.Api.ApiControllers
{
    /// <summary>
    /// enforces some basic functionality for org controllers
    /// </summary>
    public interface IOrganizationApiController<out TMainDbCtx>
        where TMainDbCtx : DbContext, new()
    {
        /// <summary>
        /// Id of an org the instance of a controller is coped to
        /// </summary>
        Guid OrganizationId { get; }

        /// <summary>
        /// Organization database if any; uses the Identifier field to identify the db settings to extract
        /// </summary>
        OrganizationDatabase GetOrganizationDatabase(string dbIdentifier = null);

        /// <summary>
        /// Gets a default organisation db context configured for the controller
        /// </summary>
        /// <param name="dbIdentifier"></param>
        /// <returns></returns>
        TMainDbCtx GetOrganizationDbContext(string dbIdentifier = null);

        /// <summary>
        /// Gets a specified organisation db context
        /// </summary>
        /// <typeparam name="TDbCtx"></typeparam>
        /// <param name="dbIdentifier"></param>
        /// <returns></returns>
        TDbCtx GetOrganizationDbContext<TDbCtx>(string dbIdentifier = null)
            where TDbCtx : DbContext, IProvideDbContextFactory, new();
    }
}
