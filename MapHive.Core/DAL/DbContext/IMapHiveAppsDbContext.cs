using MapHive.Core.DataModel;

using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DAL
{
    /// <summary>
    /// Whether or not a dbctx has access to the maphive Applications
    /// </summary>
    public interface IMapHiveAppsDbContext
    { 
        DbSet<Application> Applications { get; set; } 
    }
}