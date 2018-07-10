using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MapHive.Api.Core.UserConfiguration;
using MapHive.Api.Core.Authorize;
using MapHive.Core;
using MapHive.Core.DataModel;
using MapHive.IdentityServer.SerializableConfig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

namespace MapHive.Api.Core.StartupExtensions
{
    public static class StartupExtensions
    {
        public static void ConfigurMapHiveApiServices(this IServiceCollection services, ApiConfigurationSettings settings)
        {

            //TODO - plug in idsrv!!!!


            //FIXME - token authorize - injection!!!
            //https://stackoverflow.com/questions/41972518/how-to-add-global-authorizefilter-or-authorizeattribute-in-asp-net-core

            services
                .AddMvc(opts =>
                {
                    //mark all the controllers with authorize attribute!
                    //in order to remove it one needs to explicitly revoke it via [AllowAnonymous]
                    opts.Filters.Add(settings?.AllowApiTokenAccess == true
                        ? typeof(TokenAuthorizeAttribute)
                        : typeof(AuthorizeAttribute)
                    );

                    //use a default or customised user cfg filter 
                    opts.Filters.Add(
                        settings?.UserConfigurationActionFilterAtribute
                        ?? new UserConfigurationActionFilterAtribute()
                    );

                    //when auto migrators are specified wire them up via MapHive.API.Core.DbMigratorActionFilterAtribute
                    if (settings?.DbMigrator != null || settings?.OrganizationDbMigrator != null)
                    {
                        opts.Filters.Add(new DbMigratorActionFilterAtribute(
                                settings?.OrganizationDbMigrator, settings?.DbMigrator
                            )
                        );
                    }

                })
                .AddJsonOptions(opts =>
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

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = bearerCfg.Authority;
                    options.RequireHttpsMetadata = true;
                    options.ApiName = bearerCfg.ApiName;
                    options.ApiSecret = bearerCfg.ApiSecret;
                });

            services.Configure<IISOptions>(opts =>
            {
                opts.ForwardClientCertificate = false;
            });


            //auto swagger documentation
            if (!string.IsNullOrEmpty(settings.XmlCommentsPath))
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info
                    {
                        Title = settings?.ApiTitle ?? "unknown-api",
                        Version = settings.UseGitVersion ? Cartomatic.Utils.Git.GetRepoVersion() : settings.ApiVersion
                    });
                    c.IncludeXmlComments(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, settings.XmlCommentsPath));

                    // UseFullTypeNameInSchemaIds replacement for .NET Core
                    c.CustomSchemaIds(x => x.FullName);
                });
            }

            services.AddCors();
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
            ObjectTypeExtensions.GetRegisteredTypes();

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
                
            //TODO - extract off API config
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            //enforce auth
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}"
                );
            });
        }
    }
}
