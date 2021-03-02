// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Servicio para obtener las variables de configuración de urls
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.IO;

namespace ApiCargaWebInterface.Models.Services
{
    /// <summary>
    /// Servicio para obtener las variables de configuración de urls
    /// </summary>
    public class ConfigUrlService
    {
        public string Url { get; set; }
        public string UrlUris { get; set; }
        public string UrlDocumentacion { get; set; }
        public string Proxy { get; set; }
        public string SaprqlEndpoint { get; set; }
        public string SparqlQuery { get; set; }

        private IConfiguration _configuration { get; set; }

        public ConfigUrlService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Obtiene la url del api de carga que ha sido configurada
        /// </summary>
        /// <returns>uri del api carga</returns>
        public string GetUrl()
        {
            if (string.IsNullOrEmpty(Url))
            {
                string connectionString = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("ConfigUrl"))
                {
                    connectionString = environmentVariables["ConfigUrl"] as string;
                }
                else
                {
                    connectionString = _configuration["ConfigUrl"];
                }

                Url = connectionString;
            }
            return Url;
        }
        /// <summary>
        /// Obtiene la url del api de uris factory que ha sido configurada
        /// </summary>
        /// <returns>uri del api uris factory</returns>
        public string GetUrlUrisFactory()
        {
            if (string.IsNullOrEmpty(UrlUris))
            {
                string connectionString = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("ConfigUrlUrisFactory"))
                {
                    connectionString = environmentVariables["ConfigUrlUrisFactory"] as string;
                }
                else
                {
                    connectionString = _configuration["ConfigUrlUrisFactory"];
                }

                UrlUris = connectionString;
            }
            return UrlUris;
        }

        /// <summary>
        /// Obtiene la url del api de documentación que ha sido configurada
        /// </summary>
        /// <returns>uri del api uris factory</returns>
        public string GetUrlDocumentacion()
        {
            if (string.IsNullOrEmpty(UrlDocumentacion))
            {
                string connectionString = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("ConfigUrlDocumentacion"))
                {
                    connectionString = environmentVariables["ConfigUrlDocumentacion"] as string;
                }
                else
                {
                    connectionString = _configuration["ConfigUrlDocumentacion"];
                }

                UrlDocumentacion = connectionString;
            }
            return UrlDocumentacion;
        }

        /// <summary>
        /// Obtiene el parametro query para Sparql
        /// </summary>
        /// <returns>uri del api carga</returns>
        public string GetSparqlQuery()
        {
            if (string.IsNullOrEmpty(SparqlQuery))
            {
                string connectionString = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("SparqlQuery"))
                {
                    connectionString = environmentVariables["SparqlQuery"] as string;
                }
                else
                {
                    connectionString = _configuration["Sparql:QueryParam"];
                }

                Proxy = connectionString;
            }
            return Proxy;
        }

        /// <summary>
        /// Obtiene la dirección de sparql
        /// </summary>
        public string GetSaprqlEndpoint()
        {
            if (string.IsNullOrEmpty(SaprqlEndpoint))
            {
                string connectionString = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("SparqlEndpoint"))
                {
                    connectionString = environmentVariables["SparqlEndpoint"] as string;
                }
                else
                {
                    connectionString = _configuration["Sparql:Endpoint"];
                }

                SaprqlEndpoint = connectionString;
            }
            return SaprqlEndpoint;
        }

        /// <summary>
        /// Obtiene la dirección del directoio virtual configurado para el proxy
        /// </summary>
        /// <returns>uri del api carga</returns>
        public string GetProxy()
        {
            if (string.IsNullOrEmpty(Proxy))
            {
                string connectionString = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("Proxy"))
                {
                    connectionString = environmentVariables["Proxy"] as string;
                }
                else
                {
                    connectionString = _configuration["Proxy"];
                }

                Proxy = connectionString;
            }
            return Proxy;
        }
    }
}
