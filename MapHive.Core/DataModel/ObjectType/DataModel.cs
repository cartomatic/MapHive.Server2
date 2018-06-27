using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapHive.Core.Data
{
    public partial class ObjectType
    {
        /// <summary>
        /// Type guid
        /// </summary>
        public Guid Uuid { get; set; }

        /// <summary>
        /// Full object name
        /// </summary>
        public string Name { get; set; }
    }
}
