using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MapHive.Core.DataModel
{
    /// <summary>
    /// License options used to specify defualt opts for an application, module, data source
    /// </summary>
    public partial class LicenseOptions : Dictionary<string, LicenseOption>
    {
        private static JsonSerializerSettings JsonSerializerSettings => new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };


        /// <summary>
        /// This property is used so EF can nicely read/write and persist the data without a messy setup
        /// The TypeConfiguration setup should look like this: Property(p => p.LinkData.Serialized).HasColumnName("some_column_name");
        /// </summary>
        /// <remarks>
        /// Idea ported from MapHive's LinkData / Translations
        /// </remarks>
        [JsonIgnore]
        public string Serialized
        {
            get
            {
                //cleanup the Inherited valuess
                CleanInherited();

                return JsonConvert.SerializeObject(this, Formatting.None, JsonSerializerSettings); ;
            }
            set
            {
                //deserialise...
                LicenseOptions incomingStringData = null;
                try
                {
                    incomingStringData = JsonConvert.DeserializeObject<LicenseOptions>(value, JsonSerializerSettings);
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
        /// safe clone of the license opts
        /// </summary>
        /// <returns></returns>
        public LicenseOptions Clone()
        {
            return
                JsonConvert.DeserializeObject<LicenseOptions>(JsonConvert.SerializeObject(this, Formatting.None,
                    JsonSerializerSettings));
        }

        /// <summary>
        /// Clenas up the inherited properties
        /// </summary>
        public void CleanInherited()
        {
            var cleanedUp = new LicenseOptions();
            foreach (var key in this.Keys)
            {
                if (this[key].Inherited != true)
                    cleanedUp.Add(key, this[key]);
            }

            //clear self, so can pump in the data
            this.Clear();

            foreach (var kv in cleanedUp)
            {
                this.Add(kv.Key, kv.Value);
            }
        }
    }
}
