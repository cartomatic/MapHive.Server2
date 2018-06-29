using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel
{
    /// <summary>
    /// creates a maphive user
    /// </summary>
    public partial class MapHiveUser
    {
        protected internal override async Task<T> CreateAsync<T>(DbContext dbCtx, IEmailAccount emailAccount = null,
            IEmailTemplate emailTemplate = null)
        {
            EnsureSlug();

            var user = await base.CreateAsync<T>(dbCtx, emailAccount, emailTemplate) as MapHiveUser;

            return (T)(Base)user;
        }
    }
}
