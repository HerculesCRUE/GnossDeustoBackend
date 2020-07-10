using ApiCargaWebInterface.Middlewares;
using ApiCargaWebInterface.Models.Services;
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
            services.AddSingleton(typeof(ConfigUrlCronService));
            services.AddScoped<ICallRepositoryConfigService, CallRepositoryConfigApiService>();
            services.AddScoped<ICallUrisFactoryApiService, CallUrisFactoryApiService>(); 
            services.AddScoped<ICallService, CallApiService>();
            services.AddScoped<ICallEtlService, CallEtlService>();
            services.AddScoped<ICallShapeConfigService, CallShapeConfigApiService>(); 
            services.AddScoped(typeof(CallCronApiService));
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
