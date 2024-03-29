﻿using Cartomatic.Utils.Ef;
using MapHive.Core.DataModel;
using Microsoft.EntityFrameworkCore;
using System;

namespace MapHive.Core.Api.ApiControllers
{
    /// <summary>
    /// Base API Organisation CRUD controller is used to to get the org specific db context when interacting with the api at an organisation level
    /// <para />
    /// Similar to the base api org dbctx controller, but adds the crud ops
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TDbContext"></typeparam>
    //[OrganizationContextActionFilter] - in the startup now so kicks in when required - basically before dependant filters such as user config!
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
        protected virtual string DbIdentifier { get; set; }

        /// <summary>
        /// Organization id from OrganizationContextActionFilterAttribute action filter
        /// </summary>
        protected Guid OrganizationId => OrganizationContextActionFilterAttribute.GetOrganizationId(HttpContext);

        Guid IOrganizationApiController<TDbContext>.OrganizationId => OrganizationContextActionFilterAttribute.GetOrganizationId(HttpContext);

        /// <summary>
        /// gets organization that is a context for given action
        /// </summary>
        protected Organization OrganizationContext => OrganizationContextActionFilterAttribute.GetOrganizationContext(HttpContext);

        /// <summary>
        /// Resets organization context forcing the api to refresh it for the subsequent calls
        /// </summary>
        /// <param name="orgId"></param>
        protected void ResetOrganizationContext(Guid orgId) =>
            OrganizationContextActionFilterAttribute.ResetOrganizationContext(orgId);

        /// <summary>
        /// Organization database object from GetOrganizationDatabasesbActionFilterAttribute action filter
        /// </summary>
        protected OrganizationDatabase GetOrganizationDatabase(string dbIdentifier = null) =>
            OrganizationContextActionFilterAttribute.GetOrganisationDatabase(HttpContext, string.IsNullOrWhiteSpace(dbIdentifier) ? DbIdentifier : dbIdentifier);

        OrganizationDatabase IOrganizationApiController<TDbContext>.GetOrganizationDatabase(string dbIdentifier) =>
            OrganizationContextActionFilterAttribute.GetOrganisationDatabase(HttpContext, string.IsNullOrWhiteSpace(dbIdentifier) ? DbIdentifier : dbIdentifier);


        /// <summary>
        /// Gets a default db context for an organization
        /// </summary>
        /// <returns></returns>
        protected TDbContext GetOrganizationDbContext(string dbIdentifier = null) =>
            _organizationDb ??= GetOrganizationDatabase(dbIdentifier)?.GetDbContext<TDbContext>();

        TDbContext IOrganizationApiController<TDbContext>.GetOrganizationDbContext(string dbIdentifier) =>
            _organizationDb ??= GetOrganizationDatabase(dbIdentifier)?.GetDbContext<TDbContext>();

        /// <summary>
        /// Gets OrganizationDbContext in a safe manner - does not throw if the org dbCtx cannot be worked out
        /// </summary>
        /// <param name="dbIdentifier"></param>
        /// <returns></returns>
        protected TDbContext GetOrganizationDbContextSafe(string dbIdentifier = null)
        {
            try
            {
                return GetOrganizationDbContext(dbIdentifier);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Grets a specified db ctx
        /// </summary>
        /// <typeparam name="TDbCtx"></typeparam>
        /// <param name="dbIdentifier"></param>
        /// <returns></returns>
        protected TDbCtx GetOrganizationDbContext<TDbCtx>(string dbIdentifier = null)
            where TDbCtx : DbContext, IProvideDbContextFactory, new() => GetOrganizationDatabase(dbIdentifier)?.GetDbContext<TDbCtx>();

        TDbCtx IOrganizationApiController<TDbContext>.GetOrganizationDbContext<TDbCtx>(string dbIdentifier) => GetOrganizationDatabase(dbIdentifier)?.GetDbContext<TDbCtx>();
    }
}