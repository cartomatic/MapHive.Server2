using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DataModel;
using MapHive.Core.DataModel.Map;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DAL
{
    /// <summary>
    /// Interface for a DbContext that provides access to map stuff
    /// </summary>
    public interface IMapDbContext
    {
        DbSet<T> GetDataStoresDbSet<T>()
            where T : MapHive.Core.DataModel.Map.DataStore;

        DbSet<T> GetLayersDbSet<T>()
            where T : MapHive.Core.DataModel.Map.LayerBase;
    }
}
