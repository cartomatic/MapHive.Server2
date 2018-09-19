using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using Microsoft.EntityFrameworkCore;

namespace MapHive.Core.Configuration
{
    /// <summary>
    /// Assmbles some configuration data for an api client - user, token, etc
    /// </summary>
    public class UserConfiguration : IConfiguration
    {
        /// <summary>
        /// A full record for a user, the checkup has been performed. If the checkup was not made in user scope, then the object is null
        /// </summary>
        public MapHiveUser User { get; set; }

#if DEBUG
        /// <summary>
        /// Debug mode user property description
        /// </summary>
        public string UserDescription => User == null ? null :
            @"this property contains a user record for the user that is loading the application. This is a basic record, without any potential extra data retrieved. So lack of roles content, etc. does not mean a user has no roles. 
Note: this is a MapHive config property
Note: this description is only visible in DEBUG mode.";
#endif

        /// <summary>
        /// Whether or not the user for who the config has been output is a real user (real == authenticated with email & pass)
        /// </summary>
        public bool IsUser => User != null;


        /// <summary>
        /// Whether or not this is a token 'user'
        /// </summary>
        public bool IsToken => Token != null;


        /// <summary>
        /// Token retrieved in the config
        /// </summary>
        public Token Token { get; set; }

#if DEBUG
        public string TokenDescription => Token == null ? null : @"this property contains a token object that has been specified by the submitted token Id.
Note: this is a MapHive config property 
Note: this description is only visible in DEBUG mode."; 
#endif

        /// <summary>
        /// A list of organisations a user / token / IP grants the access to.
        /// <para />
        /// It is a user that can exist on scope of many orgs. Tokens are generated per organisation
        /// </summary>
        public List<Organization> Orgs { get; set; }

#if DEBUG
        /// <summary>
        /// Dev mode orgs property description
        /// </summary>
        public string OrgsDescription => Orgs == null ? null :
            @"this property contains a collection of organizations user has access to. this is so it is possible to serve multi-org scenarios. Orgs have apps, modules, datasources, etc properties filled in with a flat structure of objects - this means they contain the objects accessible to an organisation regardless the lvl of nesting it actually is assigned (via package, submodule, etc).
Note: this is a MapHive config property
Note: this description is only visible in DEBUG mode.";
#endif



