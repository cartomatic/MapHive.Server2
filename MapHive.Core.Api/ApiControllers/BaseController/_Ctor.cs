using System;

namespace MapHive.Core.Api.ApiControllers
{
    /// <summary>
    /// Provides the base for the Web APIs, so generic utils can be easily shared
    /// </summary>
    public abstract partial class BaseController : Microsoft.AspNetCore.Mvc.ControllerBase, IDisposable
    {
        /// <summary>
        /// Disposes object
        /// </summary>
        public virtual void Dispose()
        {
        }
    }
}
