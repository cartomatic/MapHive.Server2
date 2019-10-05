using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cartomatic.Utils;
using MapHive.Core.Api.UserConfiguration;
using MapHive.Core.Api.Authorize;
using MapHive.Core.Api.Filters;
using MapHive.Core;
using MapHive.Core.Api.Compression;
using MapHive.Core.DataModel;
using MapHive.Core.IdentityServer.SerializableConfig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

namespace MapHive.Core.Api.StartupExtensions
{
    public static class StartupExtensions
    {
        public static void ConfigurMapHiveApiServices(this IServiceCollection services, ApiConfigurationSettings settings)
        {
            services
                .AddMvc(opts =>
                {
                    var authSchemes = new List<string>
                    {
                        "Bearer"
                    };

                    if (settings.AllowApiTokenAccess)
                    {
                        authSchemes.Add(MapHiveTokenAuthenticationHandler.Scheme);
                    }

                    //by default enforce auth on all controllers!                    
                    var globalAuthorizePolicy = new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(authSchemes.ToArray())
                        .RequireAuthenticatedUser()
                        .Build();
                    opts.Filters.Add(new AuthorizeFilter(globalAuthorizePolicy));

                    //so can keep on using Cartomatic.Utils.Identity...
                    //a bit not in line with aspnet core though, but these utils are used in many places and passing identity explicitly doesn't seem to be a viable option. at least at this stage...
                    opts.Filters.Add(new OldSchoolUserImpersonationViaClaimsPrincipalAttribute());

                    //user cfg depends on the org context so this attribute must kick in earlier
                    //this way org context is exrtracted before dependant code kicks in
                    opts.Filters.Add(new OrganizationContextActionFilterAttribute());

                    //use a default or customised user cfg filter 
                    opts.Filters.Add(
                        settings?.UserConfigurationActionFilterAtribute
                        ?? new UserConfigurationActionFilterAtribute()
                    );


                    if (settings.EnableCompression)
                    {
                        //allow reading gzip/json
                        opts.InputFormatters.Add(new GZippedJsonInputFormatter());
                    }

                    //when auto migrators are specified wire them up via MapHive.Core.Api.DbMigratorActionFilterAtribute
                    if (settings?.DbMigrator != null || settings?.OrganizationDbMigrator != null)
                    {
                        opts.Filters.Add(new DbMigratorActionFilterAtribute(
                                settings?.OrganizationDbMigrator, settings?.DbMigrator
                            )
                        );
                    }

                })
                .AddJsonOptions(opts => { })

                //in 3.x using newtonsoft so far, so need to opt in!
                .AddNewtonsoftJson(opts =>
                    
                {
                    opts.SerializerSettings.Formatting = Formatting.None;
#if DEBUG
                    opts.SerializerSettings.Formatting = Formatting.Indented;
#endif
                    //make the json props be camel case!
                    opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

                    //ignore nulls, no point in outputting them!
                    opts.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

                    //ensure dates are handled as ISO 8601
                    opts.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                }
            );

            var bearerCfg = IdSrvTokenBearerOpts.InitDefault();


            //by default do Bearer auth. it fallsback for token auth when required
            var authBuilder = services.AddAuthentication("Bearer");

            //always plug in idsrv auth!
            authBuilder.AddIdentityServerAuthentication(options =>
            {
                options.Authority = bearerCfg.Authority;
                options.RequireHttpsMetadata = true;
                options.ApiName = bearerCfg.ApiName;
                options.ApiSecret = bearerCfg.ApiSecret;
                options.TokenRetriever = request =>
                {
                    //first try to obtain the bearer token off the header
                    var authToken =
                        IdentityModel.AspNetCore.OAuth2Introspection.TokenRetrieval.FromAuthorizationHeader()(request);

                    //if no token in header, try to grab it off the url
                    if (string.IsNullOrEmpty(authToken))
                        authToken =
                            IdentityModel.AspNetCore.OAuth2Introspection.TokenRetrieval.FromQueryString()(request);

                    return authToken;
                };
            });

            //should investigate tokens???
            if (settings?.AllowApiTokenAccess == true)
            {
                //services.Add<IAuthorizationHandler, MapHiveTokenAuthenticationHandler>();

                authBuilder.AddScheme<MapHiveTokenAuthenticationOptions, MapHiveTokenAuthenticationHandler>(
                    MapHiveTokenAuthenticationHandler.Scheme,
                    opts => { }
                );
            }
                
            

            services.Configure<IISOptions>(opts =>
            {
                opts.ForwardClientCertificate = false;
            });


            //auto swagger documentation
            if (!string.IsNullOrEmpty(settings.XmlCommentsPath))
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc(settings.UseGitVersion ? Cartomatic.Utils.Git.GetRepoVersion() : settings.ApiVersion, new OpenApiInfo
                    {
                        Title = settings?.ApiTitle ?? "unknown-api",
                        Version = settings.UseGitVersion ? Cartomatic.Utils.Git.GetRepoVersion() : settings.ApiVersion
                    });
                    c.IncludeXmlComments(
                        settings.XmlCommentsPath.IsAbsolute()
                            ? settings.XmlCommentsPath
                            : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, settings.XmlCommentsPath)
                    );

