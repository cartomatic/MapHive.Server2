using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DAL;
using Microsoft.EntityFrameworkCore;


namespace MapHive.Core.DataModel
{
    public partial class Organization
    {
        /// <summary>
        /// Updates an org - ensures its slug is always present & valid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCtx"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        protected internal override async Task<T> UpdateAsync<T>(DbContext dbCtx, Guid uuid)
        {
            Uuid = uuid;

            //make sure uuid is present as it is a fallback option for slug!
            EnsureSlug();

            return await base.UpdateAsync<T>(dbCtx, uuid);
        }
    }
}
