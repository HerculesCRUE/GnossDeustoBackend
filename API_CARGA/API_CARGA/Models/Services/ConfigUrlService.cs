using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
