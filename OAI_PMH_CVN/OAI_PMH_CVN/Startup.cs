// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto H�rcules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using OAI_PMH_CVN.Models.Services;
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
    

        public void ConfigureServices(IServiceCollection services)
        {               
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            IDictionary environmentVariables = Environment.GetEnvironmentVariables();
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
            services.AddSwaggerGen(c =>
            { 
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OAI-PMH cvn", Version = "v1",Description= "Open Archives Initiative Protocol for Metadata Harvesting" });
                c.IncludeXmlComments(string.Format(@"{0}comments.xml", System.AppDomain.CurrentDomain.BaseDirectory));
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });
                c.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownProxies.Add(IPAddress.Parse("127.0.0.1"));
            });

            services.AddSingleton(typeof(ConfigOAI_PMH_CVN));

        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
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
                        new OpenApiServer { Url = $"/oai-pmh-cvn"},
                        new OpenApiServer { Url = $"/" }
                      });
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "OAI-PMH cvn");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
