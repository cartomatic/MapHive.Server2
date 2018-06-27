using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapHive.Core.Configuration
{
    public class ConfigurationReader
    {
        public static async Task<IDictionary<string, object>> Read(params IConfiguration[] configs)
        {
            var output = new Dictionary<string, object>();

            foreach (var config in configs)
            {

                if (config == null)
                    continue;

                try
                {
                    foreach (var cfg in await config.Read())
                    {
                        if (output.ContainsKey(cfg.Key))
                        {
                            throw new Exception(
                                $"{cfg.Key} configuration key has already been reserved by another configuration reader. Please change it.");
                        }
                        output.Add(cfg.Key, cfg.Value);
                    }
                }
                catch (Exception ex)
                {
                    output["Error"] = true;
                    output["ErrorMsg"] = ex.Message;
                }
            }

            return output;
        }
    }
}
