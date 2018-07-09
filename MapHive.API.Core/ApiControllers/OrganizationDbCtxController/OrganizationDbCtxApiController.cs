using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Http;
using MapHive.Core.DataModel;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Api.Core.ApiControllers
{
    /// <summary>
    /// This is basic controller that pulls org dbs during a filter phase
    /// </summary>
    /// <typeparam name="TDbContext">The default context to be used for the controller</typeparam>
    //[OrganizationContextActionFilter] - in the startup now so kicks in when required!
    public abstract class OrganizationDbCtxController<TDbContext> : DbCtxController<TDbContext>, IOrganizationApiController<TDbContext>, IOrganizationDbCtxApiController
        where TDbContext : DbContext, new()
    {
        private TDbContext _organizationDb;

        /// <summary>
        /// the default db identifier to be used by the controller
        /// </summary>
        protected string DbIdentifier { get; set; }

        /// <summary>
        /// Organization id from GetOrganizationDatabasesbActionFilterAttribute action filter
        /// </summary>
        public Guid OrganizationId => OrganizationContextActionFilterAttribute.GetOrganizationId(Context);


        /// <summary>
        /// Organization database object from GetOrganizationDatabasesbActionFilterAttribute action filter
        /// </summary>
        public OrganizationDatabase GetOrganizationDatabase(string dbIdentifier = null) =>
            OrganizationContextActionFilterAttribute.GetOrganisationDatabase(Context, string.IsNullOrWhiteSpace(dbIdentifier) ? DbIdentifier : dbIdentifier);

        
        /// <summary>
        /// Gets a default db context for an organisation
        /// </summary>
        /// <returns></returns>
        public TDbContext GetOrganizationDbContext(string dbIdentifier = null) =>
            _organizationDb ?? (_organizationDb = GetOrganizationDatabase(dbIdentifier)?.GetDbContext<TDbContext>());


        /// <summary>
        /// Grets a specified db ctx
        /// </summary>
        /// <typeparam name="TDbCtx"></typeparam>
        /// <param name="dbIdentifier"></param>
        /// <returns></returns>
        public TDbCtx GetOrganizationDbContext<TDbCtx>(string dbIdentifier = null)
            where TDbCtx: DbContext, new() => GetOrganizationDatabase(dbIdentifier)?.GetDbContext<TDbCtx>();



        /// <summary>
        /// Gets a user / token organisation context
        /// </summary>
        /// <param name="dbIdentifier"></param>
        /// <returns></returns>
        public Organization GetOrganizationContext(string dbIdentifier = null)
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