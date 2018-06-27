using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.JsonSerializableObjects;
using Newtonsoft.Json;

namespace MapHive.Core.DataModel
{
    /// <inheritdoc />
    /// <summary>
    /// Whether or not a link object contains additional data
    /// </summary>
    public interface ILinkData : IJsonSerializable
    {
    }
}
