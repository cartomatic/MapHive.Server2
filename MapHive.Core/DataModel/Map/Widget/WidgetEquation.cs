namespace MapHive.Core.DataModel.Map
{
    public class WidgetEquation
    {
        /// <summary>
        /// Column to take a value from
        /// </summary>
        public string Column { get; set; }

        /// <summary>
        /// Custom numeric value to be used in place of column data
        /// </summary>
        public decimal? CustomValue { get; set; }

        /// <summary>
        /// Operator when another column is a part of operation
        /// </summary>
        public ArithmeticOperator? ArithmeticOperator { get; set; }

        /// <summary>
        /// An aggregation operator to be used when widget is set up to perform the values aggregations prior to executing final value calculation
        /// </summary>
        public AggregationOperator? AggregationOperator { get; set; }
    }
}
