using System;
using System.Reflection;

namespace MapHive.Core.DataModel.Map
{
    /// <summary>
    /// a unified db data source that could have been imported into db from shp, geoJson, csv, service, etc
    /// </summary>
    public partial class DataStore : MapHive.Core.DataModel.Base
    {
        static DataStore()
        {
            BaseObjectTypeIdentifierExtensions.RegisterTypeIdentifier(MethodInfo.GetCurrentMethod().DeclaringType, Guid.Parse("0cabfeed-67dc-439c-99e0-ace3a89b6312"));
        }
    }
}
