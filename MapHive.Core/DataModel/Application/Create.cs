using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel
{
    public partial class Application
    {
        protected internal override async Task<T> CreateAsync<T>(DbContext dbCtx)
        {
            var app = await base.CreateAsync<T>(dbCtx);

            await HandleFlagsAsync(dbCtx);

            return app;
        }
    }
}
