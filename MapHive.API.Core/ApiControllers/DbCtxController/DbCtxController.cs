using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Api.Core.ApiControllers
{
    /// <summary>
    /// Base controller with db ctx access
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    public abstract partial class DbCtxController<TDbContext> : BaseController, IDbCtxController
        where TDbContext : DbContext, new()
    {
        protected readonly TDbContext _db;
        

        protected DbCtxController()
        {
            _db = GetNewDefaultDbContext();
        }

        public TDbContext GetDefaultDbContext()
        {
            return _db;
        }

        protected TDbContext GetNewDefaultDbContext()
        {
            return new TDbContext();
        }

        DbContext IDbCtxController.GetDefaultDbContext()
        {
            return GetDefaultDbContext();
        }
    }
}