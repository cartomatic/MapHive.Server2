using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Data;
using Newtonsoft.Json;
using Npgsql;

namespace MapHive.Core.DataModel
{
    public partial class OrganizationDatabase
    {
        /// <summary>
        /// Get database connection string
        /// </summary>
        /// <param name="serviceDatabase">whether or not to connect to a service db</param>
        /// <param name="superUser">whether or not use superuser credentials</param>
        /// <returns></returns>
        public string GetConnectionString(bool serviceDatabase = false, bool superUser = false)
        {
            return new DataSourceCredentials
            {
                DataSourceProvider = DataSourceProvider,
                DbName = DbName,
                Pass = Pass,
                ServerHost = ServerHost,
                ServerName = ServerName,
                ServerPort = ServerPort,
                ServiceDb = ServiceDb,
                ServiceUserName = ServiceUserName,
                ServiceUserPass = ServiceUserPass,
                UseDefaultServiceDb = true, 
                UserName = UserName
            }.GetConnectionString(serviceDatabase, superUser);
        }
    }
}
