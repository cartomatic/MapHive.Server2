using System;
using System.Collections.Generic;
using System.Text;

namespace MapHive.Core.DataModel
{
    /// <summary>
    /// describes a resource access privilege
    /// </summary>
    public class Privilege
    {
        /// <summary>
        /// type identifier this privilege controls access to
        /// </summary>
        public Guid? TypeId { get; set; }

        /// <summary>
        /// object identifier
        /// </summary>
        public Guid? ObjectId { get; set; }

        /// <summary>
        /// free text privilege identifier - so possible to use 
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// Whether or not can read a resource
        /// </summary>
        public bool? Read { get; set; }

        /// <summary>
        /// Whether or not can create a resource
        /// </summary>
        public bool? Create { get; set; }

        /// <summary>
        /// Whether or not can update a resource
        /// </summary>
        public bool? Update { get; set; }

        /// <summary>
        /// Whether or not can destroy a resource
        /// </summary>
        public bool? Destroy { get; set; }

        /// <summary>
        /// Data container for custom privilege data
        /// </summary>
        public Dictionary<string, object> PrivilegeData { get; set; }
    }
}
