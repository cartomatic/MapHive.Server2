using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapHive.Core.DataModel
{
    public partial class Organization
    {
        /// <summary>
        /// makes sure the slug is present and valid
        /// </summary>
        protected void EnsureSlug()
        {
            Slug = Utils.Slug.SanitizeSlug(Slug);

            //fallback for org uuid if no slug provided
            if (string.IsNullOrWhiteSpace(Slug))
                Slug = Uuid.ToString();
        }
    }
}
