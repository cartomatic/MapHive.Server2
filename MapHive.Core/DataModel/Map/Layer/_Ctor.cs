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
            BaseObjectTypeIdentifierExtensions.RegisterTypeIdentifier(MethodInfo.GetCurrentMethod().DeclaringType, Guid.Parse("e40ad645-cdf7-44ed-8691-40f9b6cd4504"));
        }

        public Layer()
        {
        }

        /// <summary>
        /// need to cover the base property for proper deserialization
        /// </summary>
        //public new Layer SourceLayer { get; set; }
    }
}
