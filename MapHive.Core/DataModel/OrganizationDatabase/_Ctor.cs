using System;
using System.Reflection;
using Cartomatic.Utils.Data;
using MapHive.Core.DataModel;

namespace MapHive.Core.DataModel
{
    public partial class OrganizationDatabase : Base, IDataSourceCredentials
    {
        static OrganizationDatabase()
        {
            BaseObjectTypeIdentifierExtensions.RegisterTypeIdentifier(MethodBase.GetCurrentMethod().DeclaringType,
                Guid.Parse("1f388cd3-b739-4dcf-8314-5abf037b4fca"));
        }
    }
}
