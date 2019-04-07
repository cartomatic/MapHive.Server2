using MapHive.Core.Api.UserConfiguration;


namespace MapHive.Core.Api.ApiControllers
{
    public abstract partial class BaseController
    {
        /// <summary>
        /// Returns obtained user configuration
        /// </summary>
        public MapHive.Core.Configuration.UserConfiguration UserConfiguration => UserConfigurationActionFilterAtribute.GetUserConfiguration(HttpContext);

        /// <summary>
        /// returns a user config with no sensitive data
        /// </summary>
        public MapHive.Core.Configuration.UserConfiguration SafeUserConfiguration => UserConfigurationActionFilterAtribute.GetSafeUserConfiguration(HttpContext);
    }
}
