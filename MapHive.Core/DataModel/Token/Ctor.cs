using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.JsonSerializableObjects;
using MapHive.Core.DataModel;

namespace MapHive.Core.DataModel
{
    /// <summary>
    /// Token is an equivalent of a user; it can provide a secured but non-user api access;
    /// Token specifies access to a single or multiple APIs along with the referers that are allowed to use the specified apis;
    /// Token can specify per API license restrictions OR relaxation; license checkups are defined on a per API basis
    /// Token is tied up with an organisation. So one token can only be assigned to one organisation
    /// </summary>
    public partial class Token : Base
    {
        static Token()
        {
            BaseObjectTypeIdentifierExtensions.RegisterTypeIdentifier(MethodBase.GetCurrentMethod().DeclaringType,
                Guid.Parse("929c0da1-e910-4dc7-b945-f69490503c2a"));
        }

        public Token()
        {
            Referrers = new SerializableListOfString();
        }
    }
}
