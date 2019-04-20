using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.JsonSerializableObjects;
using Newtonsoft.Json;

namespace MapHive.Core.DataModel
{
    public abstract partial class Base
    {
        /// <inheritdoc />
        /// <summary>
        /// Unique object identifier; generated automatically upon saving; See DAL MapHiveDatabaseContextBase for more details
        /// </summary>
        public Guid Uuid { get; set; }


        //Note: automated data updates relating to IBase: hooked into OnSaving. See DAL BaseDbContext for more details
        
        /// <inheritdoc />
        /// <summary>
        /// Object creator - updated automatically
        /// </summary>
        public Guid? CreatedBy { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Object last modified by - updated automatically
        /// </summary>
        public Guid? LastModifiedBy { get; set; }

        
        /// <inheritdoc />
        /// <summary>
        /// Create date - updated automatically
        /// </summary>
        public DateTime? CreateDateUtc { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Modify date - updated automatically
        /// </summary>
        public DateTime? ModifyDateUtc { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// End date - updated automatically
        /// </summary>
        public DateTime? EndDateUtc { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// A custom data bag, so can save extra information with core objects, make it searchable and avoid having to extend a core api model when not strictly necessary
        /// </summary>
        public SerializableDictionaryOfObject CustomData { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Serialized custom data used as a backing field for EF model
        /// </summary>
        [JsonIgnore]
        string IBase.CustomDataSerialized
        {
            get => CustomData?.Serialized;
            set
            {
                if (CustomData != null)
                    CustomData.Serialized = value;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Object relations defined as set of links; this object is ignored when object is saved and is used only to provide a DIFF of links that should be applied to the db representation
        /// </summary>
        public ILinksDiff Links { get; set; } = new LinksDiff();
        
        /// <inheritdoc />
        /// <summary>
        /// When an object is used as a link, it may have some extra data; this property is not db mapped, but used for scenarios when such extra information is required
        /// </summary>
        public LinkData LinkData { get; set; }
    }
}
