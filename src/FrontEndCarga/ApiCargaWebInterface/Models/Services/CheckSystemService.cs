// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Servicio para comprobar el estado del sistema
using ApiCargaWebInterface.Models.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services
{
    /// <summary>
    /// Servicio para comprobar el estado del sistema
    /// </summary>
    public class CheckSystemService
    {
        private CallCronApiService _callCronApiService;
        private ICallRepositoryConfigService _callRepositoryConfigService;
        private CallTokenService _callTokenService;
        private ConfigPathLog _configPathLog;
        public CheckSystemService(CallCronApiService callCronApiService, ICallRepositoryConfigService callRepositoryConfigService, CallTokenService callTokenService, ConfigPathLog configPathLog)
        {
            _callCronApiService = callCronApiService;
            _callRepositoryConfigService = callRepositoryConfigService;
            _callTokenService = callTokenService;
            _configPathLog = configPathLog;
        }
        /// <summary>
        /// Obtiene una lista de ficheros de log
        /// </summary>
        /// <param name="api">nombre del api</param>
        /// <returns>diccioario con el nombre del fichero y fecha de la última modificación</returns>
        public Dictionary<string, DateTime> GetLogs(string api)
        {
            if (api.Equals("Carga"))
            {
                return GetCargaLogs();
            }
            else if (api.Equals("Cron"))
            {
                return GetCronLogs();
            }
            else if (api.Equals("Web"))
            {
                return GetWebLogs();
            }
            return null;
        }
        /// <summary>
        /// Obtiene la información de un log determinado
        /// </summary>
        /// <param name="log_name">nombre del log</param>
        /// <param name="api">api del cual se quiere obtener el log</param>
        /// <returns></returns>
        public string GetLog(string log_name, string api)
        {
            string pathApi = "";
            string path = null;
            string fileText = null;
            if (api.Equals("Carga"))
            {
                pathApi = $"{_configPathLog.GetLogPathBase()}{_configPathLog.GetLogPathCarga()}";
            }
            else if (api.Equals("Cron"))
            {
                pathApi = $"{_configPathLog.GetLogPathBase()}{_configPathLog.GetLogPathCron()}";
            }
            else if (api.Equals("Web"))
            {
                pathApi = $"{_configPathLog.GetLogPathBase()}{_configPathLog.GetLogPath()}";
            }
            if (!string.IsNullOrEmpty(pathApi))
            {
                path = $"{pathApi}/{log_name}";
                var stream = File.Open(path, FileMode.Open,FileAccess.Read, FileShare.ReadWrite);
                var streamReader = new StreamReader(stream);
                fileText = streamReader.ReadToEnd();
                streamReader.Close();
                stream.Close();
                //fileText = File.ReadAllText(path);
            }
            return fileText;
        }

        /// <summary>
        /// Obtiene una lista de ficheros de log del api cron
        /// </summary>
        /// <returns>diccioario con el nombre del fichero y fecha de la última modificación</returns>
        public Dictionary<string, DateTime> GetCronLogs()
        {
            return GetFiles($"{_configPathLog.GetLogPathBase()}{_configPathLog.GetLogPathCron()}");
        }
        /// <summary>
        /// Obtiene una lista de ficheros de log del api carga
        /// </summary>
        /// <returns>diccioario con el nombre del fichero y fecha de la última modificación</returns>
        public Dictionary<string, DateTime> GetCargaLogs()
        {
            return GetFiles($"{_configPathLog.GetLogPathBase()}{_configPathLog.GetLogPathCarga()}");
        }

        /// <summary>
        /// Obtiene una lista de ficheros de log de la web
        /// </summary>
        /// <returns>diccioario con el nombre del fichero y fecha de la última modificación</returns>
        public Dictionary<string, DateTime> GetWebLogs()
        {
            return GetFiles($"{_configPathLog.GetLogPathBase()}{_configPathLog.GetLogPath()}");
        }
        /// <summary>
        /// Obtiene una lista de ficheros de una ruta determinada
        /// </summary>
        /// <returns>diccioario con el nombre del fichero y fecha de la última modificación</returns>
        private Dictionary<string,DateTime> GetFiles(string directory)
        {
            Dictionary<string, DateTime> filesInfo = new System.Collections.Generic.Dictionary<string, DateTime>();
            if (Directory.Exists(directory))
            {
                var files = Directory.GetFiles(directory).OrderByDescending(item => File.GetLastWriteTime(item));
                foreach (string file in files)
                {
                    DateTime modified = File.GetLastWriteTime(file);
                    string name = Path.GetFileName(file);
                    filesInfo.Add(name, modified);
                }
            }
            return filesInfo;
        }

        /// <summary>
        /// Comprueba si hay algún servicio en mal estado
        /// </summary>
        /// <returns>informe</returns>
        public CheckSystemReport CheckSystem()
        {
            CheckSystemReport checkSystemReport = new CheckSystemReport();
            checkSystemReport.ApiCarga = CheckApiCarga();
            checkSystemReport.ApiCron = CheckCronApi();
            checkSystemReport.IdentityServer = CheckIdentityServer();
            return checkSystemReport;
        }

        /// <summary>
        /// Comprueba el estado del api de cron
        /// </summary>
        /// <returns>si se encuentra en un estado correcto</returns>
        public bool CheckCronApi()
        {
            try
            {
                _callCronApiService.GetJobs();
                return true;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }
        /// <summary>
        /// Comprueba el estado del api de carga
        /// </summary>
        /// <returns>si se encuentra en un estado correcto</returns>
        public bool CheckApiCarga()
        {
            try
            {
                _callRepositoryConfigService.GetRepositoryConfigs();
                return true;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }
        /// <summary>
        /// Comprueba el estado del servicio proveedor de tokens
        /// </summary>
        /// <returns>si se encuentra en un estado correcto</returns>
        public bool CheckIdentityServer()
        {
            try
            {
                _callTokenService.CallTokenCarga();
                return true;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }
    }
}
