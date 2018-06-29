using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.CmdPrompt.Core;
using Cartomatic.Utils.Data;
using MapHive.Core;
using MapHive.Core.DataModel;
using MapHive.Core.DAL;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using RestSharp;
using Token = MapHive.Core.Data.Token;


namespace MapHive.Cmd.Core
{

    public partial class CommandHandler
    {
        //NOTE: set of methods to call remote core, locale, auth apis in order to register api / app specific stuff

        /// <summary>
        /// Registers api apps against a remote core api endpoint
        /// </summary>
        /// <param name="apps"></param>
        /// <returns></returns>
        protected async Task RegisterAppsRemoteAsync(params Application[] apps)
        {
            //authenticate user
            await AuthenticateAsync();

            //and call a remote api...
            await ApiCallAsync<Auth.AuthOutput>(ApiRegCfg["CoreEndPoint"] + "envconfig/registerapps",
                Method.POST,
                data: apps
            );
        }
        protected async Task RegisterAppsRemoteAsync(List<Application> apps)
        {
            await RegisterAppsRemoteAsync(apps.ToArray());
        }

        /// <summary>
        /// Registers email templates against a remote locale endpoint
        /// </summary>
        /// <param name="emailTemplates"></param>
        /// <returns></returns>
        protected async Task RegisterEmailTemplatesRemoteAsync(List<EmailTemplate> emailTemplates)
        {
            //authenticate user
            await AuthenticateAsync();

            //and call a remote api...
            await ApiCallAsync<Auth.AuthOutput>(ApiRegCfg["LocaleEndPoint"] + "envconfig/registeremailtemplates",
                Method.POST,
                data: emailTemplates
            );
        }

        /// <summary>
        /// Whether or not an app is registered in the core env
        /// </summary>
        /// <param name="appShortName"></param>
        /// <returns></returns>
        protected async Task<bool> IsAppRegisteredRemoteAsync(string appShortName)
        {
            //authenticate user
            await AuthenticateAsync();

            //and call a remote api...
            return await ApiCallAsync<bool>(ApiRegCfg["CoreEndPoint"] + "envconfig/appregistered",
                Method.GET,
                queryParams: new Dictionary<string, string>
                {
                    { "appShortName", appShortName }
                }
            );
        }

        

        /// <summary>
        /// Gets an org by name - calls core api
        /// </summary>
        /// <param name="orgName"></param>
        /// <returns></returns>
        protected async Task<Organization> GetOrgRemoteAsync(string orgName)
        {
            //authenticate user
            await AuthenticateAsync();

            //and call a remote api...
            return await ApiCallAsync<Organization>(ApiRegCfg["CoreEndPoint"] + "envconfig/getorg",
                    Method.GET,
                    queryParams: new Dictionary<string, string>
                    {
                        { "identifier", orgName }
                    }
                );
        }

        /// <summary>
        /// Registers apps with a master org - calls core API
        /// </summary>
        /// <param name="org"></param>
        /// <param name="apps"></param>
        /// <returns></returns>
        protected async Task RegisterAppsWithMasterOrgRemoteAsync(Organization org, IEnumerable<Application> apps)
        {
            //authenticate user
            await AuthenticateAsync();

            //just a put but with url params...
            await ApiCallAsync<object>(ApiRegCfg["CoreEndPoint"] + "envconfig/registerappstoorg",
                Method.PUT,
                queryParams: new Dictionary<string, string>
                {
                    { "orgName", org.DisplayName },
                    { "appShortNames", string.Join(",", apps.Select(a=>a.ShortName)) }
                }
            );
        }

        /// <summary>
        /// Creates an organisation
        /// </summary>
        /// <param name="orgName"></param>
        /// <param name="orgDescription"></param>
        /// <param name="clean"></param>
        /// <param name="morg"></param>
        /// <param name="email"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        protected async Task CreateOrgRemoteAsync(string orgName, string orgDescription, bool clean, bool morg, string email, string pass)
        {
            //authenticate user
            await AuthenticateAsync();

            await ApiCallAsync<object>(ApiRegCfg["CoreEndPoint"] + "envconfig/createorg",
                Method.POST,
                queryParams: new Dictionary<string, string>
                {
                    { "orgName", orgName },
                    { "orgDescription", orgDescription },
                    { "clean", clean.ToString() },
                    { "morg", morg.ToString() },
                    { "email", email },
                    { "pass", pass }
                }
            );

            //this drops a user and org, so need to reset auth!
            if (ApiRegCfg["User"] == email)
            {
                ResetAuth();
            }
        }

