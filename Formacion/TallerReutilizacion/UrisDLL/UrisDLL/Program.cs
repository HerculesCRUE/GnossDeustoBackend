using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UrisFactory.Controllers;
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

            Get();
        }

        public static void Get()
        {
            UriStructureGeneral structureGeneral = new UriStructureGeneral() { Base = "http://graph.um.es" };
            List<Characters> characters = new List<Characters>();
            Characters charac = new Characters()
            {
                Character = "resource",
                LabelCharacter = "res"
            };
            characters.Add(charac);
            structureGeneral.Characters = characters;
            Component baseC = new Component()
            {
                UriComponent = "base",
                UriComponentValue = "base",
                UriComponentOrder = 1,
                Mandatory = true,
                FinalCharacter = "/"
            };
            Component character = new Component()
            {
                UriComponent = "character",
                UriComponentValue = "character@resource",
                UriComponentOrder = 2,
                Mandatory = true,
                FinalCharacter = "/"
            };
            Component resource = new Component()
            {
                UriComponent = "resourceClass",
                UriComponentValue = "resourceClass",
                UriComponentOrder = 3,
                Mandatory = true,
                FinalCharacter = "/"
            };
            Component identifier = new Component()
            {
                UriComponent = "identifier",
                UriComponentValue = "@ID",
                UriComponentOrder = 4,
                Mandatory = true,
                FinalCharacter = ""
            };
            List<Component> componentes = new List<Component>();
            componentes.Add(baseC);
            componentes.Add(character);
            componentes.Add(resource);
            componentes.Add(identifier);
            UriStructure uriStructure = new UriStructure()
            {
                Name = "test",
                Components = componentes
            };
            structureGeneral.UriStructures = new List<UriStructure>();
            structureGeneral.UriStructures.Add(uriStructure);
            ResourcesClass clas = new ResourcesClass()
            {
                LabelResourceClass = "project-object",
                ResourceClass = "Project",
                ResourceURI = "test"
            };
            ResourcesClass clas2 = new ResourcesClass()
            {
                LabelResourceClass = "researcher",
                ResourceClass = "Researcher",
                ResourceURI = "test"
            };
            structureGeneral.ResourcesClasses = new List<ResourcesClass>();
            structureGeneral.ResourcesClasses.Add(clas);
            structureGeneral.ResourcesClasses.Add(clas2);

            string uriSchemaJson = JsonConvert.SerializeObject(structureGeneral);

            ConfigJsonHandler config = new ConfigJsonHandler(uriSchemaJson);
            Dictionary<string, string> queryString = new Dictionary<string, string>();
            queryString.Add("identifier", "123d");
            UriFormer uriFormer = new UriFormer(config.GetUrisConfig());
            string uri = uriFormer.GetURI("Project", queryString);
            Console.WriteLine(uri);
        }
    }
}
