using CargaDataSetMurcia.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using VDS.RDF;

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
            SparqlConfig sparqlUnidata_1 = GetSparqlUnidata_1();
            SparqlConfig sparqlUnidata_2 = GetSparqlUnidata_2();
            string sparqlASIO_Graph = GetSparqlASIO_Graph();
            string sparqlUnidata_Graph = GetSparqlUnidata_Graph();
            string sparqlASIO_Domain = GetSparqlASIO_Domain();
            string sparqlUnidata_Domain = GetSparqlUnidata_Domain();
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
            if (sparqlASIO.Count == 0)
            {
                throw new Exception("No existen endpointsparql de ASIO configurados");
            }

            List<SparqlConfig> sparqlUnidata = new List<SparqlConfig>();
            if (sparqlUnidata_1 != null && !string.IsNullOrEmpty(sparqlUnidata_Graph))
            {
                sparqlUnidata.Add(sparqlUnidata_1);
            }
            if (sparqlUnidata_2 != null && !string.IsNullOrEmpty(sparqlUnidata_Graph))
            {
                sparqlUnidata.Add(sparqlUnidata_2);
            }
            if (sparqlUnidata.Count > 0)
            {
                if (string.IsNullOrEmpty(sparqlUnidata_Graph))
                {
                    throw new Exception("No está configurado el grafo de Unidata");
                }
                if (string.IsNullOrEmpty(sparqlUnidata_Domain))
                {
                    throw new Exception("No está configurada el dominio de Unidata");
                }
            }

            if (string.IsNullOrEmpty(urlUrisFactory))
            {
                throw new Exception("No está configurada la Url de UrisFactory");
            }
            if (string.IsNullOrEmpty(sparqlASIO_Graph))
            {
                throw new Exception("No está configurado el grafo de ASIO");
            }
            if (string.IsNullOrEmpty(sparqlASIO_Domain))
            {
                throw new Exception("No está configurada el dominio de ASIO");
            }
            

            CargaRDF.GenerarRDF(urlUrisFactory);
            CargaRDF.PublicarRDF(urlUrisFactory, sparqlASIO, sparqlASIO_Graph, sparqlASIO_Domain, sparqlUnidata, sparqlUnidata_Graph, sparqlUnidata_Domain);

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

        public static SparqlConfig GetSparqlUnidata_1()
        {
            SparqlConfig config = new SparqlConfig();
            config.endpoint = configuration["SparqlUnidata_1:Endpoint"];
            config.username = configuration["SparqlUnidata_1:Username"];
            config.pass = configuration["SparqlUnidata_1:Password"];
            if (!string.IsNullOrEmpty(config.endpoint))
            {
                return config;
            }
            return null;
        }

        public static SparqlConfig GetSparqlUnidata_2()
        {
            SparqlConfig config = new SparqlConfig();
            config.endpoint = configuration["SparqlUnidata_2:Endpoint"];
            config.username = configuration["SparqlUnidata_2:Username"];
            config.pass = configuration["SparqlUnidata_2:Password"];
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

        public static string GetSparqlUnidata_Graph()
        {
            return configuration["SparqlUnidata_Graph"];
        }

        public static string GetSparqlASIO_Domain()
        {
            return configuration["SparqlASIO_Domain"];
        }

        public static string GetSparqlUnidata_Domain()
        {
            return configuration["SparqlUnidata_Domain"];
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
