using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UrisFactory.Models.ConfigEntities;
using UrisFactory.Models.Services;

namespace UrisDLL
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, string> queryString = new Dictionary<string, string>();
            queryString.Add("identifier", "123d");
            string texto = File.ReadAllText("Config/UrisConfig.json");
            UriStructureGeneral uriStructure = JsonConvert.DeserializeObject<UriStructureGeneral>(texto);
            ConfigJsonHandler configJsonHandler = new ConfigJsonHandler(texto);
            UriFormer uriFormer = new UriFormer(configJsonHandler.GetUrisConfig());
            string uri = uriFormer.GetURI("AdvisorRole", queryString);
            Console.WriteLine(uri);
        }
    }
}
