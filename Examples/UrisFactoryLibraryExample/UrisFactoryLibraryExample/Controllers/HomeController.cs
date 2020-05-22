using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UrisFactory.Controllers;
using UrisFactory.Models.ConfigEntities;
using UrisFactory.Models.Services;

namespace UrisFactoryLibraryExample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        public HomeController()
        {
        }

        [HttpGet]
        public IActionResult Get()
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
                UriComponentValue = "character@RESOURCE",
                UriComponentOrder = 2,
                Mandatory = true,
                FinalCharacter = "/"
            };
            Component resource = new Component()
            {
                UriComponent = "resourceClass",
                UriComponentValue = "resourceClass@RESOURCECLASS",
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
                LabelResourceClass = "prueba",
                ResourceClass = "Test",
                ResourceURI = "test"
            };
            structureGeneral.ResourcesClasses = new List<ResourcesClass>();
            structureGeneral.ResourcesClasses.Add(clas);

            string uriSchemaJson = JsonConvert.SerializeObject(structureGeneral);

            ConfigJsonHandler config = new ConfigJsonHandler(uriSchemaJson);

            FactoryController factoryController = new FactoryController(config);
            var cosas = factoryController.GenerateUri("Test", "1234");
            return cosas;
        }
    }
}
