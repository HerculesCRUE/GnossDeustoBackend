using API_CARGA.Models.Entities;
using Newtonsoft.Json;
using System;
using System.Linq;


namespace API_CARGA.Models.Services
{
    public class SparqlConfigJson
    {
        private SparqlConfig _config;

        public SparqlConfigJson()
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
        public SparqlConfig GetConfig()
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
                _config = JsonConvert.DeserializeObject<SparqlConfig>(System.IO.File.ReadAllText("Config/SparqlConfig.json"));
            }
            catch (Exception)
            {
                throw new Exception("Could not load config file, maybe Config/SparqlConfig.json does not exist or is bad formed");
            }
        }
    }
}
