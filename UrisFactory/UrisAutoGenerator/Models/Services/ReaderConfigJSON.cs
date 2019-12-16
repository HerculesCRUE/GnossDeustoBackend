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
        public static UriStructure Read()
        {
            string texto = File.ReadAllText("Config/UrisConfig.json");
            UriStructure uriStructure = JsonConvert.DeserializeObject<UriStructure>(texto);
            return uriStructure;
        }
    }
}
