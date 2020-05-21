using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API_CARGA.Models.Services
{
    public class ConfigSparql
    {
        public IConfigurationRoot Configuration { get; set; }
        private string Graph { get; set; }
        public string Endpoint { get; set; }
        private string QueryParam { get; set; }
        public string GetGraph()
        {
            if (string.IsNullOrEmpty(Graph))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("Graph"))
                {
                    Graph = environmentVariables["Graph"] as string;
                }
                else
                {
                    Graph = Configuration["Sparql:Graph"];
                }
                
            }
            return Graph;
        }

        public string GetGraphUnidata()
        {
            if (string.IsNullOrEmpty(Graph))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("GraphUnidata"))
                {
                    Graph = environmentVariables["GraphUnidata"] as string;
                }
                else
                {
                    Graph = Configuration["Sparql:GraphUnidata"];
                }

            }
            return Graph;
        }

        public string GetEndpoint()
        {
            if (Endpoint==null)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("Endpoint"))
                {
                    Endpoint = environmentVariables["Endpoint"] as string;
                }
                else
                {
                    Endpoint = Configuration["Sparql:Endpoint"];
                }
            }
            return Endpoint;
        }

        public string GetQueryParam()
        {
            if (string.IsNullOrEmpty(QueryParam))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("QueryParam"))
                {
                    QueryParam = environmentVariables["QueryParam"] as string;
                }
                else
                {
                    QueryParam = Configuration["Sparql:QueryParam"];
                }

            }
            return QueryParam;
        }
    }
}
