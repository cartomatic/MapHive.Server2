using System;
using System.Collections.Generic;
using System.Text;
using Cartomatic.Utils.Data;

namespace MapHive.Core.DataModel.Map
{
    /// <summary>
    /// Describes a data source for a layer
    /// </summary>
    public class DataSource
    {
        /// <summary>
        /// data source credentials
        /// </summary>
        public DataSourceCredentials DataSourceCredentials { get; set; }

        /// <summary>
        /// Schema name
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// Name of a table data is stored in
        /// </summary>
        public string Table { get; set; }

        /// <summary>
        /// column set
        /// </summary>
        public List<Column> Columns { get; set; }
    }
}
