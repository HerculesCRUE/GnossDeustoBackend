using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API_DISCOVER.Models;
using API_DISCOVER.Models.Entities;
using API_DISCOVER.Models.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace API_DISCOVER
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                    IConfigurationRoot Configuration = builder.Build();

                    IDictionary environmentVariables = Environment.GetEnvironmentVariables();

                    //SQL
                    {
                        var optionsBuilder = new DbContextOptionsBuilder<EntityContext>();
                        var builderNpgsql = new NpgsqlDbContextOptionsBuilder(optionsBuilder);
                        builderNpgsql.SetPostgresVersion(new Version(9, 6));
                        
                        if (environmentVariables.Contains("PostgreConnectionmigration"))
                        {
                            optionsBuilder.UseNpgsql(environmentVariables["PostgreConnectionmigration"] as string);
                        }
                        else
                        {
                            optionsBuilder.UseNpgsql(Configuration.GetConnectionString("PostgreConnectionmigration"));
                        }
                        services.AddScoped<EntityContext>(s => new EntityContext(optionsBuilder.Options));
                        services.AddScoped<DiscoverItemBDService, DiscoverItemBDService>();
                    }

                    //Rabbit
                    {
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
                            );
                        }
                        else
                        {
                            services.Configure<RabbitMQInfo>(Configuration.GetSection("RabbitMQ"));
                        }
                        services.AddSingleton<RabbitMQService, RabbitMQService>();
                    }
                    services.AddHostedService<Worker>();
                });
    }
}
