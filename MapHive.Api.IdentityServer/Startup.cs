using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Cartomatic.Utils.Data;
using MapHive.Core.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Cartomatic.Utils.Ef;
using IdentityServer4.Models;
using MapHive.Core.Identity.DataModel;
using MapHive.Core.Identity.DAL;
using MapHive.Core.IdentityServer;
using MapHive.Core.IdentityServer.DAL;
using Microsoft.AspNetCore.Identity;

namespace MapHive.Api.IdentityServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MapHiveIdentityDbContext>(
                options =>
                {
                    //this should configure the user manager stuff properly...
                    options.ConfigureProvider(
                        DataSourceProvider.Npgsql,
                        Cartomatic.Utils.Ef.DbContextFactory.GetConnStr("MapHiveIdentity", false) //retrieve connection by key from configured conn strings
                    );
                });

            //asp identity scoped services
            services.ConfigureIdentityScopedServices();
            // Authentification
            services.AddIdentity();

            services.AddIdentityServer()
                .AddSigningCredential(MapHive.Core.IdentityServer.Cryptography.Certificate.Get())
                .AddInMemoryClients(Configuration.GetApiClients())
                .AddInMemoryIdentityResources(Configuration.GetIdentityResources())
                .AddInMemoryApiResources(Configuration.GetApiResources())

                // this adds the config data from DB (clients, resources)
                //.AddConfigurationStore(options =>
                //    options.ConfigureConfiguratonStoreOptions()
                //)

                //.AddInMemoryPersistedGrants()
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options => 
                    options.ConfigureOperationalStoreOptions()
                )
                .AddAspNetIdentity<MapHiveIdentityUser>()
                ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
