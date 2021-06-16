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
    //No se necesita esta configuracion para los test
    [ExcludeFromCodeCoverage]
    ///<summary>
    ///Clase para obtener la configuración necesaria para el uso del API de WOS
    ///</summary>
    public class ConfigWOS
    {
        public IConfigurationRoot Configuration { get; set; }
        private string WOSAuthorization { get; set; }

        ///<summary>
        ///Obtiene la autorización para WOS
        ///</summary>
        public string GetWOSAuthorization()
        {
            if (string.IsNullOrEmpty(WOSAuthorization))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("WOSAuthorization"))
                {
                    WOSAuthorization = environmentVariables["WOSAuthorization"] as string;
                }
                else
                {
                    WOSAuthorization = Configuration["WOSAuthorization"];
                }                                                
            }
            return WOSAuthorization;
        }
    }
}
