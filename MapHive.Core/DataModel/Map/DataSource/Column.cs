using System;
using System.Collections.Generic;
using System.Text;

namespace MapHive.Core.DataModel.Map
{

    public class Column
    {
        /// <summary>
        /// Column name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Friendly name for user data display, reports, etc.
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// Type of data in a column
        /// </summary>
        public ColumnDataType Type { get; set; }

        /// <summary>
        /// Whether or not column should appear in info output
        /// </summary>
        public bool? Queryable { get; set; }

        /// <summary>
        /// Whether or not column should be available for styling
        /// </summary>
        public bool? Styleable { get; set; }

        /// <summary>
        /// Whether or not a layer should appear in the info panel
        /// </summary>
        public bool? Info { get; set; }
    }
}
