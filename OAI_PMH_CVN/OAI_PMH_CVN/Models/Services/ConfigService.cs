using Newtonsoft.Json;
using OaiPmhNet.Models.OAIPMH;
using System;
using System.Linq;

namespace OaiPmhNet.Models.Services
{
    /// <summary>
    /// Configuración del servicio
    /// </summary>
    public class ConfigService
    {
        private OAI_PMH_CVN_Config _config;

        /// <summary>
        /// Constructor
        /// </summary>
        public ConfigService()
        {
            InitializerConfigService();
        }

        private void InitializerConfigService()
        {
            if (_config == null)
            {
                LoadConfigService();
            }
        }

        /// <summary>
        /// Obtiene la configuración del servicio
        /// </summary>
        /// <returns>Configuración del servicio</returns>
        public OAI_PMH_CVN_Config GetConfig()
        {
            if (_config == null)
            {
                InitializerConfigService();
            }
            return _config;
        }

        /// <summary>
        /// Carga la configuraión del servicio
        /// </summary>
        public void LoadConfigService()
        {
            try
            {
                _config = JsonConvert.DeserializeObject<OAI_PMH_CVN_Config>(System.IO.File.ReadAllText("Config/OAI_PMH_CVN_Config.json"));
            }
            catch (Exception)
            {
                throw new Exception("No se encuentra el fichero de configuración, puede que Config/OAI_PMH_CVN_Config.json no exista o no esté correctamente formateado");
            }
        }
    }
}
