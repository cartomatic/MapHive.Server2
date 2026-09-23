using System;

namespace MapHive.Core.DataModel
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
