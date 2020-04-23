using ApiCargaWebInterface.Middlewares;
using ApiCargaWebInterface.Models.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddControllersWithViews();
            services.AddSingleton(typeof(ConfigUrlService));
            services.AddScoped<ICallRepositoryConfigService, CallRepositoryConfigApiService>();
            services.AddScoped<ICallService, CallApiService>();
            services.AddScoped<ICallShapeConfigService, CallShapeConfigApiService>();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies")
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = "http://localhost:56306";
                //options.Authority = "http://herc-as-front-desa.atica.um.es/identityserver";
                options.RequireHttpsMetadata = false;
                options.ClientId = "client";
                options.ClientSecret = "secret";
                options.SaveTokens = true;
                options.SignInScheme = "Cookies";
                options.Scope.Add("api1");
            });
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
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
