using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
