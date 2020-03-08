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

        /// <summary>
        /// Whether or not should label points
        /// </summary>
        public bool? PointLabels { get; set; }

        /// <summary>
        /// field used to label points
        /// </summary>
        public string PointLabelField { get; set; }

        /// <summary>
        /// When to start showing point labels when zooming in and stop showing point labels when zooming out
        /// </summary>
        public int? PointLabelVisibilityScaleMin { get; set; }

        /// <summary>
        /// when to stop showing point labels when zooming in and start showing point labels when zooming out
        /// </summary>
        public int? PointLabelVisibilityScaleMax { get; set; }

        /// <summary>
        /// point label wrapping: hide, normal, shorten, wrap
        /// </summary>
        public string PointLabelWrap { get; set; }

        /// <summary>
        /// point label align: center, end, left, right, start
        /// </summary>
        public string PointLabelAlign { get; set; }

        /// <summary>
        /// point label baseline: alphabetic, bottom, hanging, ideographic, middle, top
        /// </summary>
        public string PointLabelBaseLine { get; set; }

        /// <summary>
        /// Font used to label a point
        /// </summary>
        public string PointLabelFont { get; set; }

        /// <summary>
        /// Rotation of a point label
        /// </summary>
        public int? PointLabelRotation { get; set; }

        /// <summary>
        /// Point label style: Normal, Bold, Italic, Bold Italic
        /// </summary>
        public string PointLabelFontStyle { get; set; }

        /// <summary>
        /// Placement of a point label: point, line
        /// </summary>
        public string PointLabelPlacement { get; set; }

        /// <summary>
        /// Max angle a label can be rotated when being rendered along an object
        /// </summary>
        public int? PointLabelMaxAngle { get; set; }

        /// <summary>
        /// Whether or not a point label can overflow
        /// </summary>
        public bool PointLabelOverflow { get; set; }

        /// <summary>
        /// Size of a point label
        /// </summary>
        public int? PointLabelSize { get; set; }

        /// <summary>
        /// Line height for point labels
        /// </summary>
        public decimal? PointLabelLineHeight { get; set; }

        /// <summary>
        /// Point label x offset
        /// </summary>
        public int? PointLabelOffsetX { get; set; }

        /// <summary>
        /// Point label y offset
        /// </summary>
        public int? PointLabelOffsetY { get; set; }

        /// <summary>
        /// point label color
        /// </summary>
        public string PointLabelColor { get; set; }

        /// <summary>
        /// point label outline color
        /// </summary>
        public string PointLabelOutlineColor { get; set; }

        /// <summary>
        /// point label outline width
        /// </summary>
        public int? PointLabelOutlineWidth { get; set; }

        ///// <summary>
        ///// Whether or not labels should be declutterred
        ///// </summary>
        //public bool? PointLabelDeclutter { get; set; }

    }
}
