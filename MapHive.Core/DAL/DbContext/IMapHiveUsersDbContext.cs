using MapHive.Core.DataModel;

using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DAL
{
    /// <summary>
    /// Whether or not a dbctx has access to the maphive users set
    /// </summary>
    public interface IMapHiveUsersDbContext <T>
        where T: MapHiveUserBase
    { 
        DbSet<T> Users { get; set; } 
    }
}