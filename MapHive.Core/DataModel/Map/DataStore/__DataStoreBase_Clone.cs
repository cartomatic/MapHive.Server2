using Cartomatic.Utils.Dto;
using System.Collections.Generic;
using System.Linq;

namespace MapHive.Core.DataModel.Map
{
    public abstract partial class DataStoreBase
    {
        public abstract DataStoreBase Clone(bool resetCols = false);
    }
}
