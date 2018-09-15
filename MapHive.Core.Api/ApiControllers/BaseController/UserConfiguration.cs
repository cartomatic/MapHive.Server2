﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using MapHive.Core.Api.UserConfiguration;
using Microsoft.AspNetCore.Http;


namespace MapHive.Core.Api.ApiControllers
{
    public abstract partial class BaseController
    {
        /// <summary>
        /// Returns obtained user configuration
        /// </summary>
        public MapHive.Core.Configuration.UserConfiguration UserConfiguration => UserConfigurationActionFilterAtribute.GetUserConfiguration(Context);

        /// <summary>
        /// returns a user config with no sensitive data
        /// </summary>
        public MapHive.Core.Configuration.UserConfiguration SafeUserConfiguration => UserConfigurationActionFilterAtribute.GetSafeUserConfiguration(Context);
    }
}