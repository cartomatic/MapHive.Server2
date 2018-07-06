﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;


namespace MapHive.API.Core
{
    public abstract partial class BaseApiController
    {
        /// <summary>
        /// Returns obtained user configuration
        /// </summary>
        public MapHive.Core.Configuration.UserConfiguration UserConfiguration => UserConfigurationActionFilterAtribute.GetUserConfiguration(Request);

        /// <summary>
        /// returns a user config with no sensitive data
        /// </summary>
        public MapHive.Core.Configuration.UserConfiguration SafeUserConfiguration => UserConfigurationActionFilterAtribute.GetSafeUserConfiguration(Request);
    }
}
