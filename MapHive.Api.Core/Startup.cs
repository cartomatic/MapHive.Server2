using MapHive.Core.Api.StartupExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace MapHive.Core.Api
{
    public class Startup
    {
        /// <summary>
        /// Api settings
        /// </summary>
        protected static ApiConfigurationSettings Settings => new ApiConfigurationSettings
        {
            AppShortNames = "core-api,auth-api", //so far aurth api not split into a separate service
            XmlCommentsPath = @"MapHive.Api.Core.xml",
            ApiTitle = "MapHive.Api.Core",
            UseGitVersion = true,
            AllowApiTokenAccess = true,
            UsesIdentityUserManagerUtils = true
        };

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigurMapHiveApiServices(Settings);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.ConfigureMapHiveApi(env, Settings);
        }
    }
}
