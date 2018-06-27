using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MapHive.Core.DataModel
{
    public partial class EmailTemplateLocalization : Base, ILocalization
    {
        static EmailTemplateLocalization()
        {
            BaseObjectTypeIdentifierExtensions.RegisterTypeIdentifier(MethodInfo.GetCurrentMethod().DeclaringType, Guid.Parse("8226738b-8d7b-42e7-bfd6-30cf4f55d57e"));
        }
    }
}
