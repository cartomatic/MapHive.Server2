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
        public void DropDb()
        {
            //first cut the connection to the db if any
            DisconnectDatabase();

            //it should be possible to drop the db now
            using (var conn = new NpgsqlConnection(GetConnectionString(true)))
            {
                conn.Open();

                var cmd = new NpgsqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = $"DROP DATABASE IF EXISTS {DbName}"; //pgsql

                cmd.ExecuteNonQuery();

                conn.Close();
                conn.Dispose();
            }
        }
    }
}