        /// <summary>
        /// Reads config details for a specified user characteristics - user uuid, token, etc.
        /// </summary>
        /// <param name="dbctx"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public static async Task<UserConfiguration> GetAsync<TDbCtx>(
            TDbCtx dbctx, UserConfigurationQuery q
        )
            where TDbCtx : MapHiveDbContext, new()
        {
            if (q == null)
                return null;

            var dbCtx = new TDbCtx();

            //check if user is known
            MapHiveUser user = null;
            if (q.UserId.HasValue)
                user = await new DataModel.MapHiveUser().ReadAsync(dbCtx, q.UserId.Value);

            Token token = null;
            if (q.TokenId.HasValue)
                token = await new DataModel.Token().ReadAsync(dbCtx, q.TokenId.Value, detached: true);

            
            //Orgs
            IEnumerable<Organization> orgs = null;

            if (user != null)
            {
                //grab user orgs
                //FIXME - need to enable passing all the org assets here to - packages, apps, mods, data sources; see below for details
                orgs = await MapHiveUser.GetUserOrganizationsAsync(dbCtx, q.UserId.Value, (q.AppNames ?? string.Empty).Split(','));
            }


            //Note: not likely to have both - user and token auth but in a case this is true, pull an org for token regardless the fact a user
            //may already have been 'served' above
            if (token != null)

            {
                var tokenOrg = dbCtx.Organizations.FirstOrDefault(org => org.Uuid == token.OrganizationId);

                if (tokenOrg != null)
                {
                    var tmpOrgs = orgs?.ToList() ?? new List<Organization>();

                    //get an app token grants access too
                    var tokenApp = await token.GetApplicationAsync(dbCtx);
                    if (tokenApp != null)
                    {
                        tokenOrg.Applications = new List<Application>
                        {
                            tokenApp
                        };
                    }
                    
                    tmpOrgs.Add(tokenOrg);
                    orgs = tmpOrgs;
                }

                //also check if this is a master token and if the org for which to extract stuff has been provided explicitly!
                if (token.IsMaster && q.OrganizationId.HasValue)
                {
                    //try to grab an org 
                    var explicitlyRequestedTokenOrg = await dbCtx.Organizations.FirstOrDefaultAsync(o => o.Uuid == q.OrganizationId);
                    if (explicitlyRequestedTokenOrg != null)
                    {
                        var tmpOrgs = orgs?.ToList() ?? new List<Organization>();

                        //apps
                        explicitlyRequestedTokenOrg.Applications = (await explicitlyRequestedTokenOrg.GetOrganizationAssetsAsync<Application>(dbCtx))?.assets.ToList();

                        ////mods
                        //explicitlyRequestedTokenOrg.Modules = (await explicitlyRequestedTokenOrg.GetOrganizationAssetsAsync<Module>(dbCtx)).Item1.ToList();

                        ////and data sources
                        //explicitlyRequestedTokenOrg.DataSources = (await explicitlyRequestedTokenOrg.GetOrganizationAssetsAsync<DataSource>(dbCtx)).Item1.ToList();

                        explicitlyRequestedTokenOrg.Owners = (await explicitlyRequestedTokenOrg.GetOwnersAsync(dbCtx)).ToList();
                        explicitlyRequestedTokenOrg.Admins = (await explicitlyRequestedTokenOrg.GetAdminsAsync(dbCtx)).ToList();

                        await explicitlyRequestedTokenOrg.ReadLicenseOptionsAsync(dbCtx);
                        await explicitlyRequestedTokenOrg.LoadDatabasesAsync(dbCtx);

                        tmpOrgs.Add(explicitlyRequestedTokenOrg);
                        orgs = tmpOrgs;
                    }
                }
            }


            //TODO - read asset links, packages and modules, so there is no need in re-reading them all over again! This will require some org assets related api changes
            //FIXME - so basically will need to improve the asset reading ;)

            foreach (var org in orgs ?? new Organization[0])
            {
                //what we're interested in here are:
                //modules and data sources

                //Note: if there is an org present it should have the access to a specified app...
                //unless the app has not been specified... then all orgs a user is assigned to are returned.

                //Note:
                //GetUserOrganizations already reads requested apps to orgs, so there is no need to re-pull them here!
                //Same applies to token - when read it also reads the app
                //org.Applications = (await org.GetLinkedAssetsAsync<Application>(dbCtx)).ToList();


                ////mods
                //org.Modules = (await org.GetOrganizationAssetsAsync<Module>(dbCtx)).Item1.ToList();

                ////and data sources
                //org.DataSources = (await org.GetOrganizationAssetsAsync<DataSource>(dbCtx)).Item1.ToList();


                org.Owners = (await org.GetOwnersAsync(dbCtx)).ToList();
                org.Admins = (await org.GetAdminsAsync(dbCtx)).ToList();

                await org.ReadLicenseOptionsAsync(dbCtx);

                await org.LoadDatabasesAsync(dbCtx);

            }
            //FIXME - same as above - make it possible to pass a list of org assets here
            


            //read user access to orgs / apps
            //this gathers info on user apps access for each org the user is assigned to
            if (user != null)
            {
                //user must be present as without one there is no app access really
                //Note: basically tokens grant direct access rights to apis and apis handle permissions on their own.
                foreach (var o in orgs ?? new Organization[0])
                {
                    foreach (var a in o.Applications)
                    {
                        a.OrgUserAppAccessCredentials = await o.GetUserAppAccessCredentialsAsync(dbCtx, user, a);

                        //reset some properties to avoid circular refs when serializing; and to minimize payload too
                        a.OrgUserAppAccessCredentials.User = null;
                        a.OrgUserAppAccessCredentials.Application = null;
                        a.OrgUserAppAccessCredentials.Organization = null;
                    }
                }
            }


            var userCfg = new UserConfiguration
            {
                User = user,
                Orgs = orgs?.ToList(),
                Token = token
            };

            //make sure do encrypt the dbs - this is sensitive shit after all
            userCfg.EncryptOrgDbs(q);

            return userCfg;
        }

        /// <summary>
        /// 'Reads' configuration by simply testing for a presence of a user configuration retrieved automagically via UserConfigurationFilter attribute
        /// </summary>
        /// <returns></returns>
        public async Task<IDictionary<string, object>> ReadAsync()
        {
            //so warnings dissapear from async method when not using await
            await Task.Delay(0);

            return new Dictionary<string, object>
            {
                { nameof(User), User },
                { nameof(Orgs), Orgs },

#if DEBUG
                { nameof(UserDescription), UserDescription },
                { nameof(OrgsDescription), OrgsDescription },
#endif
            };
        }

        /// <summary>
        /// Decrypts org dbs
        /// </summary>
        /// <param name="q"></param>
        public void DecryptOrgDbs(UserConfigurationQuery q)
        {
            Orgs?.ForEach(org => org.DecryptDatabases(GetEncDecKey(q)));
        }

        /// <summary>
        /// Encrypts org dbs
        /// </summary>
        /// <param name="q"></param>
        public void EncryptOrgDbs(UserConfigurationQuery q)
        {
            Orgs?.ForEach(org => org.EncryptDatabases(GetEncDecKey(q)));
        }

        protected static string GetEncDecKey(UserConfigurationQuery q)
        {
            return $"{q.UserId}_{q.TokenId}";
        }

        /// <summary>
        /// Extracts a particular database configured for an organisation.
        /// if organisation does not have a specified db configured, it creates it off app/web.config as this means a default app/api db should be used
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="dbIdentifier"></param>
        /// <returns></returns>
        public OrganizationDatabase GetOrganizationdatabase(Guid orgId, string dbIdentifier)
        {
            //first grab an org to get the dbs for
            var org = Orgs?.FirstOrDefault(o => o.Uuid == orgId);

            return org?.GetDatabase(dbIdentifier);
        }
    }
}
