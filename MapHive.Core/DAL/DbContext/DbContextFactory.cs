using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MapHive.Core.DAL
{
    public class DbContextFactory
    {
        private static IConfiguration Configuration { get; set; }

        static DbContextFactory()
        {
            Configuration = Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig();
        }

        /// <summary>
        /// Creates DbContext with the specified connection string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connStrName">name of conn string to retrieve from configuration</param>
        /// <param name="isConnStr">whether ot not it is the actual connection string supplied</param>
        /// <returns></returns>
        public static T CreateDbContext<T>(string connStrName, bool isConnStr = false)
            where T: DbContext
        {
            var ctxType = typeof(T);
            var opts = GetDbContextOptions<T>(connStrName, isConnStr);

            var newCtx = (T)Activator.CreateInstance(ctxType, new object[] { opts });

            return newCtx;
        }


        /// <summary>
        /// Gets DbContextOptions for a specified db context
        /// </summary>
        /// <param name="connStrName"></param>
        /// <param name="isConnStr"></param>
        /// <returns></returns>
        public static DbContextOptions GetDbContextOptions<T>(string connStrName, bool isConnStr = false)
            where T: DbContext
        {
            var optionsBuilder = new DbContextOptionsBuilder<T>();

            //use a specified connection string or grab it from the cfgs
            var connStr = isConnStr ? connStrName : Configuration.GetConnectionString(connStrName);

            //TODO - should DI this at some point really...
            optionsBuilder.UseNpgsql(connStr);

            return optionsBuilder.Options;
        }
    }
}
