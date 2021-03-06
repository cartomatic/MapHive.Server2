﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel
{
    public partial class Lang
    {
        /// <summary>
        /// Creates an object; returns a created object or null if it was not possible to create it due to the fact a uuid is already reserved
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        protected internal override async  Task<T> CreateAsync<T>(DbContext dbCtx)
        {
            await ResetCurrentDefaultLangAsync(dbCtx);
            return await base.CreateAsync<T>(dbCtx);
        }
        
    }
}
