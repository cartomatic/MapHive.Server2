using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapHive.Core.DataModel
{
    public partial class MapHiveUser
    {
        public string GetSlug()
        {
            EnsureSlug();
            return Slug;
        }

        /// <summary>
        /// makes sure the slug is present and valid
        /// </summary>
        protected void EnsureSlug()
        {
            //Note:
            //all users have slugs now, not only org users.
            //There is no point in using slugs only for some users

            //use email for a slug - it must be unique
            if (string.IsNullOrEmpty(Slug))
                Slug = Email;

            Slug = Utils.Slug.SanitizeSlug(Slug);
        }
    }
}
