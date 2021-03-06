﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Filtering;
using Cartomatic.Utils.Sorting;
using MapHive.Core.DataModel;

using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel
{
    public static partial class BaseCrudExtensions
    {
        //Note:
        //Crud APIs exposed as extension methods, so the type of the object is actually worked out without having to specify it explicitly;
        //the actual Crud methods are protected


        /// <summary>
        /// Reads a list of objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="dbCtx"></param>
        /// <param name="sorters"></param>
        /// <param name="filters"></param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <param name="detached"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<T>> ReadAsync<T>(this T obj, DbContext dbCtx, IEnumerable<ReadSorter> sorters,
            IEnumerable<ReadFilter> filters, int start = 0, int limit = 25, bool detached = true) where T : Base
        {
            return await obj.ReadAsync<T>(dbCtx, sorters, filters, start, limit, detached);
        }

        /// <summary>
        /// Returns a count of records for a given filters set
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="dbCtx"></param>
        /// <param name="filters"></param>
        /// <param name="detached"></param>
        /// <returns></returns>
        public static async Task<int> ReadCountAsync<T>(this T obj, DbContext dbCtx, IEnumerable<ReadFilter> filters)
            where T : Base
        {
            return await obj.ReadCountAsync<T>(dbCtx, filters);
        }

        /// <summary>
        /// Reads a single object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="dbCtx"></param>
        /// <param name="uuid"></param>
        /// <param name="detached"></param>
        /// <returns></returns>
        public static async Task<T> ReadAsync<T>(this T obj, DbContext dbCtx, Guid uuid, bool detached = true) where T : Base
        {
            return await obj.ReadAsync<T>(dbCtx, uuid, detached);
        }
    }
}
