using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DAL;
using Microsoft.EntityFrameworkCore;


namespace MapHive.Core.DataModel
{
    public static partial class OrganizationCrudExtensions
    {
        /// <summary>
        /// Creates and organisation account, register a user as an owner and registers the specified apps to be linked to it too
        /// </summary>
        /// <param name="org"></param>
        /// <param name="dbCtx"></param>
        /// <param name="owner"></param>
        /// <param name="appsToLink"></param>
        /// <returns></returns>
        public static async Task<Organization> CreateAsync(this Organization org, DbContext dbCtx, MapHiveUser owner, IEnumerable<string> appsToLink)
        {
            //finaly grab the apps that should be registered for the org and link them
            var mhDb = (MapHiveDbContext)dbCtx;

            var apps = await mhDb.Applications.Where(app => appsToLink.Contains(app.ShortName))
                .OrderBy(app => app.ShortName)
                .ToListAsync();

            return await CreateAsync(org, dbCtx, owner, apps);
        }

        /// <summary>
        /// Creates and organisation account, register a user as an owner and registers the specified apps to be linked to it too
        /// </summary>
        /// <param name="org"></param>
        /// <param name="dbCtx"></param>
        /// <param name="owner"></param>
        /// <param name="appsToLink"></param>
        /// <returns></returns>
        public static async Task<Organization> CreateAsync(this Organization org, DbContext dbCtx, MapHiveUser owner, IEnumerable<Application> appsToLink)
        {
            //first create the org
            await org.CreateAsync(dbCtx);

            //take care of assigning the owner role to a user
            await org.AddOwnerAsync(dbCtx, owner);

            //finaly grab the apps that should be registered for the org and link them
            var mhDb = (MapHiveDbContext)dbCtx;

            foreach (var app in appsToLink)
            {
                org.AddLink(app);
            }

            await org.UpdateAsync(dbCtx, org.Uuid);

            return org;
        }
    }


    public partial class Organization
    {
        protected internal override async Task<T> CreateAsync<T>(DbContext dbCtx)
        {
            //make sure uuid is present as it is a fallback option for slug!
            if (Uuid == default(Guid))
                Uuid = Guid.NewGuid();

            EnsureSlug();

            var org = await base.CreateAsync<T>(dbCtx) as Organization;

            //once an org is present need to create the default roles and link them with an org
            var rOwner = await CreateRoleAsync(dbCtx, OrgRoleIdentifierOwner, OrgRoleNameOwner);
            var rAdmin = await CreateRoleAsync(dbCtx, OrgRoleIdentifierAdmin, OrgRoleNameAdmin);
            var rMember = await CreateRoleAsync(dbCtx, OrgRoleIdentifierMember, OrgRoleNameMember);

            //link roles to org
            org.AddLink(rOwner);
            org.AddLink(rAdmin);
            org.AddLink(rMember);

            //and update the org
            await org.UpdateAsync(dbCtx);

            //NOTE:
            //org db creation now moved to a separate procedure. This way it is created implicitly when needed
            //this way each api can create org dbs totally independently from maphive meta db, server, db server, etc.

            ////once org is created, create its database
            //var orgDb = OrganizationDatabase.CreateInstanceWithDefaultCredentials();
            //orgDb.OrganizationId = org.Uuid;
            //orgDb.DbName = $"org_{org.Uuid.ToString("N")}";

            //await orgDb.Create(dbCtx);


            ////create the physical db
            //orgDb.CreateOrUpdateDatabase();

            return (T)(Base)org;
        }
    }
}
