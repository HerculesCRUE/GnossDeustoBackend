using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hercules.Asio.UrisFactory.Models.Services
{
    public class ConfigService
    {
        private IConfiguration _configuration { get; set; }
        private string Base { get; set; }

        public ConfigService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        ///<summary>
        ///Método que obtiene la ConfigUrl configurada
        ///</summary>
        public string GetBase()
        {
            if (string.IsNullOrEmpty(Base))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                string baseUrl = "";
                if (environmentVariables.Contains("Base"))
                {
                    baseUrl = environmentVariables["Base"] as string;
                }
                else
                {
                    baseUrl = _configuration["Base"];
                }
                Base = baseUrl;
            }
            return Base;
        }
    }
}
