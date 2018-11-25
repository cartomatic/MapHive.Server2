using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapHive.Core.DataModel
{
    public partial class MapHiveUser
    {
        /// <summary>
        /// Forename
        /// </summary>
        public string Forename { get; set; }

        /// <summary>
        /// surname
        /// </summary>
        public string Surname { get; set; }
        
        /// <summary>
        /// Contact phone
        /// </summary>
        public string ContactPhone { get; set; }

        /// <summary>
        /// used only when user is an independent user. Slug name becomes an org slug with an '-org' suffix. This way it is possible to maintain understandable urls when working in a context of a user/org
        /// </summary>
        public string Slug { get; set; }


        /// <summary>
        /// Some basic info on the user. Perhaps in a form of html.
        /// </summary>
        public string Bio { get; set; }

        /// <summary>
        /// Name of the company a user works for
        /// </summary>
        public string Company { get; set; }


        /// <summary>
        /// Dept a user work for
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// some info on user location; an address, coords, place name, whatever.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// When set, user's image will be pulled from the gravatar service
        /// </summary>
        public string GravatarEmail { get; set; }


        /// <summary>
        /// Profile picture data. When parsable to guid it is simply an identifier of a resource. otherwise it is assumed it is base 64 encoded picture data
        /// </summary>
        public string ProfilePicture { get; set; }


        /// <summary>
        /// Whether or not user is an organization user. Being an organization user means user does not have its own organization to work under and instead can only work in the context of other orgs he is linked to; this will usually be only one organization, but technically a user can be linked to as many orgs as required
        /// </summary>
        public bool IsOrgUser { get; set; }

        /// <summary>
        /// If a user is an organization user, this property contains an identifier of an organization he was created under.
        /// </summary>
        public Guid? ParentOrganizationId { get; set; }

        /// <summary>
        /// Whether or not a user should be visible in the users catalogue; By default, when a user is an OrgUser ('belongs' to an organization) he is not visible in the catalogue
        /// setting this property to true will cause the user will become findable in the catalogue.
        /// </summary>
        public bool VisibleInCatalogue { get; set; }


        /// <summary>
        /// identifier of an organization that is connected to user profile
        /// </summary>
        public Guid? UserOrgId { get; set; }


        /// <summary>
        /// a role within an organization. this property is not db mapped as its content depends on the orgsanisation context
        /// </summary>
        public Organization.OrganizationRole? OrganizationRole { get; set; }
    }
}
