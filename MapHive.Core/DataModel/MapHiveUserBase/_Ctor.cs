using System;
using System.Reflection;

namespace MapHive.Core.DataModel
{
    /// <summary>
    /// Basic implementation of MapHiveUser
    /// </summary>
    public abstract partial class MapHiveUserBase : Base, IMapHiveUser
    {
        static MapHiveUserBase()
        {
            BaseObjectTypeIdentifierExtensions.RegisterTypeIdentifier(MethodInfo.GetCurrentMethod().DeclaringType, Guid.Parse("c34273e1-6f57-43fb-8460-44eb7bac0315"));
        }
    }
}
