﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DAL;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel
{
    public partial class Lang
    {
        /// <summary>
        /// lang code of the default lang
        /// </summary>
        private static string DefaultLangCode { get; set; } = "en";

        /// <summary>
        /// Default lang
        /// </summary>
        private static Lang DefaultLang { get; set; }

        ///// <summary>
        ///// Gets a default lang as configured in the db
        ///// </summary>
        ///// <typeparam name="TDbCtx"></typeparam>
        ///// <param name="dbCtx"></param>
        ///// <returns></returns>
        //public static async Task<Lang> GetDefaultLangAsync<TDbCtx>(TDbCtx dbCtx)
        //    where TDbCtx : DbContext
        //{
        //    return await GetDefaultLangAsync(dbCtx as DbContext);
        //}

        /// <summary>
        /// Gets a default lang
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        public static async Task<Lang> GetDefaultLangAsync(DbContext dbCtx)
        {
            var langDbCtx = dbCtx as ILocalizedDbContext;
            return DefaultLang ?? (DefaultLang = await langDbCtx?.Langs.FirstOrDefaultAsync(l => l.IsDefault)) ?? (DefaultLang = await langDbCtx?.Langs.FirstOrDefaultAsync(l => l.LangCode == DefaultLangCode));
        }

        /// <summary>
        /// if lang is marked as default, makes sure to check if there is another lang marked as default and if so remove the flag
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        private async Task ResetCurrentDefaultLangAsync(DbContext dbCtx)
        {
            if (IsDefault)
            {
                var currentDefault = await GetDefaultLangAsync(dbCtx);
                if (currentDefault != null && currentDefault.Uuid != Uuid)
                {
                    currentDefault.IsDefault = false;
                    await currentDefault.UpdateAsync(dbCtx, currentDefault.Uuid);

                    //finally update the current lang cache
                    DefaultLang = this;
                }
            }
        }
    }
}
