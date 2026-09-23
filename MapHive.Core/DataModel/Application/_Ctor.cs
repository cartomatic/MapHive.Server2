using System;
using System.Reflection;
using Cartomatic.Utils.JsonSerializableObjects;

namespace MapHive.Core.DataModel
{
    public partial class Application : Base
    {
        static Application()
        {
            BaseObjectTypeIdentifierExtensions.RegisterTypeIdentifier(MethodInfo.GetCurrentMethod().DeclaringType, Guid.Parse("a980c990-656f-47ca-8969-100853866d7b"));
        }

        public Application()
        {
            LicenseOptions = new LicenseOptions();
            VisualIdentification = new SerializableDictionaryOfObject();
        }
    }
}
