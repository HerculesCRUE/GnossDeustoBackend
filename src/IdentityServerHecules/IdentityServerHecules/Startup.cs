// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Stores;
using IdentityServerHecules.Persist;
using System.Collections;

namespace IdentityServerHecules
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
            /////////FUnciona
            //services.AddIdentityServer()
            //    .AddDeveloperSigningCredential()
            //    .AddInMemoryApiResources(Config.GetApiResources())
            //    .AddInMemoryClients(Config.GetClients());
            ///////////////
            ///////Prueba

            IDictionary environmentVariables = Environment.GetEnvironmentVariables();
            string connectionString = "";
            if (environmentVariables.Contains("PostgreConnection"))
            {
                connectionString = environmentVariables["PostgreConnection"] as string;
            }
            else
            {
                connectionString = Configuration.GetConnectionString("PostgreConnection");
            }
            string issuerUri = "";
            if (environmentVariables.Contains("IssuerUri"))
            {
                issuerUri = environmentVariables["IssuerUri"] as string;
            }
            else
            {
                issuerUri = Configuration.GetConnectionString("IssuerUri");
            }
            //const string connectionString = @"Username =herculesdb;Password=NUuPIsrUV4x3o6sZEqE8;Host=155.54.239.203;Port=5432;Database=herculesdb;Pooling=true";
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            // configure identity server with in-memory stores, keys, clients and scopes
            services.AddIdentityServer(x=>
            {
                x.IssuerUri = issuerUri;
            })
                .AddDeveloperSigningCredential()
                //.AddInMemoryApiResources(Config.GetApiResources())
                //.AddInMemoryClients(Config.GetClients())
                .AddPersistedGrantStore<PersistedGrantStore>()
                // this adds the config data from DB (clients, resources)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseNpgsql(connectionString, opt =>
                        {
                            opt.SetPostgresVersion(new Version(9, 6));
                            opt.MigrationsAssembly(migrationsAssembly);
                        }
                        );
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseNpgsql(connectionString, opt =>
                        {
                            opt.SetPostgresVersion(new Version(9, 6));
                            opt.MigrationsAssembly(migrationsAssembly);
                        }
                        );

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 604800;
                });

            services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();
            services.AddTransient<IPersistedGrantServiceP, PersistedGrantServiceP>();
            ////////
            ///
            var sp = services.BuildServiceProvider();

            // Resolve the services from the service provider
            var context = sp.GetService<ConfigurationDbContext>();
            InitializeDatabase(context);
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void InitializeDatabase(ConfigurationDbContext context)
        {

            context.Database.Migrate();
            if (context.Clients.Any())
            {
                foreach (var client in context.Clients)
                {
                    context.Clients.Remove(client);
                }
                context.SaveChanges();
            }

            if (!context.Clients.Any())
            {
                foreach (var client in Config.GetClients())
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }

            //if (!context.IdentityResources.Any())
            //{
            //    foreach (var resource in Config.GetIdentityResources())
            //    {
            //        context.IdentityResources.Add(resource.ToEntity());
            //    }
            //    context.SaveChanges();
            //}
            if (context.ApiResources.Any())
            {
                foreach (var resource in context.ApiResources)
                {
                    context.ApiResources.Remove(resource);
                }
                context.SaveChanges();
            }
            if (!context.ApiResources.Any())
            {
                foreach (var resource in Config.GetApiResources())
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

        }
    }
}
