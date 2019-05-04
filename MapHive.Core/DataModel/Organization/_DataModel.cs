using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.JsonSerializableObjects;
using Newtonsoft.Json;

namespace MapHive.Core.DataModel
{
    public partial class Organization
    {
        /// <summary>
        /// used as the org identifier. this must be unique within the system.
        /// When an org is created for a user (in a batch account creation process) its name will be based on the user slug with an '-org' suffix;
        /// It is possible to change the org slug to an arbitrary value provided it is unique within the system
        /// No spaces are allowed and chars must be allowed in the url
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Organization display name
        /// </summary>
        public string DisplayName { get; set; }
        
        
        /// <summary>
        /// Org descrioption
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        /// some info on user location; an address, coords, place name, whatever.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Url of an org's public site
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// org contact email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// When set, user's image will be pulled from the gravatar service
        /// </summary>
        public string GravatarEmail { get; set; }


        /// <summary>
        /// Profile picture. When present it will be used in the profile editor and whenever user info is required (log on info, msngrs, etc);
        /// This property is used to suck in the data when saving; pictures themselves are stored separately
        /// </summary>
        public string ProfilePicture { get; set; }

        /// <summary>
        /// Id of a profile picture. When present a picture is available. When not present picture, if any, is deleted
        /// </summary>
        public Guid? ProfilePictureId { get; set; }

        //billing related stuff

        /// <summary>
        /// billing email if different than the contact email
        /// </summary>
        public string BillingEmail { get; set; }
        

        /// <summary>
        /// Billing address
        /// </summary>
        public string BillingAddress { get; set; }


        /// <summary>
        /// Extra information to be put on an invoice such as VAT No, Registration No, etc
        /// </summary>
        public SerializableDictionaryOfString BillingExtraInfo { get; set; }

        [JsonIgnore]
        public string BillingExtraInfoSerialized
        {
            get => BillingExtraInfo?.Serialized;
            set
            {
                if (BillingExtraInfo != null)
                {
                    BillingExtraInfo.Serialized = value;
                }
            }
        }


        /// <summary>
        /// Object contains properties and methods (connect, create) for organization database
        /// </summary>
        public virtual List<OrganizationDatabase> Databases { get; set; }

        /// <summary>
        /// dbs in an encrypted form, so the data can be transferred safely over the internet
        /// </summary>
        public string EncryptedDatabases { get; set; }

        /// <summary>
        /// Applications assigned to organization
        /// </summary>
        /// <remarks>
        /// This property is used as a placeholder for data and for linked data retrieval; it will not always be populated so do not rely on its existence
        /// </remarks>
        public virtual List<Application> Applications { get; set; }

        /// <summary>
        /// Users assigned to organization
        /// </summary>
        /// <remarks>
        /// This property is used as a placeholder for data and for linked data retrieval; it will not always be populated so do not rely on its existence
        /// </remarks>
        public virtual List<MapHiveUser> Users { get; set; }

        /// <summary>
        /// Roles assigned to organization
        /// </summary>
        /// <remarks>
        /// This property is used as a placeholder for data and for linked data retrieval; it will not always be populated so do not rely on its existence
        /// </remarks>
        public virtual List<Role> Roles { get; set; }


        /// <summary>
        /// Owners of an organisation; see Organisation GetOwnersAsync for details
        /// </summary>
        /// <remarks>
        /// This property is used as a placeholder for data and for linked data retrieval; it will not always be populated so do not rely on its existence
        /// </remarks>
        public virtual List<MapHiveUser> Owners { get; set; }

        /// <summary>
        /// Admins of an organisation; see Organisation GetAdminsAsync for details
        /// </summary>
        /// <remarks>
        /// This property is used as a placeholder for data and for linked data retrieval; it will not always be populated so do not rely on its existence
        /// </remarks>
        public virtual List<MapHiveUser> Admins { get; set; }

        /// <summary>
        /// org license opts
        /// </summary>
        public OrganizationLicenseOptions LicenseOptions { get; set; }

        [JsonIgnore]
        public string LicenseOptionsSerialized
        {
            get => LicenseOptions?.Serialized;
            set
            {
                if (LicenseOptions != null)
                {
                    LicenseOptions.Serialized = value;
                }
            }
        }

        /// <summary>
        /// Visual identification artifacts such as: OrgSwitcherIcon, BaseColor, etc.
        /// </summary>
        public SerializableDictionaryOfObject VisualIdentification { get; set; }

        /// <summary>
        /// Visual identification serialized for db storage
        /// </summary>
        [JsonIgnore]
        public string VisualIdentificationSerialized
        {
            get => VisualIdentification.Serialized;
            set => VisualIdentification.Serialized = value;
        }
    }
}
