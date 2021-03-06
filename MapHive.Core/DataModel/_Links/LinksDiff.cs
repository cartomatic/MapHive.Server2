﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapHive.Core.DataModel
{
    /// <summary>
    /// Decribes changes in the object links. Changes must be explicit in order to modify the links
    /// </summary>
    public class LinksDiff : ILinksDiff
    {
        /// <inheritdoc />
        /// <summary>
        /// Link objects to be either inserted or updated
        /// </summary>
        public List<Link> Upsert { get; set; } = new List<Link>();

        /// <inheritdoc />
        /// <summary>
        /// Link objects to be destroyed
        /// </summary>
        public List<Guid> Destroy { get; set; } = new List<Guid>();
    }
}
