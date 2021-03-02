// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para obtener la configuración necesaria para el uso de Sparql
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections;
using System.IO;

namespace API_CARGA.Models.Services
{
    ///<summary>
    ///Clase para obtener la configuración necesaria para el uso de Sparql
    ///</summary>
    public class ConfigSparql
    {
        public IConfigurationRoot Configuration { get; set; }
        private string Graph { get; set; }
        public string Endpoint { get; set; }
        private string QueryParam { get; set; }
        private string GraphRoh { get; set; }

        private IConfiguration _configuration { get; set; }

        public ConfigSparql(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        ///<summary>
        ///Obtiene el gráfo configurado en Sparql:Graph del fichero appsettings.json
        ///</summary>
        public string GetGraph()
        {
            if (string.IsNullOrEmpty(Graph))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("Graph"))
                {
                    Graph = environmentVariables["Graph"] as string;
                }
                else
                {
                    Graph = _configuration["Sparql:Graph"];
                }
                
            }
            return Graph;
        }

        ///<summary>
        ///Obtiene el gráfo configurado en Sparql:GraphRoh del fichero appsettings.json
        ///</summary>
        public string GetGraphRoh()
        {
            if (string.IsNullOrEmpty(GraphRoh))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("GraphRoh"))
                {
                    GraphRoh = environmentVariables["GraphRoh"] as string;
                }
                else
                {
                    GraphRoh = _configuration["Sparql:GraphRoh"];
                }

            }
            return GraphRoh;
        }

        ///<summary>
        ///Obtiene el endpoint configurado en Sparql:Endpoint del fichero appsettings.json
        ///</summary>
        public string GetEndpoint()
        {
            if (Endpoint==null)
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("Endpoint"))
                {
                    Endpoint = environmentVariables["Endpoint"] as string;
                }
                else
                {
                    Endpoint = _configuration["Sparql:Endpoint"];
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
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("QueryParam"))
                {
                    QueryParam = environmentVariables["QueryParam"] as string;
                }
                else
                {
                    QueryParam = _configuration["Sparql:QueryParam"];
                }

            }
            return QueryParam;
        }
    }
}
