using Cartomatic.Utils.Dto;

namespace MapHive.Core.DataModel
{
    public partial class Organization
    {
        /// <summary>
        /// returns an org object but with all the sensitive data truncated
        /// </summary>
        /// <returns></returns>
        public Organization AsSafe()
        {
            var orgCopy = this.CopyPublicPropertiesToNew<Organization>();

            orgCopy.Databases = null;
            orgCopy.EncryptedDatabases = null;

            return orgCopy;
        }
    }
}
