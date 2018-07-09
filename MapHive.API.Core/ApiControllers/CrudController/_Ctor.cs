using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Cartomatic.Utils.Data;
using Cartomatic.Utils.Ef;
using MapHive.API.Core.ApiControllers;
using MapHive.Core.DataModel;
using Microsoft.EntityFrameworkCore;

namespace MapHive.API.Core.ApiControllers
{
    /// <summary>
    /// Provides the base for the Web APIs that expose IBase like objects via RESTful like API
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TDbCtx">Context to be used for the basic CRUD ops; can always be substituted for particular method calls, as they usually have overloads that take in dbctx</typeparam>
    public abstract partial class CrudController<T, TDbCtx> : BaseController
        where T : Base
        where TDbCtx : DbContext, IProvideDbContextFactory, new()
    {
        /// <summary>
        /// Database context to be used
        /// </summary>
        protected TDbCtx _dbCtx { get; private set; }

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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _dbCtx.Dispose();

            base.Dispose(disposing);
        }
    }
}
