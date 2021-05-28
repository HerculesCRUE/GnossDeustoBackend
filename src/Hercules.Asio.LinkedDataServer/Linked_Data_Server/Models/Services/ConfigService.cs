using Microsoft.Extensions.Configuration;
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
        private string SparqlEndpoint1 { get; set; }
        private string XAppServer1 { get; set; }
        private string SparqlEndpoint2 { get; set; }
        private string XAppServer2 { get; set; }
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

        public string GetSparqlEndpoint1()
        {
            if (SparqlEndpoint1 == null)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("Endpoint1"))
                {
                    SparqlEndpoint1 = environmentVariables["Endpoint1"] as string;
                }
                else
                {
                    SparqlEndpoint1 = Configuration["Sparql1:Endpoint"];
                }
            }
            return SparqlEndpoint1;
        }

        public string GetSparqlEndpoint2()
        {
            if (SparqlEndpoint2 == null)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("Endpoint2"))
                {
                    SparqlEndpoint2 = environmentVariables["Endpoint2"] as string;
                }
                else
                {
                    SparqlEndpoint2 = Configuration["Sparql2:Endpoint"];
                }
            }
            return SparqlEndpoint2;
        }

        public string GetXAppServer1()
        {
            if (XAppServer1 == null)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("XAppServer1"))
                {
                    XAppServer1 = environmentVariables["XAppServer1"] as string;
                }
                else
                {
                    XAppServer1 = Configuration["Sparql1:XAppServer"];
                }
            }
            return XAppServer1;
        }


        public string GetXAppServer2()
        {
            if (XAppServer2 == null)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("XAppServer2"))
                {
                    XAppServer2 = environmentVariables["XAppServer2"] as string;
                }
                else
                {
                    XAppServer2 = Configuration["Sparql2:XAppServer"];
                }
            }
            return XAppServer2;
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
