using System.Collections.Generic;
using System.Threading.Tasks;

namespace MapHive.Core.Configuration
{
    public interface IConfiguration
    {
        /// <summary>
        /// Reads configuration for a specified scenario;
        /// </summary>
        /// <returns></returns>
        Task<IDictionary<string, object>> ReadAsync();
    }
}
