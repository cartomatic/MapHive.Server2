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
        /// resets all the connections to the database
        /// </summary>
        protected void DisconnectDatabase()
        {
            using (var conn = new NpgsqlConnection(GetConnectionString(true)))
            {
                conn.Open();

                var cmd = new NpgsqlCommand();
                cmd.Connection = conn;
                cmd.CommandText =
                    //pgsql 9.x
                    $"SELECT pg_terminate_backend(pg_stat_activity.pid) FROM pg_stat_activity WHERE pg_stat_activity.datname = '{DbName}';";

                cmd.ExecuteNonQuery();

                conn.Close();
                conn.Dispose();
            }
        }
    }
}
