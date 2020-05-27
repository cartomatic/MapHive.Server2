using System;
using System.Reflection;
using MapHive.Core.DataModel;

namespace MapHive.Core.DataModel
{
    /// <summary>
    /// Customer
    /// </summary>
    public partial class Resource : MapHive.Core.DataModel.Base
    {
        static Resource()
        {
            BaseObjectTypeIdentifierExtensions.RegisterTypeIdentifier(MethodInfo.GetCurrentMethod().DeclaringType, Guid.Parse("5616a05b-3fa2-40bd-8b8e-29a5ccce5862"));
        }

        /// <summary>
        /// Creates new insance
        /// </summary>
        public Resource()
        {
        }

        /// <summary>
        /// Creates new instance from base 64 instance
        /// </summary>
        /// <param name="base64Data"></param>
        public Resource(string base64Data)
        {
            var data = base64Data.Split(new[] { ";base64," }, StringSplitOptions.None);

            if (data.Length != 2)
                return;

            Mime = data[0].Replace("data:", string.Empty);
            Data = Convert.FromBase64String(data[1]);
        }
    }
}
