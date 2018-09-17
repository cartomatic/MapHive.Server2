using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Http;
using Cartomatic.Utils.Ef;
using MapHive.Core.DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.Api.ApiControllers
{
    /// <summary>
    /// Base API Organisation CRUD controller is used to to get the org specific db context when interacting with the api at an organisation level
    /// <para />
    /// Similar to the base api org dbctx controller, but adds the crud ops
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TDbContext"></typeparam>
    //[OrganizationContextActionFilter] - in the startup now so kicks in when required!
    public abstract partial class OrganizationCrudController<T, TDbContext> : CrudController<T, TDbContext>, IOrganizationApiController<TDbContext>
        where T : Base, new()
        where TDbContext : DbContext, IProvideDbContextFactory, new()
    {
        private TDbContext _organizationDb;

        /// <summary>
        /// the default db identifier to be used by the controller; should be customized for derived apis. its purpose is to allow db load distribution if required.
        /// each api looks up a db by key in the dbs property of an organization. when this is not found a default org db as configured in app settings is used; when found thoug
        /// all the org db ops are performed against a specified db
        /// </summary>
        protected virtual string DbIdentifier { get; set; } = "maphive_meta";

        /// <summary>
        /// Organization id from OrganizationContextActionFilterAttribute action filter
        /// </summary>
        public Guid OrganizationId => OrganizationContextActionFilterAttribute.GetOrganizationId(Context);


        public Organization OrganizationContext => OrganizationContextActionFilterAttribute.GetOrganizationContext(Context);

        /// <summary>
        /// Organization database object from GetOrganizationDatabasesbActionFilterAttribute action filter
        /// </summary>
        public OrganizationDatabase GetOrganizationDatabase(string dbIdentifier = null) =>
            OrganizationContextActionFilterAttribute.GetOrganisationDatabase(Context, string.IsNullOrWhiteSpace(dbIdentifier) ? DbIdentifier : dbIdentifier);


        /// <summary>
        /// Gets a default db context for an organization
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
            where TDbCtx : DbContext, new() => GetOrganizationDatabase(dbIdentifier)?.GetDbContext<TDbCtx>();

    }
}