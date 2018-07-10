using System;
using System.Collections.Generic;

using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DataModel;

using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DAL
{
    public class Seeder
    {
        /// <summary>
        /// Executes all the configured seed procedures
        /// </summary>
        /// <typeparam name="TDbCtx"></typeparam>
        /// <typeparam name="TSeed"></typeparam>
        /// <param name="dbCtx"></param>
        /// <param name="filter">Fully qualified namespaces to be taken into account when auto seeding object types</param>
        public static void SeedAll<TDbCtx, TSeed>(TDbCtx dbCtx, IEnumerable<string> filter = null)
            where TDbCtx : BaseDbContext
        {
            SeedAll(dbCtx, typeof(TSeed));
        }

        /// <summary>
        /// Executes all the configured seed procedures
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCtx"></param>
        /// <param name="t">type to execute seed for</param>
        /// <param name="filter">Fully qualified namespaces to be taken into account when auto seeding object types</param>
        public static void SeedAll<T>(T dbCtx, Type t, IEnumerable<string> filter = null)
            where T : BaseDbContext
        {
            var mi = t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            foreach (var m in mi)
            {
                if (m.Name != nameof(SeedAll) && m.Name.StartsWith("Seed"))
                {
                    //assume there is one param for the time being
                    //may have to change it later
                    m.Invoke(null, new object[] { dbCtx });
                }
            }

            //finally do the object type seed
            SeedObjectTypes(dbCtx as ApplicationDbContext);
        }

        /// <summary>
        /// Performs object type seed
        /// </summary>
        /// <typeparam name="TDbCtx"></typeparam>
        /// <param name="context"></param>
        /// <param name="filter">Fully qualified namespaces to be taken into account when auto seeding object types</param>
        private static void SeedObjectTypes<TDbCtx>(TDbCtx context, IEnumerable<string> filter = null)
            where TDbCtx : ApplicationDbContext
        {
            //this is an automated seed, so ignore scenarios where context is not an app context
            if (context == null)
                return;

            foreach (var type in ObjectTypeExtensions.GetRegisteredTypes())
            {
                if (VerifyObjectType(type, filter))
                {
                    //context.ObjectTypes.AddOrUpdate(new ObjectType { Name = type.ToString(), Uuid = ObjectTypeExtensions.GetTypeUuid(type) });  

                    if (!context.ObjectTypes.Any(o=>o.Uuid == ObjectTypeExtensions.GetTypeUuid(type)))
                    {
                        context.ObjectTypes.Add(new ObjectType
                        {
                            Name = type.ToString(),
                            Uuid = ObjectTypeExtensions.GetTypeUuid(type)
                        });
                    }
                }
                
            }
        }

        /// <summary>
        /// Verifies if an object type should be seeded
        /// </summary>
        /// <param name="t"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private static bool VerifyObjectType(Type t, IEnumerable<string> filter)
        {
            if (filter == null || !filter.Any())
                return true;

            return filter.Any(f => t.FullName.StartsWith(f) && t.Assembly.FullName.StartsWith(f));
        }

        
    }
}
