// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para obtener la configuración necesaria para el uso de Sparql
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
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
        private string UnidataDomain { get; set; }
        private string UnidataUriTransform { get; set; }    
        private string LaunchDiscoverLoadedEntitiesCronExpression { get; set; }
        private int? SleepSecondsAfterProcessEntityDiscoverLoadedEntities { get; set; }

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
        //No se van a hacer llamadas externas en un test unitatio
        [ExcludeFromCodeCoverage]
        /// <summary>
        /// Obtiene la url del dominio de Unidata
        /// </summary>
        /// <returns>uri del dominio de Unidata</returns>
        public string GetUnidataDomain()
        {
            if (string.IsNullOrEmpty(UnidataDomain))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                string connectionString = "";
                if (environmentVariables.Contains("UnidataDomain"))
                {
                    connectionString = environmentVariables["UnidataDomain"] as string;
                }
                else
                {
                    connectionString = Configuration["UnidataDomain"];
                }
                UnidataDomain = connectionString;
            }
            return UnidataDomain;
        }

        //No se van a hacer llamadas externas en un test unitatio
        [ExcludeFromCodeCoverage]
        /// <summary>
        /// Uri por la que se sustituirá el dominio de las URLs para cargarlas en Unidata
        /// </summary>
        /// <returns>Uri por la que se sustituirá el dominio de las URLs para cargarlas en Unidata</returns>
        public string GetUnidataUriTransform()
        {
            if (string.IsNullOrEmpty(UnidataUriTransform))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                string connectionString = "";
                if (environmentVariables.Contains("UnidataUriTransform"))
                {
                    connectionString = environmentVariables["UnidataUriTransform"] as string;
                }
                else
                {
                    connectionString = Configuration["UnidataUriTransform"];
                }
                UnidataUriTransform = connectionString;
            }
            return UnidataUriTransform;
        }
        //No se van a hacer llamadas a otro servicio
        [ExcludeFromCodeCoverage]
        /// <summary>
        /// Expresión Cron para representar cuando debe lanzarse el proceso de descubrimiento de enlaces
        /// </summary>
        /// <returns>Expresión Cron</returns>
        public string GetLaunchDiscoverLoadedEntitiesCronExpression()
        {
            if (string.IsNullOrEmpty(LaunchDiscoverLoadedEntitiesCronExpression))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                string dataString = "";
                if (environmentVariables.Contains("LaunchDiscoverLoadedEntitiesCronExpression"))
                {
                    dataString = environmentVariables["LaunchDiscoverLoadedEntitiesCronExpression"] as string;
                }
                else
                {
                    dataString = Configuration["LaunchDiscoverLoadedEntitiesCronExpression"];
                }
                LaunchDiscoverLoadedEntitiesCronExpression = dataString;
            }
            return LaunchDiscoverLoadedEntitiesCronExpression;
        }

        [ExcludeFromCodeCoverage]
        /// <summary>
        /// Segundos para 'dormir' tras procesar una entidad por el proceso de descubrimiento de enlaces
        /// </summary>
        /// <returns>Segundos</returns>
        public int GetSleepSecondsAfterProcessEntityDiscoverLoadedEntities()
        {
            if (!SleepSecondsAfterProcessEntityDiscoverLoadedEntities.HasValue)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                string dataString = "";
                if (environmentVariables.Contains("SleepSecondsAfterProcessEntityDiscoverLoadedEntities"))
                {
                    dataString = environmentVariables["SleepSecondsAfterProcessEntityDiscoverLoadedEntities"] as string;
                }
                else
                {
                    dataString = Configuration["SleepSecondsAfterProcessEntityDiscoverLoadedEntities"];
                }
                int numOut;
                int.TryParse(dataString, out numOut);
                SleepSecondsAfterProcessEntityDiscoverLoadedEntities = numOut;
            }
            return SleepSecondsAfterProcessEntityDiscoverLoadedEntities.Value;
        }



    }
}
