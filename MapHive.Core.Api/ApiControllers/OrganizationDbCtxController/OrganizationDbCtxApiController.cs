using Cartomatic.Utils.Ef;
using MapHive.Core.DataModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace MapHive.Core.Api.ApiControllers
{
    /// <summary>
    /// This is basic controller that pulls org dbs during a filter phase
    /// </summary>
    /// <typeparam name="TDbContext">The default context to be used for the controller</typeparam>
    //[OrganizationContextActionFilter] - in the startup now so kicks in when required!
    public abstract class OrganizationDbCtxController<TDbContext> : DbCtxController<TDbContext>, IOrganizationApiController<TDbContext>, IOrganizationDbCtxApiController, IDbCtxController
        where TDbContext : DbContext, IProvideDbContextFactory, new()
    {
        private TDbContext _organizationDb;

        /// <summary>
        /// the default db identifier to be used by the controller
        /// </summary>
        protected virtual string DbIdentifier { get; set; }

        /// <summary>
        /// Organization id from GetOrganizationDatabasesbActionFilterAttribute action filter
        /// </summary>
        protected Guid OrganizationId => OrganizationContextActionFilterAttribute.GetOrganizationId(HttpContext);

        Guid IOrganizationApiController<TDbContext>.OrganizationId => OrganizationContextActionFilterAttribute.GetOrganizationId(HttpContext);

        /// <summary>
        /// Organization database object from GetOrganizationDatabasesbActionFilterAttribute action filter
        /// </summary>
        protected OrganizationDatabase GetOrganizationDatabase(string dbIdentifier = null) =>
            OrganizationContextActionFilterAttribute.GetOrganisationDatabase(HttpContext, string.IsNullOrWhiteSpace(dbIdentifier) ? DbIdentifier : dbIdentifier);

        OrganizationDatabase IOrganizationApiController<TDbContext>.GetOrganizationDatabase(string dbIdentifier = null) =>
            OrganizationContextActionFilterAttribute.GetOrganisationDatabase(HttpContext, string.IsNullOrWhiteSpace(dbIdentifier) ? DbIdentifier : dbIdentifier);

        /// <summary>
        /// Gets a default db context for an organisation
        /// </summary>
        /// <returns></returns>
        protected TDbContext GetOrganizationDbContext(string dbIdentifier = null) =>
            _organizationDb ?? (_organizationDb = GetOrganizationDatabase(dbIdentifier)?.GetDbContext<TDbContext>());

        TDbContext IOrganizationApiController<TDbContext>.GetOrganizationDbContext(string dbIdentifier = null) =>
            _organizationDb ?? (_organizationDb = GetOrganizationDatabase(dbIdentifier)?.GetDbContext<TDbContext>());

        /// <summary>
        /// Grets a specified db ctx
        /// </summary>
        /// <typeparam name="TDbCtx"></typeparam>
        /// <param name="dbIdentifier"></param>
        /// <returns></returns>
        protected TDbCtx GetOrganizationDbContext<TDbCtx>(string dbIdentifier = null)
            where TDbCtx : DbContext, IProvideDbContextFactory, new() => GetOrganizationDatabase(dbIdentifier)?.GetDbContext<TDbCtx>();

        TDbCtx IOrganizationApiController<TDbContext>.GetOrganizationDbContext<TDbCtx>(string dbIdentifier = null) => GetOrganizationDatabase(dbIdentifier)?.GetDbContext<TDbCtx>();


        /// <summary>
        /// Gets a user / token organisation context
        /// </summary>
        /// <param name="dbIdentifier"></param>
        /// <returns></returns>
        protected Organization GetOrganizationContext(string dbIdentifier = null)
        {
            return UserConfiguration?.Orgs?.FirstOrDefault(org => org.Uuid == OrganizationId);
        }

        /// <summary>
        /// Returns org db context
        /// </summary>
        /// <param name="dbIdentifier"></param>
        /// <returns></returns>
        DbContext IOrganizationDbCtxApiController.GetOrganizationDbContext(string dbIdentifier)
        {
            return GetOrganizationDbContext(dbIdentifier);
        }

    }
}