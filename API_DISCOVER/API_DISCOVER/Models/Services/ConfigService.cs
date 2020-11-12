// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para obtener la configuración necesaria para el uso de Sparql
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Globalization;
using System.IO;

namespace API_DISCOVER.Models.Services
{
    ///<summary>
    ///Clase para obtener la configuración necesaria del servicio
    ///</summary>
    public class ConfigService
    {
        public IConfigurationRoot Configuration { get; set; }
        private float MaxScore { get; set; }
        private float MinScore { get; set; }

        ///<summary>
        ///Obtiene el MaxScore
        ///</summary>
        public float GetMaxScore()
        {
            if (MaxScore==0)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("MaxScore"))
                {
                    MaxScore = float.Parse( environmentVariables["MaxScore"] as string, CultureInfo.InvariantCulture);
                }
                else
                {
                    MaxScore = float.Parse(Configuration["MaxScore"], CultureInfo.InvariantCulture);
                }
                
            }
            return MaxScore;
        }

        ///<summary>
        ///Obtiene el MinScore
        ///</summary>
        public float GetMinScore()
        {
            if (MinScore == 0)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("MinScore"))
                {
                    MinScore = float.Parse(environmentVariables["MinScore"] as string, CultureInfo.InvariantCulture);
                }
                else
                {
                    MinScore = float.Parse(Configuration["MinScore"], CultureInfo.InvariantCulture);
                }

            }
            return MinScore;
        }
    }
}
