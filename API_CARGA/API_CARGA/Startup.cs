// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;
using API_CARGA.Middlewares;
using API_CARGA.ModelExamples;
using API_CARGA.Models;
using API_CARGA.Models.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Microsoft.OpenApi.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using Swashbuckle.AspNetCore.Filters;


namespace PRH
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        //<summary>
        // This method gets called by the runtime. Use this method to add services to the container.
        //</summary>
        public void ConfigureServices(IServiceCollection services)
        {              
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                //options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API de carga", Version = "v1",Description= "API de carga" });
                c.IncludeXmlComments(string.Format(@"{0}comments.xml", System.AppDomain.CurrentDomain.BaseDirectory));
                c.ExampleFilters();
            });
            services.AddSwaggerExamplesFromAssemblyOf<ConfigRepositoriesResponse>();
            services.AddSwaggerExamplesFromAssemblyOf<ConfigRepositoryResponse>();
            services.AddSwaggerExamplesFromAssemblyOf<AddRepositoryErrorResponse>();
            services.AddSwaggerExamplesFromAssemblyOf<ModifyRepositoryErrorResponse>();

            services.AddSwaggerExamplesFromAssemblyOf<ShapeConfigResponse>();
            services.AddSwaggerExamplesFromAssemblyOf<ShapesConfigsResponse>();
            services.AddSwaggerExamplesFromAssemblyOf<AddShapeConfigErrorResponse>();
            services.AddSwaggerExamplesFromAssemblyOf<ModifyShapeConfigErrorResponse>();

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownProxies.Add(IPAddress.Parse("127.0.0.1"));
            });

            
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
            services.AddSingleton(typeof(ConfigUrlService));
            services.AddSingleton(typeof(ConfigSparql));
            services.AddScoped(typeof(OaiPublishRDFService));
            //services.AddSingleton<IRepositoriesConfigService, RepositoriesConfigMockService>();
            services.AddScoped<IRepositoriesConfigService, RepositoriesConfigBDService>();
            //services.AddSingleton<IShapesConfigService, ShapesConfigMockService>();
            services.AddScoped<IShapesConfigService, ShapesConfigBDService>();
            services.AddScoped<ICallNeedPublishData, CallApiNeedInfoPublisData>();
            //services.AddSingleton<ISyncConfigService, SyncConfigMockService>();

        }

        //<summary>
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //</summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthorization();
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) => swaggerDoc.Servers = new List<OpenApiServer>
                      {
                        new OpenApiServer { Url = $"/carga"},
                        new OpenApiServer { Url = $"/" }
                      });
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "API de carga");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
