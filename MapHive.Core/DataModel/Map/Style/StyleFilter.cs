namespace MapHive.Core.DataModel.Map
{
    public class StyleFilter
    {
        /// <summary>
        /// Name of a column to apply a filter to
        /// </summary>
        public string Column { get; set; }

        /// <summary>
        /// Operator to apply to a column value against a filter value 
        /// </summary>
        public ComparisonOperator Operator { get; set; }

        /// <summary>
        /// Filter value to be used
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Logical join operator between filters
        /// </summary>
        public LogicalJoinOperator Join { get; set; }
    }
}
