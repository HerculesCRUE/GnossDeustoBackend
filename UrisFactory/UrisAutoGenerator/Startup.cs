using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using UrisFactory.Filters;
using UrisFactory.Middlewares;
using UrisFactory.ModelExamples;
using UrisFactory.Models.Services;

namespace UrisFactory
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Uris factory", Version = "v1"});
                c.OperationFilter<AddParametersFilter>();
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.ExampleFilters();
            });
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownProxies.Add(IPAddress.Parse("127.0.0.1"));
            });
            services.AddSwaggerExamplesFromAssemblyOf<UrisFactoryResponse>();
            services.AddSwaggerExamplesFromAssemblyOf<UriStructureInfoRequest>();
            services.AddSwaggerExamplesFromAssemblyOf<UrisFactoryErrorReponse>();
            services.AddSwaggerExamplesFromAssemblyOf<UriStructureGeneralExample>();
            services.AddSwaggerExamplesFromAssemblyOf<ReplaceSchemaResponse>();
            services.AddSwaggerExamplesFromAssemblyOf<ReplaceShemaErrorResponse>();
            services.AddSwaggerExamplesFromAssemblyOf<DeleteUriStructureErrorResponse>();
            services.AddSwaggerExamplesFromAssemblyOf<AddUriStructureResponse>();
            services.AddSwaggerExamplesFromAssemblyOf<AddUriStructureErrorResponse>();


            services.AddSingleton(typeof(ConfigJsonHandler));
            services.AddScoped<ISchemaConfigOperations, SchemaConfigFileOperations>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            app.UseMiddleware(typeof(LoadConfigJsonMiddleware));
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthorization();
            
            app.UseSwagger(c=>
                {
                    c.PreSerializeFilters.Add((swaggerDoc, httpReq) => swaggerDoc.Servers = new List<OpenApiServer>
                      {
                        new OpenApiServer { Url = $"/uris"},
                        new OpenApiServer { Url = $"/" }
                      });
                });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "Uris factory");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
