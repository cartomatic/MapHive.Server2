using System;
using System.Reflection;

namespace MapHive.Core.DataModel
{
    public partial class TranslationKey : TranslationKeyBase
    {
        static TranslationKey()
        {
            BaseObjectTypeIdentifierExtensions.RegisterTypeIdentifier(MethodInfo.GetCurrentMethod().DeclaringType, Guid.Parse("987ce604-4125-44e6-bd6d-8db0857756a4"));
        }
    }
}
