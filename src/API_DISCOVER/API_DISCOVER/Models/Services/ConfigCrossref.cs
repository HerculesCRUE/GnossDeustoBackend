// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para obtener la configuración necesaria para el uso de Sparql
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.IO;

namespace API_DISCOVER.Models.Services
{
    ///<summary>
    ///Clase para obtener la configuración necesaria para el uso del API de CROSSREF
    ///</summary>
    public class ConfigCrossref
    {
        public IConfigurationRoot Configuration { get; set; }
        private string CrossrefUserAgent { get; set; }
        
        ///<summary>
        ///Obtiene el userAgent para usar en las peticiones al API de Crossref
        ///</summary>
        public string GetCrossrefUserAgent()
        {
            if (string.IsNullOrEmpty(CrossrefUserAgent))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("CrossrefUserAgent"))
                {
                    CrossrefUserAgent = environmentVariables["CrossrefUserAgent"] as string;
                }
                else
                {
                    CrossrefUserAgent = Configuration["CrossrefUserAgent"];
                }                                                
            }
            return CrossrefUserAgent;
        }
    }
}
