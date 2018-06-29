using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.JsonSerializableObjects;
using Newtonsoft.Json;

namespace MapHive.Core.DataModel
{
    public partial class Token
    {
        //Note: the actual token uuid is an api key
        

        /// <summary>
        /// Token name, so it is possible to have sensible token naming conventions
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Token description - used for more detailed description if required
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        /// An organization a token is assigned to
        /// </summary>
        public Guid? OrganizationId { get; set; }

        /// <summary>
        /// Application identifier this tokens grants asccess to
        /// </summary>
        public Guid? ApplicationId { get; set; }

        /// <summary>
        /// Referrers that are granted access via this token
        /// </summary>
        public SerializableListOfString Referrers { get; set; }

        [JsonIgnore]
        public string ReferrersSerialized
        {
            get => Referrers?.Serialized;
            set
            {
                if (Referrers != null)
                {
                    Referrers.Serialized = value;
                }
            }
        }

        //Note:
        //perhaps we should also assume excluded referrers for scenarios of token misusage

        /// <summary>
        /// Whether or not when verifying the access rights via token it is allowed to grant access for requests without a recognised referrer
        /// </summary>
        public bool CanIgnoreReferrer { get; set; }
        

        /// <summary>
        /// Whether or not a token is a master token; master means token can impersonate any organization - when using this token it is possible to access every org
        /// resources. When token is Master it should really be kept secured...
        /// </summary>
        public bool IsMaster { get; set; }

        //Note:
        //If required we can add a 'parent' token, so we can group more tokens under one
        //could be addressed when we have such requirement
        //for the time being a single token grants a single app access

        //Note:
        //token specific license restrictions
        //potentially we could restrict / relax (not so sure about the latter though) licence restrictions on a token level
    }
}
