// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using CronConfigure.Filters;
using CronConfigure.Middlewares;
using CronConfigure.Models;
using CronConfigure.Models.Services;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using Swashbuckle.AspNetCore.Filters;

namespace CronConfigure
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
            services.AddControllers();
            IDictionary environmentVariables = Environment.GetEnvironmentVariables();
            string connectionHangfireString = "";
            if (environmentVariables.Contains("HangfireConnection"))
            {
                connectionHangfireString = environmentVariables["HangfireConnection"] as string;
            }
            else
            {
                connectionHangfireString = Configuration.GetConnectionString("HangfireConnection");
            }
            //Add Hangfire services.
            services.AddHangfire((isp, configuration) => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_110)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(connectionHangfireString, new PostgreSqlStorageOptions()
                {
                    InvisibilityTimeout = TimeSpan.FromDays(1)
                }));


            services.AddHangfireServer();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Configure cron", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
                //options.SchemaFilter<EnumSchemaFilter>();
            });
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownProxies.Add(IPAddress.Parse("127.0.0.1"));
            });

            services.AddEntityFrameworkNpgsql().AddDbContext<HangfireEntityContext>(opt =>
            {
                var builder = new NpgsqlDbContextOptionsBuilder(opt);
                builder.SetPostgresVersion(new Version(9, 6));
                builder.MigrationsHistoryTable("__EFMigrationsHistory", "hangfire");
                opt.UseNpgsql(connectionHangfireString);

            });

            services.AddScoped<ICronApiService, CronApiService>();
            services.AddScoped<IProgramingMethodService, ProgramingMethodsService>();
            services.AddScoped(typeof(CallApiService));
            services.AddSingleton(typeof(ConfigUrlService)); 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use((context, next) =>
            {
                context.Request.PathBase = "/cron-config";
                return next();
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<HangfireEntityContext>();
                serviceScope.ServiceProvider.GetRequiredService<HangfireEntityContext>().Database.Migrate();
            }

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseHangfireServer();
            app.UseHangfireDashboard("/hangfire",new DashboardOptions()
            {
                Authorization = new [] {new HangfireDashboardNoAuthorizationFilter() }
            }
                );
            //backgroundJobs.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) => swaggerDoc.Servers = new List<OpenApiServer>
                      {
                        new OpenApiServer { Url = $"/cron-config"},
                        new OpenApiServer { Url = $"/" }
                      });
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "Uris factory");
                //c.OAuthClientId("client");
                //c.OAuthClientSecret("511536EF-F270-4058-80CA-1C89C192F69A");
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
