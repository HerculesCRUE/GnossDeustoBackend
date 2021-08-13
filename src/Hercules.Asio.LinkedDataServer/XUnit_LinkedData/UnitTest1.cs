using Hercules.Asio.LinkedDataServer.Utility;
using Linked_Data_Server.Controllers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using VDS.RDF;
using VDS.RDF.Parsing;
using Xunit;

namespace XUnit_LinkedData
{
    public class UnitTest1
    {
        [Fact]
        public void TestGenerarModeloDeUrl()
        {
            //Cargamos el RDF sobre el que aplicar la reconciliación
            RohGraph dataGraph = new RohGraph();
            dataGraph.LoadFromString(System.IO.File.ReadAllText("rdfFiles/cv1.rdf"), new RdfXmlParser());

            //Cargamos el RDF de la ontología
            RohGraph ontologyGraph = new RohGraph();
            ontologyGraph.LoadFromFile("Ontology/roh-v2.owl");

            RohGraph dataGraphBBDD = new RohGraph();
            dataGraphBBDD.Merge(dataGraph);
            dataGraphBBDD.Merge(ontologyGraph);

            SparqlUtilityMock sparqlUtilityMock = new SparqlUtilityMock(dataGraphBBDD);
            HomeController homeController = new HomeController(sparqlUtilityMock, null);
            string xApp = "";
            var result = homeController.GenerarModeloDeUrl("http://graph.um.es/res/person/4bd9f652-54fa-42b7-8226-a1a140815a20", ref xApp);
            Assert.True(result.linkedDataRDF.Count > 0 && result.propsTransform.Count > 0 && result.tables.Count > 0);
        }

        [Fact]
        public void TestAutocomplete()
        {
            //Cargamos el RDF sobre el que aplicar la reconciliación
            RohGraph dataGraph = new RohGraph();
            dataGraph.LoadFromString(System.IO.File.ReadAllText("rdfFiles/cv1.rdf"), new RdfXmlParser());

            //Cargamos el RDF de la ontología
            RohGraph ontologyGraph = new RohGraph();
            ontologyGraph.LoadFromFile("Ontology/roh-v2.owl");

            RohGraph dataGraphBBDD = new RohGraph();
            dataGraphBBDD.Merge(dataGraph);
            dataGraphBBDD.Merge(ontologyGraph);

            SparqlUtilityMock sparqlUtilityMock = new SparqlUtilityMock(dataGraphBBDD);
            AutocompleteController x = new AutocompleteController(sparqlUtilityMock, null);
            var result = (List<KeyValuePair<string, string>>)((JsonResult)x.Index("ped")).Value;
            Assert.True(result.Count > 0);
        }

        [Fact]
        public void TestSearch()
        {
            //Cargamos el RDF sobre el que aplicar la reconciliación
            RohGraph dataGraph = new RohGraph();
            dataGraph.LoadFromString(System.IO.File.ReadAllText("rdfFiles/cv1.rdf"), new RdfXmlParser());

            //Cargamos el RDF de la ontología
            RohGraph ontologyGraph = new RohGraph();
            ontologyGraph.LoadFromFile("Ontology/roh-v2.owl");

            RohGraph dataGraphBBDD = new RohGraph();
            dataGraphBBDD.Merge(dataGraph);
            dataGraphBBDD.Merge(ontologyGraph);

            SparqlUtilityMock sparqlUtilityMock = new SparqlUtilityMock(dataGraphBBDD);
            SearchController x = new SearchController(sparqlUtilityMock);
            var result = x.GenerateSearchTemplate("ped", "", "", 1);
            var result2 = x.GenerateSearchTemplate("", "http://graph.um.es/concept/1", "", 1);
            var result3 = x.GenerateSearchTemplate("", "", "ia", 1);
            Assert.True(result.entidades.Count > 0 && result2.entidades.Count > 0 && result3.entidades.Count > 0);
        }
    }
}
