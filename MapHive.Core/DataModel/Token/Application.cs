using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DAL;

using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel
{
    public partial class Token
    {
        /// <summary>
        /// sets applictions that can be accessed via this token
        /// </summary>
        /// <param name="apps"></param>
        public void SetApplication(Application app)
        {
            this.ApplicationId = app.Uuid;
        }

        /// <summary>
        /// Gets application assigned to token
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        public async Task<Application> GetApplication(MapHiveDbContext dbCtx)
        {
            return await dbCtx.Applications.FirstOrDefaultAsync(app => app.Uuid == ApplicationId);
        }
    }
}
