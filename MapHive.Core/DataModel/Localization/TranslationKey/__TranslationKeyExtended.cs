using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MapHive.Core.DataModel
{
    public partial class TranslationKeyExtended : TranslationKeyBase
    {
        /// <summary>
        /// A view to map the object to; used in the type config but also in the seed method
        /// </summary>
        public static readonly string ViewName = "translation_keys_extended";

        //IMPORTANT for linking objects!
        //make sure to expose the same type uuid as the parent object
        public override Guid TypeUuid => BaseObjectTypeIdentifierExtensions.GetTypeIdentifier<TranslationKey>();


        /// <summary>
        /// Application the translation class comes from
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Translation class name
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 1st level of inheritance
        /// </summary>
        public string InheritedClassName { get; set; }


        //make the write methods fail for this object
        //-------------------------------------------
        private static readonly string ReadOnlyExceptionMsg =
            $"This object is read only mate. Use the original class for write ops: {nameof(TranslationKey)}";

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
