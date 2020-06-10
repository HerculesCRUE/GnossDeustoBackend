// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
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

            //IdentityServer
            //services.AddAuthentication("Bearer")
            //.AddJwtBearer("Bearer", options =>
            //{
            //    options.Authority = "http://localhost:5000";
            //    options.RequireHttpsMetadata = false;

            //    options.Audience = "api_uris";
            //});

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Uris factory", Version = "v1"});
                options.OperationFilter<AddParametersFilter>();
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
                options.ExampleFilters();
                // Define the OAuth2.0 scheme that's in use (i.e. Implicit Flow)
            //    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            //    {
            //        Type = SecuritySchemeType.OAuth2,
            //        Flows = new OpenApiOAuthFlows
            //        {
            //            Implicit = new OpenApiOAuthFlow
            //            {
            //                AuthorizationUrl = new System.Uri("http://localhost:5000/auth-server/connect/authorize", UriKind.Absolute),
            //                Scopes = new Dictionary<string, string>
            //                {
            //                    { "api_uris", "Uris factory" }
            //                }
            //            }
            //        }
            //    });
            //    options.AddSecurityRequirement(new OpenApiSecurityRequirement
            //    {
            //        {
            //            new OpenApiSecurityScheme
            //            {
            //                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
            //            },
            //            new[] { "api_uris", "Uris factory" }
            //        }
            //    });
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
                c.OAuthClientId("client");
                c.OAuthClientSecret("511536EF-F270-4058-80CA-1C89C192F69A");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
