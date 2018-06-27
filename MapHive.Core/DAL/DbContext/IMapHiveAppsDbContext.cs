using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DataModel;

#if NETFULL
using System.Data.Entity;
#endif
#if NETSTANDARD
using Microsoft.EntityFrameworkCore;
#endif

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