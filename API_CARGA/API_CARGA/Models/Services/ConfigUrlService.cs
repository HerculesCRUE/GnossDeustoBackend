// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase usada para obtener las urlsConfiguradas
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.IO;

namespace API_CARGA.Models.Services
{
    ///<summary>
    ///Clase usada para obtener las urlsConfiguradas
    ///</summary>
    public class ConfigUrlService
    {
        public IConfigurationRoot Configuration { get; set; }
        public string Url { get; set; }
        ///<summary>
        ///Obtiene la url configurada en ConfigUrl del fichero appsettings.json
        ///</summary>
        public string GetUrl()
        {
            if (string.IsNullOrEmpty(Url))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                string connectionString = "";
                if (environmentVariables.Contains("ConfigUrl"))
                {
                    connectionString = environmentVariables["ConfigUrl"] as string;
                }
                else
                {
                    connectionString = Configuration["ConfigUrl"];
                }
                Url = connectionString;
            }
            return Url;
        }
    }
}
