using System;
using System.Collections.Generic;
using System.Text;

namespace MapHive.Core.DataModel.Map
{
    public class LayerMetadata
    {
        
        /// <summary>
        /// Service url - applies to remote layers such as WMS or WFS
        /// </summary>
        public string ServiceUrl { get; set; }

        /// <summary>
        /// Service version; applies to remote layers such as WMS or WFS
        /// </summary>
        public string ServiceVersion { get; set; }

        /// <summary>
        /// Service capabilities; applies to remote layers such as WMS or WFS
        /// </summary>
        public Dictionary<string, object> ServiceCapabilities { get; set; }

        /// <summary>
        /// Feature type description (output of DescribeFeatureType request); applies to remote layers such as WFS
        /// </summary>
        public Dictionary<string, object> ServiceFeatureTypeDescription { get; set; }

        /// <summary>
        /// Service layers; applies to remote layers such as WMS
        /// </summary>
        public string ServiceLayers { get; set; }

        /// <summary>
        /// extents for given EPSG code. mix, miy, maxx, maxy
        /// </summary>
        public Dictionary<int, double[]> Extent { get; set; }

        /// <summary>
        /// Loading strategy for a layer
        /// </summary>
        public string OlLoadingStrategy { get; set; }

        /// <summary>
        /// User name for layers that require authentication
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Password for layers that require authentication
        /// </summary>
        public string Password { get; set; }
    }
}
