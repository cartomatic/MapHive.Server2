using Microsoft.EntityFrameworkCore;

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
