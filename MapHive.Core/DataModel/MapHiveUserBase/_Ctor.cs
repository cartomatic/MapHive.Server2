using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

        private const string WrongCrudMethodErrorInfo =
            "User CRUD ops require MembershipReboot userManager. Won't do without! Sorry... ";

    }
}
