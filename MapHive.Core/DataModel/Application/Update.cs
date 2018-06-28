using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel
{
    public partial class Application
    {
        protected internal override async Task<T> UpdateAsync<T>(DbContext dbCtx, Guid uuid)
        {
            var app = await base.UpdateAsync<T>(dbCtx, uuid);

            await HandleFlagsAsync(dbCtx);

            return app;
        }
    }
}
