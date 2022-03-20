using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Npgsql;


namespace MapHive.Core.DataModel
{
    public partial class OrganizationDatabase
    {
        /// <summary>
        /// Creates a default instance of org db object and reads the orgs dbs credentials off the config (if present)
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="dbIdentifier"></param>
        /// <returns></returns>
        public static OrganizationDatabase CreateInstanceWithDefaultCredentials(Guid orgId, string dbIdentifier)
        {
            OrganizationDatabase defaultDb = null;
            try
            {
                var cfg = Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig();

                defaultDb = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["OrganizationsDatabase"]) 
                    ? JsonConvert.DeserializeObject<OrganizationDatabase>(ConfigurationManager.AppSettings["OrganizationsDatabase"]) 
                    : cfg?.GetSection("OrganizationsDatabase")?.Get<OrganizationDatabase>();

                if (defaultDb != null)
                {
                    defaultDb.OrganizationId = orgId;
                    defaultDb.DbName = $"mhorg_{orgId:N}"; //uuid without dashes
                    defaultDb.Identifier = dbIdentifier;
                }
            }
            catch
            {
                //ignore
            }

            return defaultDb;
        }
    }
}
