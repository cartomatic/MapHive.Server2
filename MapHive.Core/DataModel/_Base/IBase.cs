using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.JsonSerializableObjects;

namespace MapHive.Core.DataModel
{

    /// <summary>
    /// A minimum required Base class model / functionality necessary to perform some standardised ops
    /// </summary>
    public interface IBase
    {
        /// <summary>
        ///  Type identifier - used to establish links between objects. not saved in a database;
        /// declared via class constructor.
        /// Important: when uuid is changed in code it will affect all the links in the database(s)
        /// </summary>
        Guid TypeUuid { get; }

        /// <summary>
        /// Unique object identifier; generated automatically upon saving; See DAL MapHiveDatabaseContextBase for more details
        /// </summary>
        Guid Uuid { get; set; }

        /// <summary>
        /// Object creator - updated automatically
        /// </summary>
        Guid? CreatedBy { get; set; }

        /// <summary>
        /// Object last modified by - updated automatically
        /// </summary>
        Guid? LastModifiedBy { get; set; }

        /// <summary>
        /// Create date - updated automatically
        /// </summary>
        DateTime? CreateDateUtc { get; set; }

        /// <summary>
        /// Modify date - updated automatically
        /// </summary>
        DateTime? ModifyDateUtc { get; set; }

        /// <summary>
        /// End date - updated automatically
        /// </summary>
        DateTime? EndDateUtc { get; set; }

        /// <summary>
        /// A custom data bag, so can save extra information with core objects, make it searchable and avoid having to extend a core api model when not strictly necessary
        /// </summary>
        SerializableDictionaryOfObject CustomData { get; set; }

        /// <summary>
        /// EF Core does not support complex types yet...
        /// Will be changed at some point, whenever this is possible to map nested properties such as SerializableDictionaryOfObject.Serialized
        /// </summary>
        string CustomDataSerialized { get; set; }

        /// <summary>
        /// Object relations defined as set of links; this object is ignored when object is saved and is used only to provide a DIFF of links that should be applied to the db representation
        /// </summary>
        ILinksDiff Links { get; set; }

        /// <summary>
        ///  When an object is used as a link, it may have some extra data; this property is not db mapped, but used for scenarios when such extra information is required
        /// </summary>
        LinkData LinkData { get; set; }
    }
}
