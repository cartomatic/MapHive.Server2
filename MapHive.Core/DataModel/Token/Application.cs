using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.JsonSerializableObjects;
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
            ApplicationIds ??= new SerializableListOfGuid();
            ApplicationIds.Add(app.Uuid);
        }

        /// <summary>
        /// Gets applications assigned to token
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        public async Task<List<Application>> GetApplicationsAsync(MapHiveDbContext dbCtx)
        {
            ApplicationIds ??= new SerializableListOfGuid();
            return await dbCtx.Applications.Where(app => ApplicationIds.Contains(app.Uuid)).ToListAsync();
        }
    }
}
