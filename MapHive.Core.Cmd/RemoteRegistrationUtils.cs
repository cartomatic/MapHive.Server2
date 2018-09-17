using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using RestSharp;


namespace MapHive.Core.Cmd
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
            await ApiCallAsync<Auth.AuthOutput>(Endpoints["Core"] + "envconfig/registerapps",
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
        protected async Task RegisterEmailTemplatesRemoteAsync(IEnumerable<EmailTemplateLocalization> emailTemplates)
        {
            //authenticate user
            await AuthenticateAsync();

            //and call a remote api...
            await ApiCallAsync<Auth.AuthOutput>(Endpoints["Core"] + "envconfig/registeremailtemplates",
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
            return await ApiCallAsync<bool>(Endpoints["Core"] + "envconfig/appregistered",
                Method.GET,
                queryParams: new Dictionary<string, object>
                {
                    { "appShortName", appShortName }
                }
            );
        }

        

        /// <summary>
        /// Gets an org by slug - calls core api
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        protected async Task<Organization> GetOrgRemoteAsync(string slug)
        {
            //authenticate user
            await AuthenticateAsync();

            //and call a remote api...
            return await ApiCallAsync<Organization>(Endpoints["Core"] + "envconfig/getorg",
                    Method.GET,
                    queryParams: new Dictionary<string, object>
                    {
                        { "identifier", slug }
                    }
                );
        }

        /// <summary>
        /// Gets org by id - calls core api
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        protected async Task<Organization> GetOrgRemoteAsync(Guid orgId)
        {
            //authenticate user
            await AuthenticateAsync();

            //and call a remote api...
            return await ApiCallAsync<Organization>(Endpoints["Core"] + "envconfig/getorg",
                Method.GET,
                queryParams: new Dictionary<string, object>
                {
                    { "identifier", orgId.ToString() }
                }
            );
        }

        /// <summary>
        /// Registers apps with a an org - calls core API
        /// </summary>
        /// <param name="org"></param>
        /// <param name="apps"></param>
        /// <returns></returns>
        protected async Task RegisterAppsWithOrgRemoteAsync(Organization org, IEnumerable<Application> apps)
        {
            await RegisterAppsWithOrgRemoteAsync(org, apps.Select(a => a.ShortName));
        }

        /// <summary>
        /// Registers apps with a an org - calls core API
        /// </summary>
        /// <param name="org"></param>
        /// <param name="appShortNames"></param>
        /// <returns></returns>
        protected async Task RegisterAppsWithOrgRemoteAsync(Organization org, IEnumerable<string> appShortNames)
        {
            //authenticate user
            await AuthenticateAsync();

            //just a put but with url params...
            await ApiCallAsync<object>(Endpoints["Core"] + "envconfig/registerappstoorg",
                Method.PUT,
                queryParams: new Dictionary<string, object>
                {
                    { "identifier", org.Uuid.ToString() },
                    { "appShortNames", string.Join(",", appShortNames) }
                }
            );
        }

        /// <summary>
        /// Creates an organisation
        /// </summary>
        /// <param name="orgName"></param>
        /// <param name="orgDescription"></param>
        /// <param name="orgSlug"></param>
        /// <param name="clean"></param>
        /// <param name="morg"></param>
        /// <param name="email"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        protected async Task CreateOrgRemoteAsync(string orgName, string orgDescription, string orgSlug, bool clean, bool morg, string email, string pass)
        {
            //authenticate user
            await AuthenticateAsync();

            await ApiCallAsync<object>(Endpoints["Core"] + "envconfig/createorg",
                Method.POST,
                queryParams: new Dictionary<string, object>
                {
                    { "orgName", orgName },
                    { "orgDescription", orgDescription },
                    { "orgSlug", orgSlug ?? MapHive.Core.Utils.Slug.GetOrgSlug(orgName, email) },
                    { "clean", clean.ToString() },
                    { "morg", morg.ToString() },
                    { "email", email },
                    { "pass", pass }
                }
            );

            //this drops a user and org, so need to reset auth!
            if (RemoteAdmin["Email"] == email)
            {
                ResetRemoteAuth();
            }
        }

        /// <summary>
        /// Drops an organisation with the remote core api
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        protected async Task DropOrgRemoteAsync(Guid orgId, bool clean)
        {
            //authenticate user
            await AuthenticateAsync();

            await ApiCallAsync<object>(Endpoints["Core"] + "envconfig/droporg",
                Method.DELETE,
                queryParams: new Dictionary<string, object>
                {
                    {
                        "orgId", orgId.ToString()
                    },
                    {
                        "clean", clean
                    }
                }
            );
        }

        /// <summary>
        /// Creates a user via core api
        /// </summary>
        /// <param name="email"></param>
        /// <param name="pass"></param>
        /// <param name="slug"></param>
        /// <param name="destroy"></param>
        /// <returns></returns>
        protected async Task<MapHiveUser> CreateUserRemoteAsync(string email, string pass, string slug, bool destroy)
        {
            if (RemoteAdmin["Email"] == email)
            {
                throw new ArgumentException($"Not possible to recreate a master user '{email}' this way. sorry.");
            }

            await AuthenticateAsync();

            return await ApiCallAsync<MapHiveUser>(Endpoints["Core"] + "envconfig/createuser",
                Method.POST,
                queryParams: new Dictionary<string, object>
                {
                    { "email", email },
                    { "pass", pass },
                    { "slug", slug },
                    { "destroy", destroy.ToString() }
                }
            );
        }

        protected async Task<MapHiveUser> DestroyUserRemoteAsync(string email)
        {
            if (RemoteAdmin["Email"] == email)
            {
                throw new ArgumentException($"Not possible to destroy a master user '{email}' this way. sorry.");
            }

            await AuthenticateAsync();

            return await ApiCallAsync<MapHiveUser>(Endpoints["Core"] + "envconfig/destroyuser",
                Method.DELETE,
                queryParams: new Dictionary<string, object>
                {
                    { "email", email }
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
            await ApiCallAsync<Auth.AuthOutput>(Endpoints["Core"] + "envconfig/registertokens",
                Method.POST,
                data: tokens
            );
        }


        //------------------------

        

        protected static string AccessToken { get; set; }

        protected void ResetRemoteAuth()
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
                var auth = await ApiCallAsync<Auth.AuthOutput>(Endpoints["Auth"] + "letmein",
                    Method.GET,
                    new Dictionary<string, object>
                    {
                        {"email", RemoteAdmin["Email"]},
                        {"pass", RemoteAdmin["Password"]}
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
        protected internal virtual async Task<TOut> ApiCallAsync<TOut>(string url, Method method = Method.GET, Dictionary<string, object> queryParams = null, object data = null)
        {
            var client = new RestClient(url);
            var request = new RestRequest(method);
            request.AddHeader("Content-Type", "application/json");

            //add params if any
            if (queryParams != null && queryParams.Keys.Count > 0)
            {
                foreach (var key in queryParams.Keys)
                {
                    request.AddParameter(key, queryParams[key]?.ToString() ?? string.Empty, ParameterType.QueryString);
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
                Console.WriteLine();
                ConsoleEx.WriteErr($"{new StackTrace().GetFrame(1).GetMethod().Name}; HTTP Code: {resp.StatusCode}; ErrorMsg: {resp.ErrorMessage}");
                Console.WriteLine();
            }

            return output;
        }
    }
}