using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#if NETFULL
using System.Data.Entity;
#endif
#if NETSTANDARD
using Microsoft.EntityFrameworkCore;
#endif

namespace MapHive.Core.DataModel
{
    public partial class Application
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        protected internal virtual async Task HandleFlags(DbContext dbCtx)
        {
            if (IsDefault)
            {
                var previouslyDefault = await dbCtx.Set<Application>().FirstOrDefaultAsync(a => a.IsDefault && a.Uuid != Uuid);
                if (previouslyDefault != null)
                {
                    previouslyDefault.IsDefault = false;
                    await previouslyDefault.UpdateAsync<Application>(dbCtx);
                }
            }
            if (IsHome)
            {
                var previsoulyHome = await dbCtx.Set<Application>().FirstOrDefaultAsync(a => a.IsHome && a.Uuid != Uuid);
                if (previsoulyHome != null)
                {
                    previsoulyHome.IsHome = false;
                    await previsoulyHome.UpdateAsync<Application>(dbCtx);
                }
            }
        }
    }
}
