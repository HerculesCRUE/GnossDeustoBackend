using Newtonsoft.Json;
using System.IO;
using System.Text;
using UrisFactory.Models.ConfigEntities;

namespace UrisFactory.Models.Services
{
    public class ReaderConfigJson
    {
        public static UriStructureGeneral Read()
        {
            string texto = File.ReadAllText("Config/UrisConfig.json");
            return Read(texto);
        }

        public static UriStructureGeneral Read(string texto)
        {
            UriStructureGeneral uriStructure = JsonConvert.DeserializeObject<UriStructureGeneral>(texto);
            return uriStructure;
        }
    }
}
