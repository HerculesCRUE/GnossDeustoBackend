// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Servicio para Obtener los path de log configurados
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
    /// Servicio para Obtener los path de log configurados
    /// </summary>
    public class ConfigPathLog
    {
        private string _LogPath;
        private string _LogPathCarga;
        private string _LogPathCron;
        private string _LogPathBase;
        private IConfiguration _configuration { get; set; }

        public ConfigPathLog(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Obtiene el nombre de la carpeta configurada para los logs de la propia aplicación
        /// </summary>
        /// <returns>path propio</returns>
        public string GetLogPath()
        {
            if (string.IsNullOrEmpty(_LogPath))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                string logPath = "";
                if (environmentVariables.Contains("LogPath"))
                {
                    logPath = environmentVariables["LogPath"] as string;
                }
                else
                {
                    logPath = _configuration["LogPath"];
                }
                _LogPath = logPath;
            }
            return _LogPath;
        }
        /// <summary>
        /// Obtiene el pathBase de log configurado para los apis
        /// </summary>
        /// <returns>path de logs del apiCarga</returns>
        public string GetLogPathBase()
        {
            if (string.IsNullOrEmpty(_LogPathBase))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                string logPath = "";
                if (environmentVariables.Contains("LogPathBase"))
                {
                    logPath = environmentVariables["LogPathBase"] as string;
                }
                else
                {
                    logPath = _configuration["LogPathBase"];
                }
                _LogPathBase = logPath;
            }
            return _LogPathBase;
        }
        /// <summary>
        /// Obtiene el nombre de la carpeta configurada para el api carga
        /// </summary>
        /// <returns>path de logs del apiCarga</returns>
        public string GetLogPathCarga()
        {
            if (string.IsNullOrEmpty(_LogPathCarga))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                string logPath = "";
                if (environmentVariables.Contains("LogPathCarga"))
                {
                    logPath = environmentVariables["LogPathCarga"] as string;
                }
                else
                {
                    logPath = _configuration["LogPathCarga"];
                }
                _LogPathCarga = logPath;
            }
            return _LogPathCarga;
        }
        /// <summary>
        /// Obtiene el nombre de la carpeta configurada para el api cron
        /// </summary>
        /// <returns>path de logs del apiCron</returns>
        public string GetLogPathCron()
        {
            if (string.IsNullOrEmpty(_LogPathCron))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                string logPath = "";
                if (environmentVariables.Contains("LogPathCron"))
                {
                    logPath = environmentVariables["LogPathCron"] as string;
                }
                else
                {
                    logPath = _configuration["LogPathCron"];
                }
                _LogPathCron = logPath;
            }
            return _LogPathCron;
        }
    }
}
