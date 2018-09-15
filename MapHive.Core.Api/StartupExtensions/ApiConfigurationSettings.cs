using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.Api.UserConfiguration;
using MapHive.Core.DataModel;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.Api
{
    public class ApiConfigurationSettings
    {
        /// <summary>
        /// comma spearated short app names used to as a scope for user cfg;
        /// usually it will be a single app, but in some scenarios more apps are allowed.
        /// basically value of 'testapi' requires the client to have an access to the testapi app,
        /// while 'testapi,testclient' means that a client granted access to any of those is allowed to use the api
        /// </summary>
        public string AppShortNames { get; set; }

        /// <summary>
        /// When not provided, a default instance is used;
        /// by default the attribute is always added to the pipeline
        /// so in order to disable it globally for the api a 'disabled' instance is required or per method usage of 
        /// [OverrideActionFiltersAttribute] (after https://damienbod.com/2014/01/04/web-api-2-using-actionfilterattribute-overrideactionfiltersattribute-and-ioc-injection/)
        /// </summary>
        public UserConfigurationActionFilterAtribute UserConfigurationActionFilterAtribute { get; set; }

        /// <summary>
        /// Whether or not tokens can be used as the api access credentials; this must be explicitly set to true to init proper middleware!
        /// Then it is up to the API to protect its content when tokens cannot be validated...
        /// </summary>
        public bool AllowApiTokenAccess { get; set; }

        
        /// <summary>
        /// API version - for swagger docs
        /// </summary>
        /// <seealso cref="UseGitVersion"/>
        /// <seealso cref="GitPath"/>
        public string ApiVersion { get; set; }

        /// <summary>
        /// API title - for swagger docs
        /// </summary>
        public string ApiTitle { get; set; }

        /// <summary>
        /// Localisation of the XML comments file - for swagger docs
        /// </summary>
        public string XmlCommentsPath { get; set; }

        /// <summary>
        /// whether or not should work out the api version from the git repo
        /// </summary>
        public bool UseGitVersion { get; set; }

        /// <summary>
        /// Path to GIT repo where the API code base is kept. When can be solve to a valid path and is a git repo and UseGitVersion is true,
        /// the api version will be worked out based on the git commits history
        /// <para />
        /// Path can be either relative to build folder or absolute; in most cases can be left out as git can be used in repo subdirs 
        /// </summary>
        /// <seealso cref="UseGitVersion"/>
        public string GitPath { get; set; }

        /// <summary>
        /// A migrator function to be executed automatically when api requires automated db migrations in org context;
        /// When present it will be passed to the <see cref="DbMigratorActionFilterAtribute"/>
        /// </summary>
        public Func<Organization, Task> OrganizationDbMigrator { get; set; }

        /// <summary>
        /// A migrator function for non-org contexted dbs to be executed automatically when automated non-org context db migrations are required;
        /// When present it will be passed to the <see cref="DbMigratorActionFilterAtribute"/>
        /// </summary>
        public Func<DbContext, Task> DbMigrator { get; set; }

        /// <summary>
        /// Whether or not a conofguration call to MapHive.Core.Identity.UserManagerUtils.Configure("MapHiveIdentity") should be performed upon startup;
        /// Set to true if user management APIs is to be used
        /// </summary>
        public bool UsesIdentityUserManagerUtils { get; set; }
    }

}
