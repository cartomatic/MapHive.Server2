using Newtonsoft.Json;
using System;

namespace MapHive.Core.DataModel.Map
{
    public abstract partial class DataStoreBase
    {
        /// <summary>
        /// A source name / file name when known
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Whether or not data store is used by a layer; when not in use, a cleanup bot will pick it up and remove along with any mvt cache that could have been created
        /// </summary>
        public bool? InUse { get; set; }

        /// <summary>
        /// Data bbox min x
        /// </summary>
        public double? MinX { get; set; }

        /// <summary>
        /// Data bbox min y
        /// </summary>
        public double? MinY { get; set; }

        /// <summary>
        /// Data bbox max x
        /// </summary>
        public double? MaxX { get; set; }

        /// <summary>
        /// Data bbox max y
        /// </summary>
        public double? MaxY { get; set; }

        /// <summary>
        /// db data source for this data store
        /// </summary>
        public DataSource DataSource { get; set; }

        /// <summary>
        /// Serialized DataSource - for db storage
        /// </summary>
        [JsonIgnore]
        public string DataSourceSerialized
        {
            get => DataSource != null ? JsonConvert.SerializeObject(DataSource) : null;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    DataSource = null;
                }
                else
                {
                    try
                    {
                        DataSource = JsonConvert.DeserializeObject<DataSource>(value);
                    }
                    catch
                    {
                        //ignore
                    }
                }
            }
        }

        /// <summary>
        /// Linked data stores - used in a case this is an 'umbrella' data source and real data lie in more than one table
        /// </summary>
        public Guid[] LinkedDataStoreIds { get; set; }

        /// <summary>
        /// serialized linked data stores
        /// </summary>
        [JsonIgnore]
        public string LinkedDataStoreIdsSerialized
        {
            get => LinkedDataStoreIds != null ? JsonConvert.SerializeObject(LinkedDataStoreIds) : null;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    LinkedDataStoreIds = null;
                }
                else
                {
                    try
                    {
                        LinkedDataStoreIds = JsonConvert.DeserializeObject<Guid[]>(value);
                    }
                    catch
                    {
                        //ignore
                    }
                }
            }
        }

        /// <summary>
        /// A helper runtime only property; not stored in the DB
        /// </summary>
        [JsonIgnore]
        public virtual DataStoreBase[] LinkedDataStores { get; set; }
    }
}
