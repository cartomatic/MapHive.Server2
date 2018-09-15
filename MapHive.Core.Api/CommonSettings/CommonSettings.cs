using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapHive.Core.Api
{
    /// <summary>
    /// a simple static shareable settings container
    /// </summary>
    public static class CommonSettings
    {
        private static readonly Dictionary<string, object> Data = new Dictionary<string, object>();

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
            if (Data.ContainsKey(key))
                return (T) Data[key];

            return default(T);
        }

        /// <summary>
        /// sets a valu in the settings obj
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public static void Set(string key, object obj)
        {
            Data[key] = obj;
        }
    }
}
