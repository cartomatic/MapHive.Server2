using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DAL
{
    /// <summary>
    /// A utility for automated extended views creation
    /// </summary>
    public partial class ExtendedViewsCreator
    {
        /// <summary>
        /// Creates extended views for a db context
        /// </summary>
        /// <param name="dbCtx"></param>
        public static void CreateAll(DbContext dbCtx)
        {
            if (dbCtx is IProvideExtendedViews extViewsDbCtx && extViewsDbCtx is BaseDbContext baseExtViewsDbContext)
                CreateAll(baseExtViewsDbContext, extViewsDbCtx.ExtendedViewsProvider);
        }

        /// <summary>
        /// Executes all the configured extended views create procedures
        /// </summary>
        /// <typeparam name="TDbCtx"></typeparam>
        /// <typeparam name="TSeed"></typeparam>
        /// <param name="dbCtx"></param>
        public static void CreateAll<TDbCtx, TSeed>(TDbCtx dbCtx)
            where TDbCtx : BaseDbContext
        {
            CreateAll(dbCtx, typeof(TSeed));
        }


        /// <summary>
        ///  Executes all the configured extended views create procedures
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCtx"></param>
        /// <param name="t">type to execute views creation for</param>
        public static void CreateAll<T>(T dbCtx, Type t)
            where T : BaseDbContext
        {
            var mi = t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            foreach (var m in mi)
            {
                if (m.Name != nameof(CreateAll) && m.Name.StartsWith("CreateView"))
                {
                    //assume there is one param for the time being
                    //may have to change it later
                    m.Invoke(null, new object[] { dbCtx });
                }
            }
        }
    }
}
