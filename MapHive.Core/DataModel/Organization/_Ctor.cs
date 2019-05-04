using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.JsonSerializableObjects;

namespace MapHive.Core.DataModel
{
    public partial class Organization : Base
    {
        static Organization()
        {
            BaseObjectTypeIdentifierExtensions.RegisterTypeIdentifier(MethodInfo.GetCurrentMethod().DeclaringType, Guid.Parse("0bc1402a-ec54-4e50-8e04-eb22a7625b91"));
        }

        public Organization()
        {
            BillingExtraInfo = new SerializableDictionaryOfString();
            LicenseOptions = new OrganizationLicenseOptions();
            VisualIdentification = new SerializableDictionaryOfObject();
        }
    }
}
