using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;
using API_CARGA.Middlewares;
using API_CARGA.ModelExamples;
using API_CARGA.Models.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Microsoft.OpenApi.Models;
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

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownProxies.Add(IPAddress.Parse("127.0.0.1"));
            });


            services.AddSingleton<IRepositoriesConfigService, RepositoriesConfigMockService>();
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
