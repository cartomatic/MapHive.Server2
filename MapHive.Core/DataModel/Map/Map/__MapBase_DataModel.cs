using System;
using System.Collections.Generic;
using System.Reflection;
using MapHive.Core.DataModel;

namespace MapHive.Core.DataModel.Map
{
    /// <summary>
    /// Map
    /// </summary>
    public partial class MapBase
    {
        /// <summary>
        /// Permalink for accessing a map without authentication
        /// </summary>
        public Guid? Permalink { get; set; }

        /// <summary>
        /// Project name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Project description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Lon to center a project on init
        /// </summary>
        public double? InitLo { get; set; }

        /// <summary>
        /// Lat to center a project on init
        /// </summary>
        public double? InitLa { get; set; }

        /// <summary>
        /// Zoom to zoom a project on init
        /// </summary>
        public int? InitZoom { get; set; }

        /// <summary>
        /// Index of a base layer to be turned on when 
        /// </summary>
        public int? BaseLayer { get; set; }

        /// <summary>
        /// Collection of layers assigned to this project
        /// </summary>
        public List<LayerBase> Layers { get; set; }

    }
}
