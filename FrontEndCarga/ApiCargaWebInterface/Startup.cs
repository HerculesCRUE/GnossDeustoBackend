using ApiCargaWebInterface.Middlewares;
using ApiCargaWebInterface.Models;
using ApiCargaWebInterface.Models.Services;
using ApiCargaWebInterface.Models.Services.VirtualPathProvider;
using AspNetCore.Security.CAS;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using System;
using System.Collections;
using System.Reflection;

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
            bool cargado = false;
            while (!cargado) 
            {
                // services.AddMvcRazorRuntimeCompilation();
                try
                {
                    services.AddRazorPages().AddRazorRuntimeCompilation();
                    services.AddControllersWithViews().AddRazorRuntimeCompilation();
                    services.Configure<MvcRazorRuntimeCompilationOptions>(opts =>
                    {
                        CallApiService serviceApi = new CallApiService();
                        CallTokenService tokenService = new CallTokenService(new ConfigTokenService());
                        ConfigUrlService serviceUrl = new ConfigUrlService();
                        CallApiVirtualPath apiVirtualPath = new CallApiVirtualPath(tokenService, serviceUrl, serviceApi);
                        opts.FileProviders.Add(
                            new ApiFileProvider(apiVirtualPath));
                    });
                    cargado = true;
                } catch (Exception ex)
                {
                    cargado = false;
                }
            }
            services.AddEntityFrameworkNpgsql().AddDbContext<EntityContext>(opt =>
            {
                var builder = new NpgsqlDbContextOptionsBuilder(opt);
                builder.SetPostgresVersion(new Version(9, 6));
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("PostgreConnectionmigration"))
                {
                    opt.UseNpgsql(environmentVariables["PostgreConnectionmigration"] as string);
                }
                else
                {
                    opt.UseNpgsql(Configuration.GetConnectionString("PostgreConnectionmigration"));
                }


            });

            services.AddScoped<DiscoverItemBDService, DiscoverItemBDService>();
            services.AddScoped<ProcessDiscoverStateJobBDService, ProcessDiscoverStateJobBDService>();


            services.AddControllersWithViews(); 
            services.AddSingleton(typeof(ConfigPathLog));
            services.AddSingleton(typeof(ConfigUrlService));
            services.AddSingleton(typeof(ConfigUrlCronService));
            services.AddSingleton(typeof(ConfigUnidataPrefix));
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
            services.AddScoped(typeof(CallApiVirtualPath));
            services.AddScoped(typeof(CallRepositoryJobService));
            services.AddScoped(typeof(ReplaceUsesService));
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
            IDictionary environmentVariables = Environment.GetEnvironmentVariables();
            string proxy = "";
            if (environmentVariables.Contains("Proxy"))
            {
                proxy = environmentVariables["Proxy"] as string;
            }
            else
            {
                proxy = Configuration.GetConnectionString("Proxy");
            }
            app.UsePathBase(proxy);
            app.Use((context, next) =>
            {
                context.Request.PathBase = proxy;
                return next();
            });
            app.UseStaticFiles();
            
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages(
                    
                    );
                //endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

            });
        }
    }
}
