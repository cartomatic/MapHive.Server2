﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.JsonSerializableObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MapHive.Core.DataModel
{
    public class LinkData : Dictionary<string, Dictionary<string,object>>, IJsonSerializable
    {
        private static JsonSerializerSettings JsonSerializerSettings => new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver()};

        [JsonIgnore]
        public string Serialized
        {
            get => JsonConvert.SerializeObject(this, Formatting.None, JsonSerializerSettings);

            set
            {
                //deserialise...
                Dictionary<string, Dictionary<string, object>> incomingStringData = null;
                try
                {
                    incomingStringData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(value, JsonSerializerSettings);
                }
                catch
                {
                    //ignore - silently fail
                }

                //do not modify self if this was invalid json...
                if (incomingStringData == null) return;
               
                //clear self, so can pump in the data
                this.Clear();

                foreach (var kv in incomingStringData)
                {
                    this.Add(kv.Key, kv.Value);
                }
            }
        }


        /// <summary>
        /// Returns an object by key, so it's not necessary to test for key presence
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetByKey(string key)
        {
            Dictionary<string, object> output = null;

            if (this.ContainsKey(key))
                output = this[key];

            return output;
        }
    }
}
