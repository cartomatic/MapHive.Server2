using System.Collections.Generic;

namespace MapHive.Core.DataModel.Map
{
    public class StyleFilterGroup
    {
        /// <summary>
        /// Logical join operator between filters
        /// </summary>
        public LogicalJoinOperator Join { get; set; }

        public List<StyleFilter> Filters { get; set; }
    }
}
