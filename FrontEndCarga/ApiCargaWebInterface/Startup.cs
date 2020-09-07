using ApiCargaWebInterface.Middlewares;
using ApiCargaWebInterface.Models.Services;
using AspNetCore.Security.CAS;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections;

namespace ApiCargaWebInterface
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
            IDictionary environmentVariables = Environment.GetEnvironmentVariables();
            string casBaseUrl = "";
            if (environmentVariables.Contains("CasBaseUrl"))
            {
                casBaseUrl = environmentVariables["CasBaseUrl"] as string;
            }
            else
            {
                casBaseUrl = Configuration["CasBaseUrl"];
            }

            string serviceHost = "";
            if (environmentVariables.Contains("ServiceHost"))
            {
                serviceHost = environmentVariables["ServiceHost"] as string;
            }
            else
            {
                serviceHost = Configuration["ServiceHost"];
            }
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/login");
                    options.AccessDeniedPath = new PathString("/access-denied");
                })
                .AddCAS(options =>
                {
                    options.CasServerUrlBase = casBaseUrl;  // Set in `appsettings.json` file.
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.ServiceHost = serviceHost;
                });
            services.AddControllersWithViews(); 
            services.AddSingleton(typeof(ConfigPathLog));
            services.AddSingleton(typeof(ConfigUrlService));
            services.AddSingleton(typeof(ConfigUrlCronService));
            services.AddScoped<ICallRepositoryConfigService, CallRepositoryConfigApiService>();
            services.AddScoped<ICallUrisFactoryApiService, CallUrisFactoryApiService>(); 
            services.AddScoped<ICallService, CallApiService>();
            services.AddScoped<ICallEtlService, CallEtlService>();
            services.AddScoped<ICallShapeConfigService, CallShapeConfigApiService>(); 
            services.AddScoped(typeof(CallCronApiService));
            services.AddScoped(typeof(CheckSystemService));
            services.AddScoped(typeof(CallCronService));
            services.AddScoped(typeof(ConfigTokenService));
            services.AddScoped(typeof(CallTokenService)); 
            services.AddScoped(typeof(CallRepositoryJobService)); 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error/exception");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            app.UseStatusCodePagesWithReExecute("/error/{0}");
            app.UseAuthentication();
            app.UseHttpsRedirection();

            app.UsePathBase("/carga-web");
            app.Use((context, next) =>
            {
                context.Request.PathBase = "/carga-web";
                return next();
            });
            app.UseStaticFiles();
            
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=RepositoryConfig}/{action=Index}/{id?}");
            });
        }
    }
}
