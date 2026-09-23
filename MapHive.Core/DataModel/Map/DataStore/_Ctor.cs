using System;
using System.Reflection;

namespace MapHive.Core.DataModel.Map
{
    /// <summary>
    /// a unified db data source that could have been imported into db from shp, geoJson, csv, service, etc
    /// </summary>
    public partial class DataStore : Base
    {
        static DataStore()
        {
            BaseObjectTypeIdentifierExtensions.RegisterTypeIdentifier(MethodInfo.GetCurrentMethod().DeclaringType, Guid.Parse("d84701ad-913f-4e99-9ed7-749230f132a5"));
        }
    }
}