        /// <summary>
        /// Drops an organisation with the remote core api
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        protected async Task DropOrgRemoteAsync(Guid orgId)
        {
            //authenticate user
            await AuthenticateAsync();

            await ApiCallAsync<object>(ApiRegCfg["CoreEndPoint"] + "envconfig/droporg",
                Method.DELETE,
                queryParams: new Dictionary<string, string>
                {
                    { "orgId", orgId.ToString() }
                }
            );
        }

        /// <summary>
        /// Creates a user via core api
        /// </summary>
        /// <param name="email"></param>
        /// <param name="pass"></param>
        /// <param name="destroy"></param>
        /// <returns></returns>
        protected async Task<MapHiveUser> CreateUserRemoteAsync(string email, string pass, string slug, bool destroy)
        {
            ExtractConfig();

            if (ApiRegCfg["User"] == email)
            {
                throw new ArgumentException($"Not possible to recreate a master user '{email}' this way. sorry.");
            }

            await AuthenticateAsync();

            return await ApiCallAsync<MapHiveUser>(ApiRegCfg["CoreEndPoint"] + "envconfig/createuser",
                Method.POST,
                queryParams: new Dictionary<string, string>
                {
                    { "email", email },
                    { "pass", pass },
                    { "slug", slug },
                    { "destroy", destroy.ToString() }
                }
            );
        }


        /// <summary>
        /// Registers tokens with a core api
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        protected async Task RegisterTokensRemoteAsync(List<Token> tokens)
        {
            //authenticate user
            await AuthenticateAsync();

            //and call a remote api...
            await ApiCallAsync<Auth.AuthOutput>(ApiRegCfg["CoreEndPoint"] + "envconfig/registertokens",
                Method.POST,
                data: tokens
            );
        }


        //------------------------

        private Dictionary<string, string> ApiRegCfg { get; set; }
        protected void ExtractConfig()
        {
            ApiRegCfg = JsonConvert.DeserializeObject<Dictionary<string, string>>(ConfigurationManager.AppSettings["ApiRegistrationConfig"]);
        }

        protected static string AccessToken { get; set; }

        private void ResetAuth()
        {
            AccessToken = null;
        }

        /// <summary>
        /// Gets access token
        /// </summary>
        /// <param name="email"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        private async Task AuthenticateAsync()
        {
            if (string.IsNullOrEmpty(AccessToken))
            {
                ExtractConfig();

                var auth = await ApiCallAsync<Auth.AuthOutput>(ApiRegCfg["AuthEndPoint"] + "login",
                    Method.GET,
                    new Dictionary<string, string>
                    {
                        {"email", ApiRegCfg["User"]},
                        {"pass", ApiRegCfg["Password"]}
                    });

                AccessToken = auth.AccessToken;
            }
        }
        

        /// <summary>
        /// Calls a specified REST backend at the specified url and using the specified method and params and parses the response to the appropriate output type
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="queryParams"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected internal virtual async Task<TOut> ApiCallAsync<TOut>(string url, Method method = Method.GET, Dictionary<string, string> queryParams = null, object data = null)
        {
            var client = new RestClient(url);
            var request = new RestRequest(method);
            request.AddHeader("Content-Type", "application/json");

            //add params if any
            if (queryParams != null && queryParams.Keys.Count > 0)
            {
                foreach (var key in queryParams.Keys)
                {
                    request.AddParameter(key, queryParams[key], ParameterType.QueryString);
                }
            }

            if ((method == Method.POST || method == Method.PUT) && data != null)
            {
                //use custom serializer on output! This is important as the newtonsoft's json stuff is used for the object serialization!
                request.RequestFormat = DataFormat.Json;
                request.JsonSerializer = new Cartomatic.Utils.RestSharpSerializers.NewtonSoftJsonSerializer();
                request.AddBody(data);
            }

            if (!string.IsNullOrEmpty(AccessToken))
            {
                request.AddHeader("Authorization", $"Bearer {AccessToken}");
            }


            //because of some reason RestSharp is bitching around when deserializing the arr / list output...
            //using Newtonsoft.Json instead

            var output = default(TOut);

#if DEBUG
            var debugUrl = client.BuildUri(request);
#endif

            var resp = await client.ExecuteTaskAsync(request);
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                output = (TOut)Newtonsoft.Json.JsonConvert.DeserializeObject(resp.Content, typeof(TOut));
            }
            else
            {
               throw new Exception($"{resp.StatusCode}: {resp.ErrorMessage}");
            }

            return output;
        }
    }
}