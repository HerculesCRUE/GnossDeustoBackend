﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Linked_Data_Server.Models.Services
{
    ///<summary>
    ///Clase para obtener la configuración necesaria
    ///</summary>
    public class ConfigService
    {
        public IConfigurationRoot Configuration { get; set; }
        private string NameTitle { get; set; }
        private string ConstrainedByUrl { get; set; }
        private string SparqlGraph { get; set; }
        private string SparqlEndpoint { get; set; }
        private string SparqlQueryParam { get; set; }
        private string UrlHome { get; set; }
        private string OntologyGraph { get; set; }

        ///<summary>
        ///Obtiene el título:NameTitle del fichero appsettings.json
        ///</summary>
        public string GetNameTitle()
        {
            if (string.IsNullOrEmpty(NameTitle))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("NameTitle"))
                {
                    NameTitle = environmentVariables["NameTitle"] as string;
                }
                else
                {
                    NameTitle = Configuration["NameTitle"];
                }

            }
            return NameTitle;
        }

        ///<summary>
        ///Obtiene el título:NameTitle del fichero appsettings.json
        ///</summary>
        public string GetConstrainedByUrl()
        {
            if (string.IsNullOrEmpty(ConstrainedByUrl))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("ConstrainedByUrl"))
                {
                    ConstrainedByUrl = environmentVariables["ConstrainedByUrl"] as string;
                }
                else
                {
                    ConstrainedByUrl = Configuration["ConstrainedByUrl"];
                }

            }
            return ConstrainedByUrl;
        }

        ///<summary>
        ///Obtiene el gráfo configurado en Sparql:Graph del fichero appsettings.json
        ///</summary>
        public string GetSparqlGraph()
        {
            if (string.IsNullOrEmpty(SparqlGraph))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("Graph"))
                {
                    SparqlGraph = environmentVariables["Graph"] as string;
                }
                else
                {
                    SparqlGraph = Configuration["Sparql:Graph"];
                }

            }
            return SparqlGraph;
        }

        ///<summary>
        ///Obtiene el endpoint configurado en Sparql:Endpoint del fichero appsettings.json
        ///</summary>
        public string GetSparqlEndpoint()
        {
            if (SparqlEndpoint == null)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("Endpoint"))
                {
                    SparqlEndpoint = environmentVariables["Endpoint"] as string;
                }
                else
                {
                    SparqlEndpoint = Configuration["Sparql:Endpoint"];
                }
            }
            return SparqlEndpoint;
        }

        ///<summary>
        ///Obtiene el parametro de query configurado en Sparql:QueryParam del fichero appsettings.json
        ///</summary>
        public string GetSparqlQueryParam()
        {
            if (string.IsNullOrEmpty(SparqlQueryParam))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("QueryParam"))
                {
                    SparqlQueryParam = environmentVariables["QueryParam"] as string;
                }
                else
                {
                    SparqlQueryParam = Configuration["Sparql:QueryParam"];
                }

            }
            return SparqlQueryParam;
        }


        ///<summary>
        ///Obtiene el título:UrlHome del fichero appsettings.json
        ///</summary>
        public string GetUrlHome()
        {
            if (string.IsNullOrEmpty(UrlHome))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("UrlHome"))
                {
                    UrlHome = environmentVariables["UrlHome"] as string;
                }
                else
                {
                    UrlHome = Configuration["UrlHome"];
                }

            }
            return UrlHome;
        }

        ///<summary>
        ///Obtiene el grafo de la ontología:OntologyGraph del fichero appsettings.json
        ///</summary>
        public string GetOntologyGraph()
        {
            if (string.IsNullOrEmpty(OntologyGraph))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("OntologyGraph"))
                {
                    OntologyGraph = environmentVariables["OntologyGraph"] as string;
                }
                else
                {
                    OntologyGraph = Configuration["OntologyGraph"];
                }

            }
            return OntologyGraph;
        }
    }

    
}
