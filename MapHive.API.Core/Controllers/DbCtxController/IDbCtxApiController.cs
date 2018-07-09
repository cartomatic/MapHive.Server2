using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace MapHive.API.Core.Controllers
{
    interface IDbCtxApiController
    {
        /// <summary>
        /// returns a default db context for a controller
        /// </summary>
        /// <returns></returns>
        DbContext GetDefaultDbContext();
    }
}
