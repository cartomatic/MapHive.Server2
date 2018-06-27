using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Filtering;
using Cartomatic.Utils.Sorting;
using MapHive.Core.DAL;
#if NETFULL
using System.Data.Entity;
#endif
#if NETSTANDARD
using Microsoft.EntityFrameworkCore;
#endif

namespace MapHive.Core.DataModel
{
    public partial class Organization
    {
        protected internal override async Task<T> ReadAsync<T>(DbContext dbCtx, Guid uuid, bool detached = true)
        {
            var org = (Organization)(Base) await base.ReadAsync<T>(dbCtx, uuid, detached);

            await org.ReadLicenseOptionsAsync((MapHiveDbContext) dbCtx);

            return (T)(Base) org;
        }

        protected internal override async Task<IEnumerable<T>> ReadAsync<T>(DbContext dbCtx, IEnumerable<Guid> uuids, bool detached = true)
        {
            var orgs = (IEnumerable<Organization>) await base.ReadAsync<T>(dbCtx, uuids, detached);

            await orgs.ReadLicenseOptionsAsync((MapHiveDbContext)dbCtx);

            return (IEnumerable<T>) orgs;
        }

        /// <inheritdoc />
        protected internal override async Task<IEnumerable<T>> ReadAsync<T>(DbContext dbCtx, IEnumerable<ReadSorter> sorters, IEnumerable<ReadFilter> filters, int start = 0, int limit = 25,
            bool detached = true)
        {
            var orgs = (IEnumerable<Organization>)await base.ReadAsync<T>(dbCtx, sorters, filters, start, limit, detached);

            await orgs.ReadLicenseOptionsAsync((MapHiveDbContext)dbCtx);

            return (IEnumerable<T>)orgs;
        }
    }
}
