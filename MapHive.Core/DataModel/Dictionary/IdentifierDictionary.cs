using System;
using System.Collections.Generic;
using System.Text;

namespace MapHive.Core.DataModel.Dictionary
{
    /// <summary>
    /// Identifier dictionary - enforces a presence of a unique Identifier field
    /// </summary>
    public class IdentifierDictionary : SimpleDictionary, IIdentifierDictionary
    {
        /// <inheritdoc />
        public string Identifier { get; set; }
    }
}
