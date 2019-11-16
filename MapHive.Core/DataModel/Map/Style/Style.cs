using System.Collections.Generic;

namespace MapHive.Core.DataModel.Map
{
    /// <summary>
    /// describes a single styling rule for a data set
    /// </summary>
    public class Style
    {
        /// <summary>
        /// rule name. also an entry in a legend
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Whether or not styling rule applies to all objects or an object filter should be used
        /// </summary>
        public bool? AllObjects { get; set; }


        /// <summary>
        /// Filters used for dataset filtering
        /// </summary>
        public List<StyleFilterGroup> Filters { get; set; }

        /// <summary>
        /// Whether or not polygon style should be applied
        /// </summary>
        public bool? Poly { get; set; }

        /// <summary>
        /// Hex8 representation of rgba fill color
        /// </summary>
        public string PolyFillColor { get; set; }

        /// <summary>
        /// hex8 representation of stroke rgba color
        /// </summary>
        public string PolyStrokeColor { get; set; }

        /// <summary>
        /// Width of a stroke
        /// </summary>
        public double? PolyStrokeWidth { get; set; }

        /// <summary>
        /// Whether or not polyline style should be applied
        /// </summary>
        public bool? Line { get; set; }

        /// <summary>
        /// hex8 representation of stroke rgba color
        /// </summary>
        public string LineStrokeColor { get; set; }

        /// <summary>
        /// Width of a stroke
        /// </summary>
        public double? LineStrokeWidth { get; set; }

        /// <summary>
        /// Whether or not point style should be applied
        /// </summary>
        public bool? Point { get; set; }

        /// <summary>
        /// Size of point symbol
        /// </summary>
        public double? PointSize { get; set; }

        /// <summary>
        /// Shape of a point
        /// </summary>
        public string PointShape { get; set; }

        /// <summary>
        /// Hex representation of rg fill color
        /// </summary>
        public string PointFillColor { get; set; }

        /// <summary>
        /// hex representation of stroke rgb color
        /// </summary>
        public string PointStrokeColor { get; set; }

        /// <summary>
        /// Width of a stroke
        /// </summary>
        public double? PointStrokeWidth { get; set; }

    }
}
