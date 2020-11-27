using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chino.IdentityServer.Data;
using Chino.IdentityServer.Models.User;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Chino.IdentityServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Chino.IdentityServer", Version = "v1" });
            });


            services.AddChinoDatabase<ChinoApplicationDbContext>(this.Configuration);

            services.AddIdentity<ChinoUser, IdentityRole>()
                .AddEntityFrameworkStores<ChinoApplicationDbContext>()
                .AddDefaultTokenProviders();

            #region IdentityServer
            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                options.EmitStaticAudienceClaim = true;
            })
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.AddIdentityServerConfigurationDatabase(this.Configuration);
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.AddIdentityServerOperationalDatabase(this.Configuration);

                    options.EnableTokenCleanup = true;
                })
                .AddAspNetIdentity<ChinoUser>();

            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }

            services.AddAuthentication();

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chino.IdentityServer v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}