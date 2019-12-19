using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UrisFactory.Models.ConfigEntities;

namespace UrisFactory.Models.Services
{
    public class ReaderConfigJSON
    {
        public static UriStructureGeneral Read()
        {
            string texto = File.ReadAllText("Config/UrisConfig.json");
            UriStructureGeneral uriStructure = JsonConvert.DeserializeObject<UriStructureGeneral>(texto);
            return uriStructure;
        }
    }
}
