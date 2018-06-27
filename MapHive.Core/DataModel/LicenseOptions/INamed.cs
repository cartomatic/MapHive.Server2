using System;
using System.Collections.Generic;
using System.Text;

namespace MapHive.Core.DataModel
{
    /// <summary>
    /// Enforces a string Name property
    /// </summary>
    public interface INamed
    {
        string Name { get; set; }
    }
}
