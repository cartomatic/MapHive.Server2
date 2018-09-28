using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.DAL
{
    /// <summary>
    /// Db context that uses extended views and therefore it should be safe to feed them to MapHive.Core.DAL.ExtendedViewsCreator
    /// </summary>
    public interface IProvideExtendedViews
    {
        /// <summary>
        /// Type to be used as an extended views provider
        /// </summary>
        Type ExtendedViewsProvider { get; }
    }
}
