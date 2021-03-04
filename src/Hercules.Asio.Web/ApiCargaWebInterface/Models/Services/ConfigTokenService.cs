// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para la obtención de los datos necesarios para obtener los tokens de acceso 
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services
{
    /// <summary>
    /// Clase para la obtención de los datos necesarios para obtener los tokens de acceso 
    /// </summary>
    public class ConfigTokenService
    {
        public string Authority { get; set; }
        private IConfiguration _configuration { get; set; }
        public ConfigTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// obtiene el authority configurado
        /// </summary>
        /// <returns>authority</returns>
        public string GetAuthority()
        {
            if (string.IsNullOrEmpty(Authority))
            {
                string authority = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("Authority"))
                {
                    authority = environmentVariables["Authority"] as string;
                }
                else
                {
                    authority = _configuration["Authority"];
                }

                Authority = authority;
            }
            return Authority;
        }
    }
}
