// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para obtener la configuración necesaria para el uso de Sparql
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace API_DISCOVER.Models.Services
{
    [ExcludeFromCodeCoverage]
    ///<summary>
    ///Clase para obtener la configuración necesaria para el uso de Sparql
    ///</summary>
    public class ConfigSparql
    {
        public IConfigurationRoot Configuration { get; set; }
        private string Graph { get; set; }
        private string Endpoint { get; set; }
        private string QueryParam { get; set; }
        private string Username { get; set; }
        private string Password { get; set; }


        private string UnidataGraph { get; set; }
        private string UnidataEndpoint { get; set; }
        private string UnidataQueryParam { get; set; }
        private string UnidataUsername { get; set; }
        private string UnidataPassword { get; set; }

        ///<summary>
        ///Obtiene el gráfo configurado en Sparql:Graph del fichero appsettings.json
        ///</summary>
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

        ///<summary>
        ///Obtiene el endpoint configurado en Sparql:Endpoint del fichero appsettings.json
        ///</summary>
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

        ///<summary>
        ///Obtiene el parametro de query configurado en Sparql:QueryParam del fichero appsettings.json
        ///</summary>
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

        ///<summary>
        ///Obtiene el parametro de usuario en Sparql:Username del fichero appsettings.json
        ///</summary>
        public string GetUsername()
        {
            if (string.IsNullOrEmpty(Username))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("Username"))
                {
                    Username = environmentVariables["Username"] as string;
                }
                else
                {
                    Username = Configuration["Sparql:Username"];
                }

            }
            return Username;
        }

        ///<summary>
        ///Obtiene el parametro de password en Sparql:Password del fichero appsettings.json
        ///</summary>
        public string GetPassword()
        {
            if (string.IsNullOrEmpty(Password))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("Password"))
                {
                    Password = environmentVariables["Password"] as string;
                }
                else
                {
                    Password = Configuration["Sparql:Password"];
                }

            }
            return Password;
        }

        ///<summary>
        ///Obtiene el gráfo configurado en SparqlUnidata:Graph del fichero appsettings.json
        ///</summary>
        public string GetUnidataGraph()
        {
            if (string.IsNullOrEmpty(UnidataGraph))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("UnidataGraph"))
                {
                    UnidataGraph = environmentVariables["UnidataGraph"] as string;
                }
                else
                {
                    UnidataGraph = Configuration["SparqlUnidata:Graph"];
                }

            }
            return UnidataGraph;
        }

        ///<summary>
        ///Obtiene el endpoint configurado en SparqlUnidata:Endpoint del fichero appsettings.json
        ///</summary>
        public string GetUnidataEndpoint()
        {
            if (UnidataEndpoint == null)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("UnidataEndpoint"))
                {
                    UnidataEndpoint = environmentVariables["UnidataEndpoint"] as string;
                }
                else
                {
                    UnidataEndpoint = Configuration["SparqlUnidata:Endpoint"];
                }
            }
            return UnidataEndpoint;
        }

        ///<summary>
        ///Obtiene el parametro de query configurado en SparqlUnidata:QueryParam del fichero appsettings.json
        ///</summary>
        public string GetUnidataQueryParam()
        {
            if (string.IsNullOrEmpty(UnidataQueryParam))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("UnidataQueryParam"))
                {
                    UnidataQueryParam = environmentVariables["UnidataQueryParam"] as string;
                }
                else
                {
                    UnidataQueryParam = Configuration["SparqlUnidata:QueryParam"];
                }

            }
            return UnidataQueryParam;
        }

        ///<summary>
        ///Obtiene el parametro de usuario en SparqlUnidata:Username del fichero appsettings.json
        ///</summary>
        public string GetUnidataUsername()
        {
            if (string.IsNullOrEmpty(UnidataUsername))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("UnidataUsername"))
                {
                    UnidataUsername = environmentVariables["UnidataUsername"] as string;
                }
                else
                {
                    UnidataUsername = Configuration["SparqlUnidata:Username"];
                }

            }
            return UnidataUsername;
        }

        ///<summary>
        ///Obtiene el parametro de password en SparqlUnidata:Password del fichero appsettings.json
        ///</summary>
        public string GetUnidataPassword()
        {
            if (string.IsNullOrEmpty(UnidataPassword))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("UnidataPassword"))
                {
                    UnidataPassword = environmentVariables["UnidataPassword"] as string;
                }
                else
                {
                    UnidataPassword = Configuration["SparqlUnidata:Password"];
                }

            }
            return UnidataPassword;
        }
    }
}
