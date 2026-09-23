using System;
using System.Reflection;

namespace MapHive.Core.DataModel
{
    public partial class Role : Base
    {
        static Role()
        {
            BaseObjectTypeIdentifierExtensions.RegisterTypeIdentifier(MethodInfo.GetCurrentMethod().DeclaringType, Guid.Parse("20cb08ce-1edb-4461-8140-1046d965496f"));
        }
    }
}
