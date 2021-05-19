using CargaDataSetMurcia.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace CargaDataSetMurcia
{
    class Program
    {
        /// <summary>
        /// Configuración
        /// </summary>
        public static IConfigurationRoot configuration;

        static void Main(string[] args)
        {
            //Cargamos la configuración
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            SparqlConfig sparqlASIO_1 = GetSparqlASIO_1();
            SparqlConfig sparqlASIO_2 = GetSparqlASIO_2();
            string sparqlASIO_Graph = GetSparqlASIO_Graph();
            string urlUrisFactory = GetUrlUrisFactory();

            List<SparqlConfig> sparqlASIO = new List<SparqlConfig>();
            if (sparqlASIO_1 != null && !string.IsNullOrEmpty(sparqlASIO_Graph))
            {
                sparqlASIO.Add(sparqlASIO_1);
            }
            if (sparqlASIO_2 != null && !string.IsNullOrEmpty(sparqlASIO_Graph))
            {
                sparqlASIO.Add(sparqlASIO_2);
            }
            if(sparqlASIO.Count==0)
            {
                throw new Exception("No existen endpointsparql de ASIO configurados");
            }

            if(string.IsNullOrEmpty(urlUrisFactory))
            {
                throw new Exception("No está configurada la Url de UrisFactory");
            }
            if (string.IsNullOrEmpty(sparqlASIO_Graph))
            {
                throw new Exception("No está configurado el grafo de ASIO");
            }

            CargaRDF.GenerarRDF(urlUrisFactory);
            CargaRDF.PublicarRDF(urlUrisFactory, sparqlASIO, sparqlASIO_Graph);

        }


        public static SparqlConfig GetSparqlASIO_1()
        {
            SparqlConfig config = new SparqlConfig();
            config.endpoint = configuration["SparqlASIO_1:Endpoint"];
            config.username = configuration["SparqlASIO_1:Username"];
            config.pass = configuration["SparqlASIO_1:Password"];
            if (!string.IsNullOrEmpty(config.endpoint))
            {
                return config;
            }
            return null;
        }

        public static SparqlConfig GetSparqlASIO_2()
        {
            SparqlConfig config = new SparqlConfig();
            config.endpoint = configuration["SparqlASIO_2:Endpoint"];
            config.username = configuration["SparqlASIO_2:Username"];
            config.pass = configuration["SparqlASIO_2:Password"];
            if (!string.IsNullOrEmpty(config.endpoint))
            {
                return config;
            }
            return null;
        }

        public static string GetSparqlASIO_Graph()
        {
            return configuration["SparqlASIO_Graph"];
        }
        public static string GetUrlUrisFactory()
        {
            return configuration["UrlUrisFactory"];
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Build configuration
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            // Add access to generic IConfigurationRoot
            serviceCollection.AddSingleton<IConfigurationRoot>(configuration);
        }
    }
}
