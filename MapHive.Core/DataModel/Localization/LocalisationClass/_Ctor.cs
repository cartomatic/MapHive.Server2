using System;
using System.Reflection;

namespace MapHive.Core.DataModel
{
    public partial class LocalizationClass : Base
    {
        static LocalizationClass()
        {
            BaseObjectTypeIdentifierExtensions.RegisterTypeIdentifier(MethodInfo.GetCurrentMethod().DeclaringType, Guid.Parse("03ad4b67-7801-4cf1-90dd-fe65674fc1e6"));
        }
    }
}
