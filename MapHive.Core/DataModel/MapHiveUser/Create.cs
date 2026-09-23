using System;
using System.Threading.Tasks;
using Cartomatic.Utils.Email;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel
{
    /// <summary>
    /// creates a maphive user
    /// </summary>
    public partial class MapHiveUser
    {
        protected internal override async Task<T> CreateAsync<T>(DbContext dbCtx, IEmailSender emailSender, IEmailAccount emailAccount = null,
            IEmailTemplate emailTemplate = null)
        {
            EnsureSlug();

            //need guid to properly tie up resources with the parent object
            //so generating it here
            Uuid = Guid.NewGuid();

            await HandleResources(dbCtx, Uuid);

            Fullname = this.GetFullUserName();

            var user = await base.CreateAsync<T>(dbCtx, emailSender, emailAccount, emailTemplate) as MapHiveUser;

            return (T)(Base)user;
        }
    }
}
