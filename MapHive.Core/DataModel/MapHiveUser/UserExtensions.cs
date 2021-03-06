﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapHive.Core.DataModel
{
    public static class UserExtensions
    {
        /// <summary>
        /// Gets a full user name based on user data, so pretty much name && surname? name surname : surname || name || email
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public static string GetFullUserName(this MapHiveUser u)
        {
            return !string.IsNullOrEmpty(u.Forename) && !string.IsNullOrEmpty(u.Surname)
                ? $"{u.Forename} {u.Surname}"
                : !string.IsNullOrEmpty(u.Forename)
                    ? u.Forename
                    : !string.IsNullOrEmpty(u.Surname)
                        ? u.Surname
                        : u.Email;
        }
    }
}
