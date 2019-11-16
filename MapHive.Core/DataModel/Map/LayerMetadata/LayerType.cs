using System;
using System.Collections.Generic;
using System.Text;

namespace MapHive.Core.DataModel.Map
{
    public enum LayerType
    {
        Custom, //not used anywhere really...
        SHP = 1,
        WMS,
        WFS,
        GeoJSON,
        JSON,
        CSV
    }
}
    