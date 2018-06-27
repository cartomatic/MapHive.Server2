using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DAL;
#if NETFULL
using System.Data.Entity;
#endif
#if NETSTANDARD
using Microsoft.EntityFrameworkCore;
#endif

namespace MapHive.Core.DataModel
{
    public abstract partial class Base
    {
        /// <summary>
        /// Helper method, check if object exist in a database
        /// </summary>
        /// <param name="dbSet"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected static async Task<bool> ObjectExistsAsync<T>(DbSet<T> dbSet, Guid? uuid) where T : Base
        {
            return await dbSet.Where(e => e.Uuid == uuid).CountAsync() > 0;
        }

        /// <summary>
        /// Gets a ILinksDbContext; returns null if ctx is not ILinksDbContext
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static ILinksDbContext GetLinksDbContext(DbContext db)
        {
            return  db as ILinksDbContext;
        }
    }
}
