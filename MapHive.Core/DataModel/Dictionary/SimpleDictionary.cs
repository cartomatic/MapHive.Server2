using System;
using System.Collections.Generic;
using System.Text;

namespace MapHive.Core.DataModel.Dictionary
{
    /// <summary>
    /// Simple name / descriptin dictionary
    /// </summary>
    public class SimpleDictionary : Base, ISimpleDictionary
    {
        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public string Description { get; set; }
    }
}
