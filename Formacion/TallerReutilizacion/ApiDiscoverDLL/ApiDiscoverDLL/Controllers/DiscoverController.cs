using API_DISCOVER.Models.Entities.Discover;
using API_DISCOVER.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Parsing;

namespace ApiDiscoverDLL.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DiscoverController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            //Cargamos el RDF sobre el que aplicar el reconocimiento de enlaces
            RohGraph dataGraph = new RohGraph();
            dataGraph.LoadFromString(System.IO.File.ReadAllText("rdfFiles/rdfFile.rdf"), new RdfXmlParser());

            //Cargamos el RDF de la ontología
            RohGraph ontologyGraph = new RohGraph();
            ontologyGraph.LoadFromFile("Ontology/roh-v2.owl");

            DiscoverUtility discoverUtility = new DiscoverUtility();
            discoverUtility.test = true;

            //Aplicamos el descubrimiento de enlaces
            Dictionary<string, List<DiscoverLinkData.PropertyData>> discoverLinks = discoverUtility.ApplyDiscoverLinks(ref dataGraph, ontologyGraph, 0.7f, 0.9f, "", "", "HerculesASIO-University-of-Murcia (https://github.com/HerculesCRUE/GnossDeustoBackend; mailto:<mail>) AsioBot", "");

            //En 'discoverLinks' estarán los datos que se han recuperado de las integraciones externas junto con su provenance
            //En 'dataGraph' estará el grafo modificado tras el descubrimiento de enlaces
            return Ok(discoverLinks);
        }
    }
}
