using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;

using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DAL;
using Newtonsoft.Json;
using Npgsql;

using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DataModel
{
    public partial class OrganizationDatabase
    {
        /// <summary>
        /// Renames a database; will throw if a name is not valid
        /// </summary>
        /// <param name="mhDbCtx"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public async Task Rename(MapHiveDbContext mhDbCtx, string newName)
        {
            //first cut the connection to the db if any
            DisconnectDatabase();

            //rename the db
            using (var conn = new NpgsqlConnection(GetConnectionString(true)))
            {
                conn.Open();

                var cmd = new NpgsqlCommand
                {
                    Connection = conn,
                    CommandText = $"ALTER DATABASE {DbName} RENAME TO {newName}"
                };

                cmd.ExecuteNonQuery();

                conn.Close();
                conn.Dispose();
            }

            //and update self
            DbName = newName;

            await UpdateAsync<OrganizationDatabase>(mhDbCtx, Uuid);
        }
    }
}
