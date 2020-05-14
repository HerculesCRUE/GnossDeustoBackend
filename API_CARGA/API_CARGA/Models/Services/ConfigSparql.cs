using Microsoft.Extensions.Configuration;
using System;
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
        private string Endpoint { get; set; }
        private string QueryParam { get; set; }
        public string GetGraph()
        {
            if (string.IsNullOrEmpty(Graph))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                Graph = Configuration["Sparql:Graph"];
            }
            return Graph;
        }

        public string GetEndpoint()
        {
            if (string.IsNullOrEmpty(Endpoint))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                Endpoint = Configuration["Sparql:Endpoint"];
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
                QueryParam = Configuration["Sparql:QueryParam"];
            }
            return QueryParam;
        }
    }
}
