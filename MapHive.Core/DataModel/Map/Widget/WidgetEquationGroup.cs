using System.Collections.Generic;

namespace MapHive.Core.DataModel.Map
{
    public class WidgetEquationGroup
    {
        /// <summary>
        /// Equations that make up the group
        /// </summary>
        public List<WidgetEquation> Equations { get; set; }

        /// <summary>
        /// Operator that joins this group with another one
        /// </summary>
        public ArithmeticOperator? ArithmeticOperator { get; set; }
    }
}
