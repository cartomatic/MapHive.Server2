using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Cartomatic.Utils.Cache;
using MapHive.Api.Core.ApiControllers;
using MapHive.Api.Core.Authorize;
using MapHive.Api.Core.Extensions;
using MapHive.Core.Configuration;
using MapHive.Core.DataModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MapHive.Api.Core.UserConfiguration
{
    public class UserConfigurationActionFilterAtribute : IAsyncActionFilter
    {
        private readonly bool _disabled;

        protected static ICache<MapHive.Core.Configuration.UserConfiguration> Cache { get; private set; }

        public UserConfigurationActionFilterAtribute()
            : this(CacheType.InMemory, 5000)
        {
        }

        /// <summary>
        /// used to customise user configuration caching options; in most cases defaults will be ok
        /// </summary>
        /// <param name="cacheType"></param>
        /// <param name="cacheTimeout"></param>
        public UserConfigurationActionFilterAtribute(CacheType cacheType, int cacheTimeout)
        {
            if (Cache == null)
                Cache = CacheFactory.CreateCache<MapHive.Core.Configuration.UserConfiguration>(cacheType, cacheTimeout);
        }

        /// <summary>
        /// used to disable filter at an action or controller level;
        /// </summary>
        /// <param name="disable"></param>
        public UserConfigurationActionFilterAtribute(bool disable)
        {
            this._disabled = disable;
        }

        /// <inheritdoc />
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // do something before the action executes
            //var resultContext = await next();
            // do something after the action executes; resultContext.Result will be set

            var allowAnonymous = CheckAnonymous(context);

            if (!allowAnonymous)
            {

                if (!_disabled)
                {
                    //grab cfg
                    await GetUserConfigurationAsync(context);

                    //test if user can use this app
                    if (!AllowViaUserConfig(context, out var failureReason))
                    {
                        //shortcircuit!
                        context.Result = new ObjectResult(
                            new {
                                ErrorMessage = failureReason
                            }
                        )
                        {
                            StatusCode = (int)HttpStatusCode.Forbidden
                        };

                        //after setting a non-null result must return as cannot call next() - it results in:
                        //InvalidOperationException: If an IAsyncActionFilter provides a result value by setting the Result property of ActionExecutingContext to a non-null value, then it cannot call the next filter by invoking ActionExecutionDelegate
                        return;
                    }
                    
                    await next();
                }
            }

            await next();
        }


        /// <summary>
        /// Checks if the current user or token can use the action
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected async Task GetUserConfigurationAsync(ActionExecutingContext actionContext)
        {
            await GetUserConfigurationAsync(actionContext, new UserConfigurationQuery
            {
                UserId = GetUserId(actionContext),
                TokenId = GetTokenId(actionContext),
                Referrer = GetReferrer(actionContext),
                AppNames = GetCommaDelimitedAppNames(),
                Ip = GetIp(actionContext),
                OrganizationId = GetOrganizationId(actionContext)
            });

        }


        /// <summary>
        /// Performs the actual core api inspection to obtain a user config for the callee
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        protected async Task GetUserConfigurationAsync(ActionExecutingContext actionContext, UserConfigurationQuery q)
        {
            var cached = Cache.Get(q.CacheKey);
            if (cached.Valid)
            {
                actionContext.HttpContext.Items[nameof(UserConfiguration)] = cached.Item;
                return;
            }

            //reset
            actionContext.HttpContext.Items[nameof(UserConfiguration)] = null;

            //cached does not exist or has expired, so basically need to perform a call to the core api to obtain the user cfg
            var userCfg =
                await (actionContext.Controller as BaseController)
                    .CoreApiCall<MapHive.Core.Configuration.UserConfiguration>(
                        "configuration/user",
                        queryParams: new Dictionary<string, object>
                        {
                                    {nameof(q.UserId), q.UserId},
                                    {nameof(q.AppNames), q.AppNames},
                                    {nameof(q.Ip), q.Ip},
                                    {nameof(q.Referrer), q.Referrer},
                                    {nameof(q.TokenId), q.TokenId},
                                    {nameof(q.OrganizationId), q.OrganizationId}
                        }
                    );

            //because the db encryption does actually depend on the query sent to the usercfg, need to decrypt it as it is only known here...
            userCfg?.Output?.DecryptOrgDbs(q);

            //cache the data for further usage
            if (userCfg != null)
            {
                Cache.Set(q.CacheKey, userCfg.Output);
            }

            actionContext.HttpContext.Items[nameof(UserConfiguration)] = userCfg?.Output;
        }

        /// <summary>
        /// Checks whether or not action can be accessed anonymously
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        private static bool CheckAnonymous(ActionExecutingContext actionContext)
        {
            return CheckAttributePresence<AllowAnonymousAttribute>(actionContext);
        }

        /// <summary>
        /// Checks for a presence of an attibute on either a controller level or action level
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        private static bool CheckAttributePresence<T>(ActionExecutingContext actionContext)
            where T : class
        {
            //check controller level
            return actionContext.Controller.GetType().GetCustomAttributes(typeof(T)).Any()
                   //AND action level
                   || (actionContext.ActionDescriptor as ControllerActionDescriptor).MethodInfo.GetCustomAttributes(typeof(T)).Any();
        }


        /// <summary>
        /// Gets authenticated user id
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        private Guid? GetUserId(ActionExecutingContext actionContext)
        {
            return Cartomatic.Utils.Identity.GetUserGuid();
        }

        /// <summary>
        /// Extracts token of the authorization header 
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        private Guid? GetTokenId(ActionExecutingContext actionContext)
        {
            var token = default(Guid);


            var auth = actionContext.HttpContext.ExtractAuthHeader();
            if (auth?.scheme == MapHiveTokenAuthenticationHandler.Scheme)
                Guid.TryParse(auth?.parameter, out token);

            return token == default(Guid) ? (Guid?)null : token;
        }

        

        /// <summary>
        /// Returns a referrer
        /// </summary>
        /// <returns></returns>
        private string GetReferrer(ActionExecutingContext actionContext)
        {
            return actionContext.HttpContext.ExtractReferrerHeader()?.Host;
        }

        /// <summary>
        /// Gets the app shortnames api is configured to scope the configuration retrievel to
        /// </summary>
        /// <returns></returns>
        private static string GetCommaDelimitedAppNames()
        {
            return CommonSettings.Get(nameof(ApiConfigurationSettings.AppShortNames));
        }

        /// <summary>
        /// gets an array of app names that this api can be accessed through. 
        /// </summary>
        /// <returns></returns>
        private static string[] GetAppNames()
        {
            return GetCommaDelimitedAppNames().Split(',');
        }

        /// <summary>
        /// Gets a caller IP
        /// </summary>
        /// <returns></returns>
        private string GetIp(ActionExecutingContext actionContext)
        {
            var httpConnectionFeature = actionContext.HttpContext.Features.Get<IHttpConnectionFeature>();
            return httpConnectionFeature?.LocalIpAddress.ToString();
        }

        /// <summary>
        /// Tries to obtain org id used in some scearios with master tokens
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        private static Guid? GetOrganizationId(ActionExecutingContext actionContext)
        {
            if (actionContext.ActionArguments.ContainsKey(OrganizationContextActionFilterAttribute.OrgIdPropertyName))
            {
                return (Guid)actionContext.ActionArguments[OrganizationContextActionFilterAttribute.OrgIdPropertyName];
            }
            return null;
        }

        public static MapHive.Core.Configuration.UserConfiguration GetUserConfiguration(HttpContext context)
        {
            if (context.Items.ContainsKey(nameof(MapHive.Core.Configuration.UserConfiguration)))
                return (MapHive.Core.Configuration.UserConfiguration)context.Items[nameof(MapHive.Core.Configuration.UserConfiguration)];

            return null;
        }

        /// <summary>
        /// Returns full user configuration
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        public static MapHive.Core.Configuration.UserConfiguration GetUserConfiguration(ActionExecutingContext actionContext)
        {
            return GetUserConfiguration(actionContext.HttpContext);
        }

        /// <summary>
        /// Returns a safe configuration - safe means configuration with no sensitive data
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static MapHive.Core.Configuration.UserConfiguration GetSafeUserConfiguration(HttpContext context)
        {
            var userCfg = GetUserConfiguration(context);

            return userCfg == null ? null : new MapHive.Core.Configuration.UserConfiguration
            {
                User = userCfg.User,

                //make sure to wipeout org dbs info!!!
                Orgs = userCfg.Orgs.Select(o => o.AsSafe()).ToList()
            };
        }


        /// <summary>
        /// Returns a safe configuration - safe means configuration with no sensitive data
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        public static MapHive.Core.Configuration.UserConfiguration GetSafeUserConfiguration(ActionExecutingContext actionContext)
        {
            return GetSafeUserConfiguration(actionContext.HttpContext);
        }


        /// <summary>
        /// Whether or now the action is allowed via user config
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected static bool AllowViaUserConfig(ActionExecutingContext actionContext, out string failureReason)
        {
            failureReason = null;

            var userCfg = GetUserConfiguration(actionContext);
            if (userCfg == null)
                return false;

            //Note:
            //possible improvement - work out a way of caching output of this method.
            //the problem though is the very same userCfg may be valid for some requests but invalid for some others
            //all depends on context
            //maybe a combination of url, referrer & authorization header attribute???
            //though with not full url, but rather a path with no attributes, so can efficiently use cache with 
            //apis such as search api - tons of requests in order to get the suggestions

            //if access is via token make sure to check referrers
            if (userCfg.IsToken && !userCfg.Token.CanIgnoreReferrer)
            {
                var referrer = actionContext.HttpContext.ExtractReferrerHeader();
                if (referrer == null)
                {
                    //cannot ignore referrer, need to fail!
                    failureReason = $"Token '{userCfg.Token.Uuid}' requires a referrer but it has not been provided";
                    return false;
                }


                if (userCfg.Token.Referrers == null || !userCfg.Token.Referrers.Contains(referrer.Host))
                {
                    failureReason = $"Token '{userCfg.Token.Uuid}' requires a referrer but the actual one {referrer.Host} is not allowed. ";
#if DEBUG
                    failureReason +=
                        $"Allowed referers for this token are: {string.Join(", ", userCfg?.Token?.Referrers?.ToArray() ?? new[] { "referrers not defined" })}";
#endif
                    return false;
                }
            }


            //org uuid if any - in such case access is via org api: organizations/orgUuid/conntroller
            Organization org = null;
            if (CheckAttributePresence<OrganizationContextActionFilterAttribute>(actionContext))
            {
                //this throws when org uuid is not known, but since the controller is tagged with the org ctx attribute
                //this should not be the case because a non-guid route would not let the request through
                var orgId = OrganizationContextActionFilterAttribute.GetOrganizationId(actionContext.HttpContext);

                //check the org to work out the context for
                org = userCfg.Orgs.FirstOrDefault(o => o.Uuid == orgId);

                //if org does not exist in the collection then neither user nor token grants the access to it 
                if (org == null)
                {
                    failureReason = $"Token / User does not have access to organization '{orgId}'.";
                    return false;
                }
            }


            //if org is not null, then this is an 'organization' controller - the access should be checked for this very org
            //otherwise the api does not know about the organization context and therefore needs to check if any of the orgs
            //have access to the apps served by this controller.

            var appNames = GetAppNames();

            var orgs = org != null ? new[] { org } : (userCfg.Orgs?.ToArray() ?? new Organization[0]);

            //if any app withn an org(s) is the app representing this api, then can use it
            var ok = orgs.Any(o => o?.Applications?.Any(a => appNames.Contains(a.ShortName)) ?? false);
            if (!ok)
            {
                failureReason = $"Org(s): '{string.Join(",", orgs.AsEnumerable())}' have no access to the '{string.Join(", ", appNames)}' app(s) granted.";
            }
            return ok;
        }

        /// <summary>
        /// Gets org from the user cfg
        /// </summary>
        /// <param name="context"></param>
        /// <param name="orgnizationId"></param>
        /// <returns></returns>
        public static Organization GetOrganization(HttpContext context, Guid? orgnizationId)
        {
            return GetUserConfiguration(context)?.Orgs?.FirstOrDefault(o => o.Uuid == orgnizationId);
        }
    }
}
