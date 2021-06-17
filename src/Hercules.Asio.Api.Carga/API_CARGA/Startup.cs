// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto H�rcules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using API_CARGA.Middlewares;
using API_CARGA.ModelExamples;
using API_CARGA.Models;
using API_CARGA.Models.Entities;
using API_CARGA.Models.Services;
using Hercules.Asio.Api.Carga.Models.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private IConfiguration _configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        //<summary>
        // This method gets called by the runtime. Use this method to add services to the container.
        //</summary>
        public void ConfigureServices(IServiceCollection services)
        {
            IDictionary environmentVariables = Environment.GetEnvironmentVariables();
            string authority = "";
            if (environmentVariables.Contains("Authority"))
            {
                authority = environmentVariables["Authority"] as string;
            }
            else
            {
                authority = _configuration["Authority"];
            }
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.IgnoreNullValues=true;
            });

            if (_env.IsDevelopment())
            {
                services.AddSingleton<IAuthorizationHandler, AllowAnonymous>();
            }
            else
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = authority;
                    options.RequireHttpsMetadata = false;
                    options.ApiName = "apiCarga";
                });
                services.AddAuthorization();
            }

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API de carga", Version = "v1",Description= "API de carga" });
                c.IncludeXmlComments(string.Format(@"{0}comments.xml", System.AppDomain.CurrentDomain.BaseDirectory));
                c.ExampleFilters();
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
                    opt.UseNpgsql(_configuration.GetConnectionString("PostgreConnectionmigration"));
                }
                

            });

            
            if (environmentVariables.Contains("uriRabbitMq"))
            {
                string uriRabbitMq = environmentVariables["uriRabbitMq"] as string;
                string usernameRabbitMq = environmentVariables["usernameRabbitMq"] as string;
                string passwordRabbitMq = environmentVariables["passwordRabbitMq"] as string;
                string virtualhostRabbitMq = environmentVariables["virtualhostRabbitMq"] as string;
                string hostnameRabbitMq = environmentVariables["hostnameRabbitMq"] as string;
                services.Configure<RabbitMQInfo>(options => 
                {
                    options.HostNameRabbitMq = hostnameRabbitMq;
                    options.PasswordRabbitMq = passwordRabbitMq;
                    options.UriRabbitMq = uriRabbitMq;
                    options.UsernameRabbitMq = usernameRabbitMq;
                    options.VirtualHostRabbitMq = virtualhostRabbitMq;
                }
                ) ;
            }
            else
            {
                services.Configure<RabbitMQInfo>(_configuration.GetSection("RabbitMQ"));
            }
            
            services.AddSingleton(typeof(ConfigUrlService));
            services.AddSingleton(typeof(ConfigSparql));
            services.AddScoped(typeof(OaiPublishRDFService));
            services.AddScoped<IRepositoriesConfigService, RepositoriesConfigBDService>();
            services.AddScoped<IDiscoverItemService, DiscoverItemBDService>();
            services.AddSingleton<IRabbitMQService, RabbitMQService>();
            services.AddScoped<IShapesConfigService, ShapesConfigBDService>();
            services.AddScoped<ICallNeedPublishData, CallApiNeedInfoPublisData>();
            services.AddScoped<ICallService, CallApiService>();
            services.AddScoped(typeof(CallTokenService));
            services.AddScoped(typeof(CallApiService));
            services.AddScoped(typeof(CallOAIPMH));
            services.AddScoped(typeof(CallConversor));            
        }

        //<summary>
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //</summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, EntityContext entityContext)
        {
            entityContext.Migrate();
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
    public class AllowAnonymous : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            foreach (IAuthorizationRequirement requirement in context.PendingRequirements.ToList())
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
