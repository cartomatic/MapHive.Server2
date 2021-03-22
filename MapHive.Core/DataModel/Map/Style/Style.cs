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
        /// Whether or not should label polys
        /// </summary>
        public bool? PolyLabels { get; set; }

        /// <summary>
        /// field used to label polys
        /// </summary>
        public string PolyLabelField { get; set; }

        /// <summary>
        /// When to start showing poly labels when zooming in and stop showing poly labels when zooming out
        /// </summary>
        public int? PolyLabelVisibilityScaleMin { get; set; }

        /// <summary>
        /// when to stop showing poly labels when zooming in and start showing poly labels when zooming out
        /// </summary>
        public int? PolyLabelVisibilityScaleMax { get; set; }

        /// <summary>
        /// Whether or not poly labels should be decluttered
        /// </summary>
        public bool? PolyLabelDeclutter { get; set; }

        /// <summary>
        /// Poly label wrapping: hide, normal, shorten, wrap
        /// </summary>
        public string PolyLabelWrap { get; set; }

        /// <summary>
        /// Poly label align: center, end, left, right, start
        /// </summary>
        public string PolyLabelAlign { get; set; }

        /// <summary>
        /// Poly label baseline: alphabetic, bottom, hanging, ideographic, middle, top
        /// </summary>
        public string PolyLabelBaseLine { get; set; }

        /// <summary>
        /// Font used to label a poly
        /// </summary>
        public string PolyLabelFont { get; set; }

        /// <summary>
        /// Rotation of a poly label
        /// </summary>
        public int? PolyLabelRotation { get; set; }

        /// <summary>
        /// Poly label style: Normal, Bold, Italic, Bold Italic
        /// </summary>
        public string PolyLabelFontStyle { get; set; }

        /// <summary>
        /// Placement of a poly label: point, line
        /// </summary>
        public string PolyLabelPlacement { get; set; }

        /// <summary>
        /// Max angle a label can be rotated when being rendered along an object
        /// </summary>
        public int? PolyLabelMaxAngle { get; set; }

        /// <summary>
        /// Whether or not a poly label can overflow
        /// </summary>
        public bool PolyLabelOverflow { get; set; }

        /// <summary>
        /// Size of a poly label
        /// </summary>
        public int? PolyLabelSize { get; set; }

        /// <summary>
        /// Line height for poly labels
        /// </summary>
        public decimal? PolyLabelLineHeight { get; set; }

        /// <summary>
        /// Poly label x offset
        /// </summary>
        public int? PolyLabelOffsetX { get; set; }

        /// <summary>
        /// Poly label y offset
        /// </summary>
        public int? PolyLabelOffsetY { get; set; }

        /// <summary>
        /// Poly label color
        /// </summary>
        public string PolyLabelColor { get; set; }

        /// <summary>
        /// Poly label outline color
        /// </summary>
        public string PolyLabelOutlineColor { get; set; }

        /// <summary>
        /// Poly label outline width
        /// </summary>
        public int? PolyLabelOutlineWidth { get; set; }




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
        /// Whether or not should label lines
        /// </summary>
        public bool? LineLabels { get; set; }

        /// <summary>
        /// field used to label lines
        /// </summary>
        public string LineLabelField { get; set; }

        /// <summary>
        /// When to start showing line labels when zooming in and stop showing line labels when zooming out
        /// </summary>
        public int? LineLabelVisibilityScaleMin { get; set; }

        /// <summary>
        /// when to stop showing line labels when zooming in and start showing line labels when zooming out
        /// </summary>
        public int? LineLabelVisibilityScaleMax { get; set; }

        /// <summary>
        /// Whether or not line labels should be decluttered
        /// </summary>
        public bool? LineLabelDeclutter { get; set; }

        /// <summary>
        /// Line label wrapping: hide, normal, shorten, wrap
        /// </summary>
        public string LineLabelWrap { get; set; }

        /// <summary>
        /// Line label align: center, end, left, right, start
        /// </summary>
        public string LineLabelAlign { get; set; }

        /// <summary>
        /// Line label baseline: alphabetic, bottom, hanging, ideographic, middle, top
        /// </summary>
        public string LineLabelBaseLine { get; set; }

        /// <summary>
        /// Font used to label a line
        /// </summary>
        public string LineLabelFont { get; set; }

        /// <summary>
        /// Rotation of a line label
        /// </summary>
        public int? LineLabelRotation { get; set; }

        /// <summary>
        /// Line label style: Normal, Bold, Italic, Bold Italic
        /// </summary>
        public string LineLabelFontStyle { get; set; }

        /// <summary>
        /// Placement of a line label: point, line
        /// </summary>
        public string LineLabelPlacement { get; set; }

        /// <summary>
        /// Max angle a line label can be rotated when being rendered along an object
        /// </summary>
        public int? LineLabelMaxAngle { get; set; }

        /// <summary>
        /// Whether or not a line label can overflow
        /// </summary>
        public bool LineLabelOverflow { get; set; }

        /// <summary>
        /// Size of a line label
        /// </summary>
        public int? LineLabelSize { get; set; }

        /// <summary>
        /// Line height for line labels
        /// </summary>
        public decimal? LineLabelLineHeight { get; set; }

        /// <summary>
        /// Line label x offset
        /// </summary>
        public int? LineLabelOffsetX { get; set; }

        /// <summary>
        /// Line label y offset
        /// </summary>
        public int? LineLabelOffsetY { get; set; }

        /// <summary>
        /// line label color
        /// </summary>
        public string LineLabelColor { get; set; }

        /// <summary>
        /// Line label outline color
        /// </summary>
        public string LineLabelOutlineColor { get; set; }

        /// <summary>
        /// Line label outline width
        /// </summary>
        public int? LineLabelOutlineWidth { get; set; }



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
        /// Whether or not labels should be decluttered
        /// </summary>
        public bool? PointLabelDeclutter { get; set; }

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

    }
}
