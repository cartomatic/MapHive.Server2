using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace MapHive.Core.DataModel.Map
{
    /// <summary>
    /// truncated version of a layer, so can discard metadata reads at the db level
    /// </summary>
    public class LayerTruncated : LayerBase
    {
        /// <summary>
        /// A view to map the object to; used in the type config but also in the seed method
        /// </summary>
        public static readonly string ViewName = "layers_truncated";

        //IMPORTANT for linking objects!
        //make sure to expose the same type uuid as the parent object
        public override Guid TypeUuid => BaseObjectTypeIdentifierExtensions.GetTypeIdentifier<Layer>();

        //hide some properties, so a layer list has smaller data footprint

        private new LayerMetadata Metadata { get; set; }
        private new string MetadataSerialized { get; set; }
        private new DataSource DataSource { get; set; }
        private new string DataSourceSerialized { get; set; }
        private new SerializableListOfStyle Styles { get; set; }
        private new string StylesSerialized { get; set; }
        private new SerializableListOfWidget Widgets { get; set; }
        private new string WidgetsSerialized { get; set; }

        //make the write methods fail for this object
        //-------------------------------------------
        private static readonly string ReadOnlyExceptionMsg =
            $"This object is read only mate. Use the original class for write ops: {nameof(Layer)}";

        protected internal override Task<T> UpdateAsync<T>(DbContext dbCtx, Guid uuid)
        {
            throw new Exception(ReadOnlyExceptionMsg);
        }

        protected internal override Task<T> CreateAsync<T>(DbContext dbCtx)
        {
            throw new Exception(ReadOnlyExceptionMsg);
        }

        protected internal override Task<T> DestroyAsync<T>(DbContext dbCtx, Guid uuid)
        {
            throw new Exception(ReadOnlyExceptionMsg);
        }
    }
}
