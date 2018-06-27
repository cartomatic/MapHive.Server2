using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapHive.Core.DataModel
{
    public enum LicenseOptionDataType
    {
        Boolean,
        Integer,
        Number,
        String,
        Date,
        Time
    }

    public class LicenseOption
    {
        /// <summary>
        /// Type of the stored data
        /// </summary>
        public LicenseOptionDataType Type { get; set; }

        /// <summary>
        /// Stored Value
        /// </summary>
        public object Value { get; set; }

        //Note: here potentially we may have more stuff coming: min value, max value, allowed ranges etc... So basically it may happen this object will actuall grow

        /// <summary>
        /// Whether or not this is an inherited value; if marked as inherited, will be discarded when saving
        /// </summary>
        public bool? Inherited { get; set; }
    }
}
