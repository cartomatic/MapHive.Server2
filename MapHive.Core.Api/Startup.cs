using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MapHive.Api.Core;
using MapHive.Api.Core.StartupExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace MapHive.Core.API
{
    public class Startup
    {
        /// <summary>
        /// Api settings
        /// </summary>
        protected static ApiConfigurationSettings Settings => new ApiConfigurationSettings
        {
            AppShortNames = "hive,home,dashboard,hgis1,masterofpuppets",
            XmlCommentsPath = @"bin\Debug\netcoreapp2.1\MapHive.Core.Api.xml",
            ApiTitle = "MapHive.Core.Api",
            UseGitVersion = true,
            AllowApiTokenAccess = true
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
