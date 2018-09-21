using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapHive.Core.DataModel;

namespace MapHive.Core.Defaults
{
    public class Applications
    {
        /// <summary>
        /// Returns a list of the default applications that should be registered to each organization
        /// </summary>
        /// <returns></returns>
        public static List<Application> GetDefaultOrgApps()
        {
            //simply add all the common apps
            return GetApplications().Where(a => a.IsCommon).ToList(); 
        }

        /// <summary>
        /// Returns all the default applications declared in the mh core
        /// </summary>
        /// <returns></returns>
        public static List<Application> GetApplications()
        {
            return new List<Application>
            {
                //Note:
                //All the apps marked as common are linked to an organization upon org account creation
                //this is the desired scenario

                //core apis
                //----------------------------------------------------------------------------
                new Application
                {
                    Uuid = Guid.Parse("1d2add57-4ab3-4da5-b745-519507b355d6"),
                    ShortName = "core-api",
                    Name = "Core API",
                    Description = "Core API @ MapHive",
                    Urls = "https://core-api.maphive.local/|https://core-api.maphive.net/",
                    IsCommon = true,
                    RequiresAuth = true,
                    IsApi = true
                },
                new Application
                {
                    Uuid = Guid.Parse("cb354aa5-074b-45a7-adba-4717b76c394a"),
                    ShortName = "auth-api",
                    Name = "Auth API",
                    Description = "Auth API @ MapHive",
                    Urls = "https://auth-api.maphive.local/|https://auth-api.maphive.net/|https://core-api.maphive.local/auth/|https://core-api.maphive.net/auth/",
                    IsCommon = true,
                    RequiresAuth = true,
                    IsApi = true
                },

                //default apps
                //----------------------------------------------------------------------------

                //if ever used this is gonna be the 'umbrella' app that loads other apps inside itself
                //more likely scenario is to use regular site to site reloads
                new Application
                {
                    Uuid = Guid.Parse("5f541902-4f42-4a58-8dee-523ea02cd1fd"),
                    ShortName = "hive",
                    Name = "The Hive",
                    Description = "Hive @ MapHive",
                    Urls = "https://maphive.local/|https://maphive.net/|https://hive.maphive.local/|https://hive.maphive.net/",
                    IsCommon = true,
                    IsHive = true
                },

                //home app when there is no org context
                new Application
                {
                    Uuid = Guid.Parse("eea081b5-6a3d-4a11-87e8-55dbe042322c"),
                    ShortName = "home",
                    Name = "Home",
                    IsHome = true, //home app - when no user / org context known, this is what should load
                    IsCommon = true,
                    Urls = "https://home.maphive.local/|https://home.maphive.net/"
                },

                //dashboard app when there is org context, but no app specified
                new Application
                {
                    Uuid = Guid.Parse("fe6801c4-c9cb-4b86-9416-a143b355deab"),
                    ShortName = "dashboard",
                    Name = "Dashboard",
                    IsDefault = true, //dashboard - this app should be loaded when user / org context is known
                    IsCommon = true,
                    Urls = "https://dashboard.maphive.local/|https://dashboard.maphive.net/",
                    RequiresAuth = true
                },

                //one of the default app examples
                new Application
                {
                    Uuid = Guid.Parse("27321a8a-aa7d-47fd-a539-761b248ef248"),
                    ShortName = "hgis",
                    Name = "HGIS v2",
                    Description = "Cartomatic\'s HGIS",
                    Urls = "https://hgis.maphive.local/|https://hgis.maphive.net/",
                    IsCommon = true
                },

                //site admin app 
                new Application
                {
                    Uuid = Guid.Parse("1e025446-1a25-4639-a302-9ce0e2017a59"),
                    //no short name, so can test uuid in the url part!
                    Name = "MapHive SiteAdmin",
                    ShortName = "masterofpuppets",

                    IsCommon = false,
                    Description = "MapHive platform Admin app",
                    Urls = "https://masterofpuppets.maphive.local/|https://masterofpuppets.maphive.net/",
                    RequiresAuth = true
                },

            };
        }
    }
}
