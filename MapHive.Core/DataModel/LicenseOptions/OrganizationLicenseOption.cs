using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapHive.Core.DataModel
{
    public class OrganizationLicenseOption
    {
        /// <summary>
        /// type identifier of the licensed object - used to perform object lookups
        /// </summary>
        public Guid LicensedObjectTypeUuid { get; set; }

        /// <summary>
        /// identifier of a licensed object - used to perform lookups
        /// </summary>
        public Guid LicensedObjectUuid { get; set; }

        /// <summary>
        /// Human readable licensed object type name; one should not relay on neither this param presence nor its value as it may change without a notice!
        /// </summary>
        public string LicensedObjectType { get; set; }

        /// <summary>
        /// human readable licensed object name; one should not relay on neither this param presence nor its value as it may change without a notice!
        /// </summary>
        public string LicensedObjectName { get; set; }

        /// <summary>
        /// The actual license options
        /// </summary>
        public LicenseOptions LicenseOptions { get; set; }
    }
}
