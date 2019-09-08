using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.JsonSerializableObjects;
using Newtonsoft.Json;

namespace MapHive.Core.DataModel
{
    /// <summary>
    /// Roles are the containers for system level and app level priviliges;
    /// A role as such does not have to define any priviliges as it is up to an application to properly define and understand a role meaning;
    /// </summary>
    public partial class Role
    {
        /// <summary>
        /// identifier of a role; some roles may have special meaning and use identifiers to uniquely address them
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// Name of a role
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Description of a role
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        /// Privileges granted by this role
        /// </summary>
        public SerializableListOfPrivilege Privileges { get; set; }

        [JsonIgnore]
        public string PrivilegesSerialized
        {
            get => Privileges?.Serialized;
            set
            {
                if (value != null)
                {
                    if (Privileges == null)
                    {
                        Privileges = new SerializableListOfPrivilege();
                    }
                    Privileges.Serialized = value;
                }
                else
                {
                    Privileges = null;
                }
            }
        }
    }
}
