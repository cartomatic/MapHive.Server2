using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cartomatic.Utils.Data;
using MapHive.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Cartomatic.Utils.Ef;
using Microsoft.AspNetCore.Identity;

namespace MapHive.IdentityServer
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
                .AddSigningCredential(Cryptography.Certificate.Get())
                .AddInMemoryClients(Configuration.GetApiClients())
                .AddInMemoryIdentityResources(Configuration.GetIdentityResources())
                .AddInMemoryApiResources(Configuration.GetApiResources())
                
                .AddInMemoryPersistedGrants()
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
