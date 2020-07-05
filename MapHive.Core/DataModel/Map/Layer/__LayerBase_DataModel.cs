using Newtonsoft.Json;
using System;
using System.Threading;

namespace MapHive.Core.DataModel.Map
{
    /// <summary>
    /// basic model for the layer objects
    /// </summary>
    public abstract partial class LayerBase
    {
        /// <summary>
        /// Identifier of a map this layer is assigned to if any
        ///
        /// Basically a layer may be a 'common' layer OR a 'map' layer;
        /// when a layer is a map layer it is only visible at a specified map's level
        /// null value of mapId means this is a 'common' layer
        /// </summary>
        public Guid? MapId { get; set; }

        /// <summary>
        /// Name of a map this layer is assigned to; this property is not persisted and usually populated when reading in map context
        /// </summary>
        public string MapName { get; set; }

        /// <summary>
        /// Layer identifier - a key based on which a client app can provide customized UI
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// Type of a layer
        /// </summary>
        public LayerType Type { get; set; }

        /// <summary>
        /// Whether or not a layer is a default layer. A default layer as such is not removable and can have an identifier that is used by a client application to provide
        /// layer specific UI
        /// </summary>
        public bool? IsDefault { get; set; }

        /// <summary>
        /// Identifier of a layer that should be used as a base layer for this one.
        /// When present, layer definition acts as an override - overrides properties that have different values than the parent
        /// </summary>
        public Guid? SourceLayerId { get; set; }

        /// <summary>
        /// Order of layers within a project; this field only makes sense for project layers
        /// </summary>
        public int? Order { get; set; }

        /// <summary>
        /// Client name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Client description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Whether or not a layer should be visible when added to a layer; this is controlled at a project level
        /// </summary>
        public bool? Visible { get; set; }


        /// <summary>
        /// Start displaying layer at a scale specified (while zooming in when to START displaying, while zooming out when to STOP displaying);
        /// layer is displayed when scale is <= the value specified; when a value is not specified, restriction is not applied
        /// </summary>
        public decimal? VisibilityScaleMin { get; set; }

        /// <summary>
        /// Stop displaying layer at a scale specified (while zooming in when to STOP displaying, while zooming out when to START displaying);
        /// layer is displayed when scale >= the value specified; when a value is not specified, restriction is not applied
        /// </summary>
        public decimal? VisibilityScaleMax { get; set; }

        /// <summary>
        /// Whether or not layer features can be querried for info
        /// </summary>
        public bool? Queryable { get; set; }

        /// <summary>
        /// Whether or not layer features can be selected
        /// </summary>
        public bool Selectable { get; set; }

        
        /// <summary>
        /// Identifier of a data store - a unified db data source that could have been imported into db from shp, geoJson, csv, service, etc
        /// </summary>
        public Guid? DataStoreId { get; set; }


        /// <summary>
        /// Layer metadata - extra information that describes a layer and makes it possible to consume it
        /// </summary>
        public LayerMetadata Metadata { get; set; }


        /// <summary>
        /// Serialized metadata
        /// </summary>
        [JsonIgnore]
        public string MetadataSerialized
        {
            get => Metadata != null ? JsonConvert.SerializeObject(Metadata) : null;

            set
            {
                if (string.IsNullOrEmpty(value))
                    Metadata = null;
                else
                    try
                    {
                        Metadata = JsonConvert.DeserializeObject<LayerMetadata>(value);
                    }
                    catch
                    {
                        //ignore
                    }
            }

        }

        /// <summary>
        /// Underlying data source for this layer. 
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
        /// Styles definition of this layer
        /// </summary>
        public SerializableListOfStyle Styles { get; set; }

        /// <summary>
        /// Serialized Styles - for db storage
        /// </summary>
        [JsonIgnore]
        public string StylesSerialized
        {
            get => Styles?.Serialized;
            set
            {
                if (Styles == null)
                    Styles = new SerializableListOfStyle();

                Styles.Serialized = value;
            }
        }


        /// <summary>
        /// Widgets to be displayed for this layer
        /// </summary>
        public SerializableListOfWidget Widgets { get; set; }

        /// <summary>
        /// Serialized Widgets - for db storage
        /// </summary>
        [JsonIgnore]
        public string WidgetsSerialized
        {
            get => Widgets?.Serialized;
            set
            {
                if (Widgets == null)
                    Widgets = new SerializableListOfWidget();

                Widgets.Serialized = value;
            }
        }

        /// <summary>
        /// a protected property for the public SourceLayer container; used so does not have to fight deserialization to abstract LayerBase in some cases
        /// </summary>
        protected LayerBase _sourceLayer { get; set; }

        /// <summary>
        /// Source layer; this field is a convenience data container used when reading single layer data and in most cases will not hold a value;
        /// It is here, so can output extra data when returning a complete layer record
        /// </summary>
        public LayerBase SourceLayer => _sourceLayer;


        /// <summary>
        /// cleans up layer's sensitive data
        /// </summary>
        /// <param name="cleanLayerMetadata"> whether or not should clean the metadata too</param>
        public void CleanSensitiveData(bool cleanLayerMetadata = true)
        {
            CleanDataSourceCredentialsUserInfo(DataSource?.DataSourceCredentials);
            CleanDataSourceCredentialsUserInfo(_sourceLayer?.DataSource?.DataSourceCredentials);

            if (cleanLayerMetadata)
            {
                CleanMetadataCredentialsUserInfo(Metadata);
                CleanMetadataCredentialsUserInfo(_sourceLayer?.Metadata);
            }
        }

        /// <summary>
        /// cleans up layer data source sensitive info
        /// </summary>
        /// <param name="ds"></param>
        protected void CleanDataSourceCredentialsUserInfo(Cartomatic.Utils.Data.DataSourceCredentials ds)
        {
            if (ds == null)
                return;

            ds.UserName = null;
            ds.ServiceUserName = null;
            ds.Pass = null;
            ds.ServiceUserPass = null;
        }

        /// <summary>
        /// cleans up layer metadata credentials
        /// </summary>
        /// <param name="m"></param>
        protected internal void CleanMetadataCredentialsUserInfo(LayerMetadata m)
        {
            if (m == null)
                return;

            m.Password = null;
            m.UserName = null;
        }
    }
}
