using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Npgsql;


namespace MapHive.Core.DataModel
{
    public partial class OrganizationDatabase
    {
        /// <summary>
        /// Creates a default isntance of org db object and reads the orgs dbs credentials off the config (if present)
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

                defaultDb =
                    JsonConvert.DeserializeObject<OrganizationDatabase>(
                        ConfigurationManager.AppSettings["OrganizationsDatabase"]
                        ?? cfg?["OrganizationsDatabase"]
                    );

                defaultDb.OrganizationId = orgId;
                defaultDb.DbName = $"org_{orgId.ToString().Replace("-", "")}";
                defaultDb.Identifier = dbIdentifier;
            }
            catch
            {
                //ignore
            }

            return defaultDb;
        }
    }
}
