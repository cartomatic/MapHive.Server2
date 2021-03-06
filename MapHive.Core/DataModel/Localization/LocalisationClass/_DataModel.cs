﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MapHive.Core.DataModel
{
    public partial class LocalizationClass : Base
    {
        /// <summary>
        /// Application name a translation applies to; Fully qualified namespaces is achieved by combining it with the ClassName 
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Class name a translation applies to; Fully qualified namespaces is achieved by combining it with the ApplicationName 
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Specifies localization class this very class inherits from
        /// </summary>
        public string InheritedClassName { get; set; }

        /// <summary>
        /// Temp list of translation keys that belong to this class
        /// </summary>
        public virtual IEnumerable<TranslationKey> TranslationKeys { get; set; } 
    }
    
}
