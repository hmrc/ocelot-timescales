using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Timescales.Controllers.Helpers;
using Timescales.Controllers.Helpers.Interfaces;
using Timescales.Models;

namespace Timescales
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
       
            services.AddDbContext<Context>(options =>
                options
                    .UseNpgsql(Configuration
                        .GetConnectionString("DefaultConnection") +
                            Environment.GetEnvironmentVariable("Connection", EnvironmentVariableTarget.Machine)));

            services.AddAuthentication(HttpSysDefaults.AuthenticationScheme);   
            services.AddMvc();

            services.AddScoped<IAuditHandler, AuditHandler>();
            services.AddScoped<IPublishHandler, PublishHandler>();
            services.AddScoped<ILegacyPublishHandler, LegacyPublishHandler>();
            services.Configure<IISOptions>(c =>
            {
                c.ForwardClientCertificate = true;
                c.AutomaticAuthentication = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<Context>();

                context.Database.EnsureCreated();
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
