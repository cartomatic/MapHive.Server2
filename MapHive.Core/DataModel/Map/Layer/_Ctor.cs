using System;
using System.Reflection;
using MapHive.Core.DataModel;

namespace MapHive.Core.DataModel.Map
{
    /// <summary>
    /// Client
    /// </summary>
    public partial class Layer : LayerBase
    {
        static Layer()
        {
            BaseObjectTypeIdentifierExtensions.RegisterTypeIdentifier(MethodInfo.GetCurrentMethod().DeclaringType, Guid.Parse("45869968-574a-4eac-9353-3a13766108be"));
        }

        public Layer()
        {
        }
    }
}
