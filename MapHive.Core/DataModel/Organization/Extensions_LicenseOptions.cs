using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DAL;

namespace MapHive.Core.DataModel
{

    public static partial class OrganizationCrudExtensions
    {
        /// <summary>
        /// Reads organization's licence options
        /// </summary>
        /// <param name="org"></param>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        public static async Task ReadLicenseOptionsAsync(this Organization org, MapHiveDbContext dbCtx)
        {
            //FIXME - see TODO below; this will need to be optimised!

            //apps
            org.LicenseOptions.Apply((await org.GetOrganizationAssetsAsync<Application>(dbCtx)).Item1);

        }

        /// <summary>
        /// Reads licence options for a range of orgs
        /// </summary>
        /// <param name="orgs"></param>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        public static async Task ReadLicenseOptionsAsync(this IEnumerable<Organization> orgs, MapHiveDbContext dbCtx)
        {
            //TODO - make it possible to read org assets in bulk in two scenarios
            //TODO - * for a single org
            //TODO - * for a range of orgs
            //TODO - this is so the license opts read is quick and optimised instead of reading all the stuff one by one. it should be enough to read all the packages, modules, apps and datasources at in single reads and then just pass the data for further processing

            //for the above also see the Assets.cs - this is where the optimisation will take place
            //for the time being just extracting all the stuff one by one...

            foreach (var org in orgs)
            {
                await org.ReadLicenseOptionsAsync(dbCtx);
            }
        }
    }
}