                    // UseFullTypeNameInSchemaIds replacement for .NET Core
                    c.CustomSchemaIds(x => x.FullName);
                });
            }

            if (settings.EnableCompression)
            {
                services.Configure<GzipCompressionProviderOptions>(cfgOpts => cfgOpts.Level = System.IO.Compression.CompressionLevel.Optimal);

                services.AddResponseCompression(opts =>
                {
                    //custom brotli compression provider
                    //MapHive.Core.Api.Compression.BrotliCompressionProvider - used prior to 3.x when brotli was not provided by the framework
                    opts.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();

                    //add customized mime
                    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(settings.CompressedMimeTypes ?? new string[0]);
                    opts.EnableForHttps = true;
                });

                

            }

            if (settings.UsesIdentityUserManagerUtils)
            {
                MapHive.Core.Identity.UserManagerUtils.Configure(services, "MapHiveIdentity");
            }

            //customize cors settings - example using cors policy
            //services.AddCors(opts =>
            //{
            //    opts.AddPolicy("MapHiveCors", builder => builder.CustomizeCors());
            //});
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="settings"></param>
        public static void ConfigureMapHiveApi(this IApplicationBuilder app, IHostingEnvironment env, ApiConfigurationSettings settings)
        {
            //this makes sure all the types inheriting from object base auto register themselves
            BaseObjectTypeIdentifierExtensions.AutoRegisterBaseTypes();

            //store api short name
            CommonSettings.Set(nameof(settings.AppShortNames), settings?.AppShortNames);


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //so can return the api docs too!
            app.UseStaticFiles();

            //use the middleware for api token related preflight
            if (settings?.AllowApiTokenAccess == true)
                app.UseTokenAuthorizeMiddleware();



            //auto swagger documentation
            if (!string.IsNullOrEmpty(settings?.XmlCommentsPath))
            {
                app.UseSwagger();

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint(
                        $"/swagger/{(settings.UseGitVersion ? Cartomatic.Utils.Git.GetRepoVersion() : settings.ApiVersion)}/swagger.json",
                        settings?.ApiTitle ?? "unknown-api"
                    );
                });
            }

            //customize cors
            app.UseCors(builder => builder.CustomizeCors());
            //example using a cors policy added in the service confi section
            //app.UseCors("MapHiveCors");

            //enforce auth
            app.UseAuthentication();

            //this should give us the ability to check the request lng in a case it has not been explicitly provided by a callee
            app.UseRequestLocalization();

            //enable outgoing compression when required
            if (settings.EnableCompression)
            {
                app.UseResponseCompression();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller}/{action}");
            });
        }

        /// <summary>
        /// Customizes cors cfg
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static CorsPolicyBuilder CustomizeCors(this CorsPolicyBuilder builder)
        {
            var cfg = Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig();

            var origins = cfg.GetSection("CorsCfg:Origins").Get<string[]>() ?? new string[0];
            var headers = cfg.GetSection("CorsCfg:Headers").Get<string[]>() ?? new string[0];
            var methods = cfg.GetSection("CorsCfg:Methods").Get<string[]>() ?? new string[0];

            //all origins allowed
            if (origins.Any(o => o == "*"))
            {
                builder.AllowAnyOrigin();
            }
            else if(origins.Any())
            {
                if (origins.Any(o => o.IndexOf("*") > -1))
                    builder.SetIsOriginAllowedToAllowWildcardSubdomains();

                builder.WithOrigins(origins);
            }

            if (headers.Any(h => h == "*"))
            {
                builder.AllowAnyHeader();
            }
            else if (headers.Any())
            {
                builder.WithHeaders(headers);
            }

            if (methods.Any(m => m == "*"))
            {
                builder.AllowAnyMethod();
            }
            else if(methods.Any())
            {
                builder.WithMethods(methods);
            }

            return builder;
        }
    }
}
