﻿using Cartomatic.Utils.Data;
using Cartomatic.Utils.Ef;
using MapHive.Core.DataModel;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.Api.ApiControllers
{

    /// <summary>
    /// Provides the base for the Web APIs that expose IBase like objects via RESTful like API
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TDbCtx">Context to be used for the basic CRUD ops; can always be substituted for particular method calls, as they usually have overloads that take in dbctx</typeparam>
    public abstract partial class CrudController<T, TDbCtx> : BaseController, IDbCtxController
        where T : Base
        where TDbCtx : DbContext, IProvideDbContextFactory, new()
    {
        /// <summary>
        /// Database context to be used
        /// </summary>
        protected TDbCtx _dbCtx { get; private set; }

        protected TDbCtx GetDefaultDbContext()
        {
            return _dbCtx;
        }

        protected TDbCtx GetNewDefaultDbContext()
        {
            return new TDbCtx();
        }

        DbContext IDbCtxController.GetDefaultDbContext()
        {
            return GetDefaultDbContext();
        }

        protected CrudController()
            : this("MapHiveMetadata")
        {
        }

        protected CrudController(string connectionStringName)
        {
            //pass the conn string to the constructor.
            _dbCtx = default(TDbCtx);
            if (!string.IsNullOrEmpty(connectionStringName))
            {
                try
                {
                    var facadeDbCtx = Cartomatic.Utils.Ef.DbContextFactory.CreateDbContextFacade<TDbCtx>();
                    _dbCtx = (TDbCtx)facadeDbCtx.ProduceDbContextInstance(connectionStringName, false, DataSourceProvider.Npgsql);
                }
                catch
                {
                    //ignore
                }
            }

            if (_dbCtx == null)
            {
                _dbCtx = new TDbCtx();
            }
        }

        public override void Dispose()
        {
            _dbCtx.Dispose();

            base.Dispose();
        }
    }

}
