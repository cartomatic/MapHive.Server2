using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DAL;
using Npgsql;

#if NETFULL
using System. Data.Entity;
#endif

#if NETSTANDARD
using Microsoft.EntityFrameworkCore;
#endif

namespace MapHive.Core.DataModel
{
    public static partial class OrganizationCrudExtensions
    {
        public static async Task<T> DestroyAsync<T>(this T org, DbContext dbCtx, bool clean)
            where T : Organization
        {
            return await org.DestroyAsync<T>(dbCtx, org.Uuid, clean);
        }
    }

    public partial class Organization
    {
        protected internal override async Task<T> DestroyAsync<T>(DbContext dbCtx, Guid uuid)
        {
            return await DestroyAsync<T>(dbCtx, uuid, true);
        }

        /// <summary>
        /// Destroys an organisation and by default drops its db
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCtx"></param>
        /// <param name="uuid"></param>
        /// <param name="clean"></param>
        /// <returns></returns>
        protected internal async Task<T> DestroyAsync<T>(DbContext dbCtx, Guid uuid, bool clean)
            where T : Base
        {
            //dbCtx
            var mhDb = (MapHiveDbContext)dbCtx;

            //first get rid of the org db!
            var org = await mhDb.Organizations.FirstOrDefaultAsync(o => o.Uuid == uuid);

            if (clean)
            {
                org.LoadDatabases((MapHiveDbContext) dbCtx);

                foreach (var db in org.Databases)
                {
                    //OK, for the time being we work with PgSQL only...
                    //It will blow up though if we decide to support other datasources too....
#if NETFULL
                    var conn = dbCtx.Database.Connection as NpgsqlConnection;
#endif

#if NETSTANDARD
                    var conn = dbCtx.Database.GetDbConnection() as NpgsqlConnection;
#endif
                    if (conn == null)
                        throw new InvalidOperationException("Uhuh, looks like not a PgSQL conn is used by the Db context...");

                    //Only destroy dbs that are created on this server...
                    //as we may not be able to access the other one...
                    //also in many cases, per api dbs will only be defined at the api level
                    if (db.ServerHost == conn.Host && db.ServerPort == conn.Port)
                        db.DropDb();
                }
            }

            //cleanup org databases metadata too
            var orgToRemove = await mhDb.OrganizationDatabases.FirstOrDefaultAsync(odb => odb.OrganizationId == uuid);
            if (orgToRemove != null)
            {
                mhDb.OrganizationDatabases.Remove(orgToRemove);
                await mhDb.SaveChangesAsync();
            }

            //as well as the roles
            var orgRoles = await this.GetChildrenAsync<Organization, Role>(mhDb,false); //false to attach objects. otherwise will not be able to delete them!
            if (orgRoles.Any())
            {
                mhDb.Roles.RemoveRange(orgRoles);
                await mhDb.SaveChangesAsync();
            }


            //and all the links
            var orgLinks = await mhDb.Links.Where(l => l.ParentUuid == uuid || l.ChildUuid == uuid).ToListAsync();
            if (orgLinks.Count > 0)
            {
                mhDb.Links.RemoveRange(orgLinks);
            }

            
            //Note: roles are created per org, so need to be removed.
            //Other objects such as modules, packages etc, are system specific and should not be removed
            //base object delete will take care of their links, and perform an automated cleanup
            
            return await base.DestroyAsync<T>(dbCtx, uuid);
        }
    }
}
