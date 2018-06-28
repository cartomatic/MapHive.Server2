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
        /// Loads dbs configured for an org
        /// </summary>
        /// <param name="organization"></param>
        /// <param name="db"></param>
        public static void LoadDatabases(this Organization organization, MapHiveDbContext db)
        {
            organization.Databases = db.OrganizationDatabases.Where(x => x.OrganizationId == organization.Uuid).ToList();
        }

        /// <summary>
        /// Loads dbs configured for an org
        /// </summary>
        /// <param name="organization"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static async Task LoadDatabasesAsync(this Organization organization, MapHiveDbContext db)
        {
            organization.Databases = await db.OrganizationDatabases.Where(x => x.OrganizationId == organization.Uuid).ToListAsync();
        }

    }
}
