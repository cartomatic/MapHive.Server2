﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel
{
    public partial class Organization
    {
        /// <summary>
        /// Creates org db 
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <param name="identifier">DB identifier - key that allows specifying different dbs for one organization. identifier has a unique constraint per org uuid</param>
        /// <returns></returns>
        public async Task CreateDataBaseAsync(DbContext dbCtx, string identifier)
        {
            //once org is created, create its database
            var orgDb = GetDatabase(identifier);
            await orgDb.CreateAsync(dbCtx);
            
            //create the physical db
            orgDb.CreateOrUpdateDatabase();
        }

        /// <summary>
        /// Gets a db with a specified db identifier
        /// <para />
        /// If live data is required make sure to call LoadDatabases prior to calling this method
        /// </summary>
        /// <param name="dbIdentifier"></param>
        /// <returns></returns>
        public OrganizationDatabase GetDatabase(string dbIdentifier)
        {
            //Note:
            //org dbs data should have been decrypted by the UserConfigurationFilter
            var orgDb = Databases
                ?.FirstOrDefault(db => db.Identifier == dbIdentifier);

            //If org db is not defined, it means it has not been overwritten at the core meta level, so create it off the web.config
            //as the default api db srv should be used in this scenario
            return orgDb ?? OrganizationDatabase.CreateInstanceWithDefaultCredentials(Uuid, dbIdentifier);
        }
    }
}
