// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para obtener la configuración necesaria para el uso de Sparql
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Api_Unidata.Models.Services
{
    ///<summary>
    ///Clase para obtener la configuración necesaria para el uso de Sparql
    ///</summary>
    public class ConfigSparql
    {
        /// <summary>
        /// Configuración.
        /// </summary>
        public IConfigurationRoot Configuration { get; set; }
        private string GraphUnidata { get; set; }
        private string EndpointUnidata { get; set; }
        private string QueryParam { get; set; }
        
        ///<summary>
        ///Obtiene el gráfo de unidata configurado en GraphUnidata del fichero appsettings.json
        ///</summary>
        public string GetGraphUnidata()
        {
            if (string.IsNullOrEmpty(GraphUnidata))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("GraphUnidata"))
                {
                    GraphUnidata = environmentVariables["GraphUnidata"] as string;
                }
                else
                {
                    GraphUnidata = Configuration["GraphUnidata"];
                }

            }
            return GraphUnidata;
        }

        ///<summary>
        ///Obtiene el endpoint configurado en Sparql:Endpoint del fichero appsettings.json
        ///</summary>
        public string GetEndpointUnidata()
        {
            if (EndpointUnidata == null)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("EndpointUnidata"))
                {
                    EndpointUnidata = environmentVariables["EndpointUnidata"] as string;
                }
                else
                {
                    EndpointUnidata = Configuration["EndpointUnidata"];
                }
            }
            return EndpointUnidata;
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
                    QueryParam = Configuration["QueryParam"];
                }

            }
            return QueryParam;
        }
    }
}
