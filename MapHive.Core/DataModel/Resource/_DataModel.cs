using System;
using System.Collections.Generic;
using System.Text;

namespace MapHive.Core.DataModel
{
    public partial class Resource
    {
        /// <summary>
        /// Id of the resource owner if any
        /// </summary>
        public Guid? OwnerId { get; set; }

        /// <summary>
        /// Owner type identifier
        /// </summary>
        public Guid? OwnerTypeId { get; set; }

        /// <summary>
        /// Identifier, so it is possible to recognise it properly
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// original file name if any
        /// </summary>
        public string OriginalFileName { get; set; }

        /// <summary>
        /// Mime if any
        /// </summary>
        public string Mime { get; set; }

        /// <summary>
        /// resource data
        /// </summary>
        public byte[] Data { get; set; }
    }
}
