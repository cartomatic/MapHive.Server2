using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Cartomatic.Utils.Cache;
using MapHive.Api.Core.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using RestSharp;

namespace MapHive.Api.Core.Authorize
{
    public static class TokenAuthorizeMiddlewareExtensions
    {
        /// <summary>
        /// Activates token authorize middleware to handle api token auth properly;
        /// <para/>this middleware should only be activated on demand
        /// </summary>
        /// <param name="app"></param>
        public static void UseTokenAuthorizeMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<TokenAuthorizeMiddleware>();
        }
    }

    public class TokenAuthorizeMiddleware
    {
        protected static ICache<bool> Cache { get; private set; }

        private readonly RequestDelegate _next;

        public TokenAuthorizeMiddleware(RequestDelegate next)
            : this(next, CacheType.InMemory, 5000)
        {
        }

        public TokenAuthorizeMiddleware(RequestDelegate next, CacheType cacheType, int cacheTimeout)
        {
            this._next = next;
            if (Cache == null)
                Cache = CacheFactory.CreateCache<bool>(cacheType, cacheTimeout);
        }


        public async Task Invoke(HttpContext context)
        {
            var authHeader = context.ExtractAuthHeader();


            //if token is present, extract and validate it
            if (authHeader != null && authHeader?.scheme == MapHiveTokenAuthenticationHandler.Scheme)
            {
                var token = authHeader?.parameter;
                if (string.IsNullOrWhiteSpace(token))
                    return;

                await ValidateToken(context, token);
            }

            //let all the middleware do the job
            await _next.Invoke(context);
        }

        /// <summary>
        /// validates a token
        /// </summary>
        /// <param name="context"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected async Task ValidateToken(HttpContext context, string token)
        {
            var cached = Cache.Get(token);
            if (!cached.Valid)
            {
                var tokenOk = await CheckIfTokenAuthorized(token);
                Cache.Set(token, tokenOk);
                cached = Cache.Get(token);
            }
            context.Items[token] = cached.Item;
        }

        /// <summary>
        /// Checks token against a remote core api
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected async Task<bool> CheckIfTokenAuthorized(string token)
        {
            var cfg = Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig();

            var uri = cfg["Endpoints:Core"] + $"tokens/{token}/validation";

            var client = new RestClient(cfg["endpoints:core"] + $"tokens/{token}/validation");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json");

#if DEBUG
            var debug = client.BuildUri(request).AbsoluteUri;
#endif

            var resp = await client.ExecuteTaskAsync(request);
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                return (bool) Newtonsoft.Json.JsonConvert.DeserializeObject(resp.Content, typeof(bool));
            }
            
            //something screwed up with the backend...
            return false;
        }
    }
}
