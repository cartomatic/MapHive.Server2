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
        /// Organization id for database
        /// </summary>
        public Guid OrganizationId { get; set; }

        /// <summary>
        /// Identifier of a db record; Identifiers are in most cases equal to app short names, so each api can have not only its very own db server, but also can load balance
        /// per org with no restrictions
        /// </summary>
        public string Identifier { get; set; }

        /// <inheritdoc />
        public DataSourceProvider DataSourceProvider { get; set; }

        /// <inheritdoc />
        public string ServerHost { get; set; }

        /// <inheritdoc />
        public string ServerName { get; set; }

        /// <inheritdoc />
        public int? ServerPort { get; set; }

        /// <inheritdoc />
        public string DbName { get; set; }

        /// <inheritdoc />
        public string ServiceDb { get; set; }

        /// <inheritdoc />
        public string UserName { get; set; }

        /// <inheritdoc />
        public string Pass { get; set; }

        /// <inheritdoc />
        public string ServiceUserName { get; set; }

        /// <inheritdoc />
        public string ServiceUserPass { get; set; }

    }
}
