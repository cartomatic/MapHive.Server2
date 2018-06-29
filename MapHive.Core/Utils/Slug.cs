using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapHive.Core.Utils
{
    public class Slug
    {
        /// <summary>
        /// Sanitizes slug - replaces [@.] with a dash 
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        public static string SanitizeSlug(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                return null;
            }

            //firs chars repalcement
            slug = slug
                .Replace("@", "-")
                .Replace(".", "-");

            //this is for org slugs based on org names
            var tokens = slug.Split(' ').Select(str => !string.IsNullOrEmpty(str));

            slug = string.Join("-", tokens);

            return slug;

            //todo - validate slug. it can only have a subset of chars, basically [A-Za-z0-9]
        }

        /// <summary>
        /// works out a slug for an organization from org name and org owner email. if one does not qualify as a slug, a second one is used
        /// </summary>
        /// <param name="orgName"></param>
        /// <param name="ownerEmail"></param>
        /// <returns></returns>
        public static string GetOrgSlug(string orgName, string ownerEmail)
        {
            var orgSlug = SanitizeSlug(orgName);
            if (string.IsNullOrWhiteSpace(orgSlug))
            {
                orgSlug = SanitizeSlug(ownerEmail);
                if (!string.IsNullOrWhiteSpace(orgSlug) && !orgSlug.EndsWith("-org"))
                {
                    orgSlug += "-org";
                }
            }

            return orgSlug;
        }

    }
}
