﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Api.Core.ApiControllers
{
    interface IDbCtxController
    {
        /// <summary>
        /// returns a default db context for a controller
        /// </summary>
        /// <returns></returns>
        DbContext GetDefaultDbContext();
    }
}
