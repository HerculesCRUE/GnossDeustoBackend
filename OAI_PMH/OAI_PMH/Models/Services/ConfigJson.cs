using Newtonsoft.Json;
using OaiPmhNet.Models.OAIPMH;
using System;
using System.Linq;

namespace OaiPmhNet.Models.Services
{
    public class ConfigJson
    {
        private OAI_PMHConfig _config;

        public ConfigJson()
        {
            InitializerConfigJson();
        }

        private void InitializerConfigJson()
        {
            if (_config == null)
            {
                LoadConfigJson();
            }
        }
        public OAI_PMHConfig GetConfig()
        {
            if (_config == null)
            {
                InitializerConfigJson();
            }
            return _config;
        }

        public void LoadConfigJson()
        {
            try
            {
                _config = JsonConvert.DeserializeObject<OAI_PMHConfig>(System.IO.File.ReadAllText("Config/OAI_PMHConfig.json"));
            }
            catch (Exception)
            {
                throw new Exception("Could not load config file, maybe Config/OAI_PMHConfig.json does not exist or is bad formed");
            }
        }
    }
}
