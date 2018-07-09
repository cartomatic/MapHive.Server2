using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapHive.API.Core
{
    /// <summary>
    /// a simple static shareable settings container
    /// </summary>
    public static class CommonSettings
    {
        private static Dictionary<string, object> _data = new Dictionary<string, object>();

        /// <summary>
        /// gets a string setting
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Get(string key)
        {
            return Get<string>(key);
        }

        /// <summary>
        /// Gets a settingg by key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            if (_data.ContainsKey(key))
                return (T) _data[key];

            return default(T);
        }

        /// <summary>
        /// sets a valu in the settings obj
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public static void Set(string key, object obj)
        {
            _data[key] = obj;
        }
    }
}
