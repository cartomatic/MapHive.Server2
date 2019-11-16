using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DataModel;
using MapHive.Core.DataModel.Map;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DAL
{
    /// <summary>
    /// Interface for a DbContext that provides access to map stuff
    /// </summary>
    public interface IMapDbContext
    {
        DbSet<DataStore> DataStores { get; set; }
        DbSet<Layer> Layers { get; set; }
    }

    public interface IMapDbContext<TDataStore, TLayer> : IMapDbContext
        where TDataStore: DataStoreBase
        where TLayer: LayerBase
    {
        new DbSet<TDataStore> DataStores { get; set; }
        new DbSet<TLayer> Layers { get; set; }
    }
}
