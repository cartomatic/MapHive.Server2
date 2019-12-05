using System.Threading.Tasks;
using Cartomatic.Utils.Email;
using MapHive.Core.Api.StartupExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MapHive.Core.Api
{

    public class Startup
    {
        /// <summary>
        /// Api settings
        /// </summary>
        protected static ApiConfigurationSettings Settings => new ApiConfigurationSettings
        {
            AppShortNames = "core-api,auth-api", //so far auth api not split into a separate service
            XmlCommentsPath = @"MapHive.Api.Core.xml",
            ApiTitle = "MapHive.Api.Core",
            UseGitVersion = true,
            AllowApiTokenAccess = true,
            UsesIdentityUserManagerUtils = true,
            EnableCompression = true,

            //this will ensure the api migrates DB upon a very first call after deploy.
            //this way, will not have to run cmd db setup when deploying to server
            DbMigrator = MapHive.Core.DAL.DbMigrator.MapHiveMetaDbMigrator
        };

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigurMapHiveApiServices(Settings);

            services.AddSingleton<IEmailSender, Cartomatic.Utils.Email.MailKit.EmailSender>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();

            app.ConfigureMapHiveApi(env, Settings);
        }
    }
}
