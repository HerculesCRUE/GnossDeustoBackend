// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using GestorDocumentacion.Middlewares;
using GestorDocumentacion.Models;
using GestorDocumentacion.Models.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using Swashbuckle.AspNetCore.Filters;

namespace GestorDocumentacion
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
            IdentityModelEventSource.ShowPII = true; //Add this line
            IDictionary environmentVariables = Environment.GetEnvironmentVariables();
            services.AddControllers();
            string authority = "";
            if (environmentVariables.Contains("Authority"))
            {
                authority = environmentVariables["Authority"] as string;
            }
            else
            {
                authority = Configuration["Authority"];
            }
            string scope = "";
            if (environmentVariables.Contains("Scope"))
            {
                scope = environmentVariables["Scope"] as string;
            }
            else
            {
                scope = Configuration["Scope"];
            }

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
               .AddIdentityServerAuthentication(options =>
               {
                   options.Authority = authority;
                    //options.Authority = "http://herc-as-front-desa.atica.um.es/identityserver";
                    options.RequireHttpsMetadata = false;
                   options.ApiName = scope;
               });
            services.AddAuthorization();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Gestor documentacion", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
               // options.ExampleFilters();
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });
                options.OperationFilter<SecurityRequirementsOperationFilter>();
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
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownProxies.Add(IPAddress.Parse("127.0.0.1"));
            });

            services.AddScoped<IPagesOperationsServices, PagesOperationService>(); 
            services.AddScoped<ITemplatesOperationsServices, TemplatesOperationsService>();
            services.AddScoped<IDocumentsOperationsService, DocumentsOperationsService>();
            services.AddScoped<IFileOperationService, FileOperationsService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
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
                        new OpenApiServer { Url = $"/documentacion"},
                        new OpenApiServer { Url = $"/" }
                      });
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "Gestor Documentacion");
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
