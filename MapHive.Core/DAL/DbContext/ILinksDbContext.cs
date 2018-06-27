using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if NETFULL
using System.Data.Entity;
#endif
#if NETSTANDARD
using Microsoft.EntityFrameworkCore;
#endif

namespace MapHive.Core.DAL
{
    /// <summary>
    /// Db context that can save object links
    /// </summary>
    public interface ILinksDbContext
    {
        /// <summary>
        /// A collection of link objects that define object relations
        /// </summary>
        DbSet<MapHive.Core.DataModel.Link> Links { get; set; }
    }
}
