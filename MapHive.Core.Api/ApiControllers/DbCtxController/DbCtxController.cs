using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.Api.ApiControllers
{

    /// <summary>
    /// Base controller with db ctx access; the provided db context must be initialable via paramless ctor
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


        /// <summary>
        /// gets tge default db ctx
        /// </summary>
        /// <returns></returns>
        protected TDbContext GetDefaultDbContext()
        {
            return _db;
        }

        /// <summary>
        /// Gets a new instance of default db ctx
        /// </summary>
        /// <returns></returns>
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