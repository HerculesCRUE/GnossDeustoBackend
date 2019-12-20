using Newtonsoft.Json;
using System.IO;
using UrisFactory.Models.ConfigEntities;

namespace UrisFactory.Models.Services
{
    public class ReaderConfigJson
    {
        public static UriStructureGeneral Read()
        {
            string texto = File.ReadAllText("Config/UrisConfig.json");
            UriStructureGeneral uriStructure = JsonConvert.DeserializeObject<UriStructureGeneral>(texto);
            return uriStructure;
        }
    }
}
