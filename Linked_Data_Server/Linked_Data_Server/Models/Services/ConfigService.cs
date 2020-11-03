using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Linked_Data_Server.Models.Services
{
    ///<summary>
    ///Clase para obtener la configuración necesaria
    ///</summary>
    public class ConfigService
    {
        public IConfigurationRoot Configuration { get; set; }
        private string NameTitle { get; set; }
        private string ConstrainedByUrl { get; set; }

        ///<summary>
        ///Obtiene el título:NameTitle del fichero appsettings.json
        ///</summary>
        public string GetNameTitle()
        {
            if (string.IsNullOrEmpty(NameTitle))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("NameTitle"))
                {
                    NameTitle = environmentVariables["NameTitle"] as string;
                }
                else
                {
                    NameTitle = Configuration["NameTitle"];
                }

            }
            return NameTitle;
        }

        ///<summary>
        ///Obtiene el título:NameTitle del fichero appsettings.json
        ///</summary>
        public string GetConstrainedByUrl()
        {
            if (string.IsNullOrEmpty(ConstrainedByUrl))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("ConstrainedByUrl"))
                {
                    ConstrainedByUrl = environmentVariables["ConstrainedByUrl"] as string;
                }
                else
                {
                    ConstrainedByUrl = Configuration["ConstrainedByUrl"];
                }

            }
            return ConstrainedByUrl;
        }
    }

    
}
