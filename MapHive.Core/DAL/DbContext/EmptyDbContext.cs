using System;
using System.Collections.Generic;
using System.Data.Common;
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
    /// Context used for empty db creation
    /// </summary>
    public class EmptyDbContext : BaseDbContext
    {
        public EmptyDbContext() : base()
        {
        }

        public EmptyDbContext(DbConnection dbConnection, bool contextOwnsConnection)
            : base(dbConnection, contextOwnsConnection)
        {
        }
    }
}
