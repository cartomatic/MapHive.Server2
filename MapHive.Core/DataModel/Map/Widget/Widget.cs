using System.Collections.Generic;

namespace MapHive.Core.DataModel.Map
{
    public class Widget
    {
        /// <summary>
        /// Name of a widget to be displayed
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Prefix to append in front of a calculated value
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Suffix to append to the end of a calculated value
        /// </summary>
        public string Suffix { get; set; }

        /// <summary>
        /// A column to aggregate the data on; when null, all data is aggregated
        /// </summary>
        public string AggregationColumn { get; set; }

        /// <summary>
        /// Whether or not aggregation info should be shown in a widget
        /// </summary>
        public bool? ShowAggregationInfo { get; set; }

        /// <summary>
        /// Operator to be used when aggregating data for this widget
        /// </summary>
        public AggregationOperator AggregationOperator { get; set; }

        /// <summary>
        /// Whether or not the aggregation of per object data should happen prior to calculating a final value
        /// </summary>
        public bool? AggregateFirst { get; set; }

        /// <summary>
        /// Equations that make up this widget
        /// </summary>
        public List<WidgetEquationGroup> Equations { get; set; }

    }
}
