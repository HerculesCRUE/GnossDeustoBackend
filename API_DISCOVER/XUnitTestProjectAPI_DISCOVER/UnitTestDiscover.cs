using System;
using Xunit;
using API_DISCOVER.Utility;
using VDS.RDF;
using VDS.RDF.Query.Inference;
using VDS.RDF.Parsing;
using System.Collections.Generic;
using API_DISCOVER.Models.Entities;
using VDS.RDF.Query;
using API_DISCOVER.Models.Services;
using System.Linq;

namespace XUnitTestProjectAPI_DISCOVER
{
    public class UnitTestDiscover
    {
        [Fact]
        public void TestPrepareData()
        {
            RohGraph dataGraph = new RohGraph();
            dataGraph.LoadFromString(rdfFile, new RdfXmlParser());

            RohRdfsReasoner reasoner = new RohRdfsReasoner();
            RohGraph ontologyGraph = new RohGraph();

            ontologyGraph.LoadFromFile("Ontology/roh-v2.owl");
            reasoner.Initialise(ontologyGraph);

            RohGraph dataInferenceGraph = dataGraph.Clone();
            reasoner.Apply(dataInferenceGraph);

            Dictionary<string, HashSet<string>> entitiesRdfTypes;
            Dictionary<string, string> entitiesRdfType;
            Dictionary<string, List<DisambiguationData>> disambiguationDataRdf;

            Discover.PrepareData(dataGraph, reasoner, out dataInferenceGraph, out entitiesRdfTypes, out entitiesRdfType, out disambiguationDataRdf, false);

            int count1 = 0;
            foreach(var entity in entitiesRdfTypes)
            {
                if (entity.Key.Contains("document"))
                {
                    if (entity.Value.Contains("http://purl.org/roh/mirror/bibo#Document") && entity.Value.Contains("http://purl.org/roh/mirror/obo/iao#IAO_0000030"))
                    {
                        count1++;
                    }
                }else if (entity.Key.Contains("person"))
                {
                    if (entity.Value.Contains("http://purl.org/roh/mirror/foaf#Person") && entity.Value.Contains("http://purl.org/roh/mirror/foaf#Agent"))
                    {
                        count1++;
                    }
                }
            }
            int count2 = 0;
            foreach (var entity in entitiesRdfType)
            {
                if (entity.Key.Contains("document") && entity.Value.Contains("http://purl.org/roh/mirror/bibo#Document"))
                {
                    count2++;
                    
                }
                else if (entity.Key.Contains("person") && entity.Value.Contains("http://purl.org/roh/mirror/foaf#Person"))
                {
                    count2++;
                }
            }
			int count3 = 0;
			foreach(var data in disambiguationDataRdf)
            {
                if (data.Key.Contains("person"))
                {
					int countperson = 0;
                    foreach(var values in data.Value)
                    {
						foreach(var property in values.properties)
						{
                            if (property.property.property.Contains("http://purl.org/roh/mirror/foaf#name") || property.property.property.Contains("http://purl.org/roh#correspondingAuthorOf") || property.property.property.Contains("http://purl.org/roh/mirror/bibo#authorList@@@?"))
							{
								countperson++;
                            }
                        }
                        if (countperson == 3)
                        {
							count3++;
                        }
                    }
                }
                else
                {
					if(data.Key.Contains("11826b3e") || data.Key.Contains("17d68fc4") || data.Key.Contains("86769078") || data.Key.Contains("a7db2203") || data.Key.Contains("feb48b56"))
                    {
						int countdoc = 0;
						foreach (var values in data.Value)
						{
							foreach (var property in values.properties)
							{
								if (property.property.property.Contains("http://purl.org/roh#title")) 
								{
									countdoc++;
								}
								if (property.property.property.Contains("http://purl.org/roh/mirror/bibo#authorList@@@?"))
								{
                                    if (property.values.Count == 1)
									{
										countdoc++;
									}
								}
							}
							if(countdoc == 2)
                            {
								count3++;
							}
						}
                    }
                    else
                    {
						int countdoc = 0;
						foreach (var values in data.Value)
						{
							foreach (var property in values.properties)
							{
								if (property.property.property.Contains("http://purl.org/roh#title"))
								{
									countdoc++;
								}
								else if(property.property.property.Contains("http://purl.org/roh/mirror/bibo#authorList@@@?"))
								{
									if (property.values.Count == 2)
									{
										countdoc++;
									}
								}
							}
							if (countdoc == 2)
							{
								count3++;
							}
						}
					}
                }
            }

            if(count1 ==16 && count2 == 16 && count3 == 16 )
            {
                Assert.True(true);
            }
            else
            {
				Assert.True(false);
            }
        }

        [Fact]
        public void TestReconciliateRDF()
        {
            bool hasChanges = false;

            Dictionary<string, string> discoveredEntityList = new Dictionary<string, string>();

            RohGraph dataGraph = new RohGraph();
            dataGraph.LoadFromString(rdfFileRecon, new RdfXmlParser());

            RohRdfsReasoner reasoner = new RohRdfsReasoner();

            Dictionary<string, HashSet<string>> discardDissambiguations = new Dictionary<string, HashSet<string>>();

            DiscoverCache discoverCache = new DiscoverCache();

            Discover.ReconciliateRDF(ref hasChanges, ref discoveredEntityList, ref dataGraph, reasoner, discardDissambiguations, discoverCache);
			if(hasChanges == true){
				if(discoveredEntityList.ContainsKey("http://graph.um.es/res/person/e12d2a31-e285-459a-9d71-2d4a45329532") && discoveredEntityList.ContainsValue("http://graph.um.es/res/person/aaad2a31-e285-459a-9d71-2d4a45329532") && dataGraph.Triples.Count == 89)
				{
					Assert.True(true);
                }
                else
                {
					Assert.True(false);
                }
			}
		}

     //   [Fact]
     //   public void TestExternalIntegration()
     //   {
     //       bool hasChanges = false;
     //       Dictionary<string, string> discoveredEntityList = new Dictionary<string, string>();

     //       Dictionary<string, Dictionary<string, float>> discoveredEntitiesProbability = new Dictionary<string, Dictionary<string, float>>();

     //       RohGraph dataGraph = new RohGraph();
     //       dataGraph.LoadFromString(rdfFile, new RdfXmlParser());

     //       RohRdfsReasoner reasoner = new RohRdfsReasoner();

     //       Dictionary<string, Dictionary<string, float>> namesScore = new Dictionary<string, Dictionary<string, float>>();

     //       RohGraph ontologyGraph = new RohGraph();
     //       ontologyGraph.LoadFromFile("Ontology/roh-v2.owl");
     //       reasoner.Initialise(ontologyGraph);

     //       Dictionary<string, KeyValuePair<string, float>> entidadesReconciliadasConIntegracionExternaAux;

     //       Dictionary<string, HashSet<string>> discardDissambiguations = new Dictionary<string, HashSet<string>>();

     //       DiscoverCache discoverCache = new DiscoverCache();

     //       RohGraph dataGraphBBDD = new RohGraph();
     //       dataGraphBBDD.LoadFromString(databaseFile, new RdfXmlParser());
     //       Discover.mSparqlUtility = new SparqlUtilityMock(dataGraphBBDD);

     //       Dictionary<string, Dictionary<string, KeyValuePair<string, HashSet<string>>>> externalIds = Discover.ExternalIntegration(ref hasChanges, ref discoveredEntityList, ref discoveredEntitiesProbability, ref dataGraph, reasoner, namesScore, ontologyGraph, out entidadesReconciliadasConIntegracionExternaAux, discardDissambiguations, discoverCache);

     //       if (hasChanges == true)
     //       {
     //           int suma = 0;
     //           foreach(var id in externalIds)
     //           {
     //               suma += id.Value.Count;
     //           }
     //           string query = @"select distinct ?s where{?s ?p ?o. FILTER(!isBlank(?s))}";
     //           SparqlResultSet sparqlResultSet = (SparqlResultSet)dataGraph.ExecuteQuery(query.ToString());
                
     //           if(suma > sparqlResultSet.Count/2)
     //           {
     //               Assert.True(true);
     //           }
     //           else
     //           {
					//Assert.True(false);
     //           }
     //       }
     //   }

        [Fact]
        public void TestLoadNamesScore()
        {
            Dictionary<string, Dictionary<string, float>> namesScore = new Dictionary<string, Dictionary<string, float>>();

            RohGraph dataGraphBBDD = new RohGraph();
            dataGraphBBDD.LoadFromString(databaseFile, new RdfXmlParser()); 

            Discover.mSparqlUtility = new SparqlUtilityMock(dataGraphBBDD);
            Dictionary<string, string> personsWithName = Discover.LoadPersonWithName();


            RohGraph dataGraph = new RohGraph();
            dataGraph.LoadFromString(rdfFile, new RdfXmlParser());

            RohGraph dataInferenceGraph = dataGraph.Clone();

            DiscoverCache discoverCache = new DiscoverCache();

            Discover.LoadNamesScore(ref namesScore, personsWithName, dataInferenceGraph, discoverCache);

			if(personsWithName.Count == 2 && personsWithName.ContainsValue("Diego López-de-Ipiña") && personsWithName.ContainsValue("Diego Casado-Mansilla"))
            {
				if(namesScore.Count == 2 && namesScore.ContainsKey("Diego López-de-Ipiña") && namesScore.ContainsKey("Diego Casado-Mansilla"))
				{
					Assert.True(true);
                }
                else
                {
					Assert.True(false);
                }
            }
        }

        [Fact]
        public void TestLoadEntitiesDB()
        {
            Dictionary<string, string> entitiesRdfType = new Dictionary<string, string>();

            Dictionary<string, string> discoveredEntityList = new Dictionary<string, string>();
            RohGraph dataGraphBBDD = new RohGraph();
            dataGraphBBDD.LoadFromString(databaseFile, new RdfXmlParser());
            Discover.mSparqlUtility = new SparqlUtilityMock(dataGraphBBDD);

            string query = @"select ?s ?rdftype where {?s a ?rdftype. FILTER(!isBlank(?s))}";
            SparqlResultSet sparqlResultSet = (SparqlResultSet)dataGraphBBDD.ExecuteQuery(query.ToString());
            foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
            {
                string s = sparqlResult["s"].ToString();
                string rdfType = sparqlResult["rdftype"].ToString();
                if (!entitiesRdfType.ContainsKey(s))
                {
                    entitiesRdfType.Add(s, rdfType);
                }
            }

            List<string> entidadesCargadas = Discover.LoadEntitiesDB(entitiesRdfType.Keys.ToList().Except(discoveredEntityList.Keys.Union(discoveredEntityList.Values))).Keys.ToList();
			int suma = 0;
			foreach(var entity in entitiesRdfType)
            {
				if (entidadesCargadas.Contains(entity.Key))
				{
					suma++;
				}
			}
            if(suma == 5)
            {
				Assert.True(true);
            }
            else
            {
				Assert.True(false);
            }

        }

        [Fact]
        public void TestReconciliateIDs()
        {
            bool hasChanges = false;

            Dictionary<string, string> discoveredEntityList = new Dictionary<string, string>();

            Dictionary<string, string> entitiesRdfType = new Dictionary<string, string>();

            Dictionary<string, List<DisambiguationData>> disambiguationDataRdf = new Dictionary<string, List<DisambiguationData>>();

            Dictionary<string, HashSet<string>> discardDissambiguations = new Dictionary<string, HashSet<string>>();

            RohGraph dataGraph = new RohGraph();
            dataGraph.LoadFromString(rdfFileReconciliateIDs, new RdfXmlParser());

            RohGraph dataGraphBBDD = new RohGraph();
            dataGraphBBDD.LoadFromString(databaseFileReconciliateIds, new RdfXmlParser());

            Discover.mSparqlUtility = new SparqlUtilityMock(dataGraphBBDD);



			RohRdfsReasoner reasoner = new RohRdfsReasoner();
			RohGraph ontologyGraph = new RohGraph();
			ontologyGraph.LoadFromFile("Ontology/roh-v2.owl");
			reasoner.Initialise(ontologyGraph);
			RohGraph dataInferenceGraph = dataGraph.Clone();
			reasoner.Apply(dataInferenceGraph);
			Dictionary<string, HashSet<string>> entitiesRdfTypes;
			Discover.PrepareData(dataGraph, reasoner, out dataInferenceGraph, out entitiesRdfTypes, out entitiesRdfType, out disambiguationDataRdf, false);


			Dictionary<string, string> entidadesReconciliadasConIdsAux = Discover.ReconciliateIDs(ref hasChanges, ref discoveredEntityList, entitiesRdfType, disambiguationDataRdf, discardDissambiguations, ref dataGraph); 

			if(entidadesReconciliadasConIdsAux.Count == 1 && entidadesReconciliadasConIdsAux.ContainsKey("http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0") && entidadesReconciliadasConIdsAux.ContainsValue("http://graph.um.es/res/person/aaa16863-a606-48e3-a858-9def000380c0") && hasChanges == true)
            {
				Assert.True(true);
            }
            else
            {
				Assert.True(false);
            }

        }

        [Fact]
        public void TestReconciliateBBDD()
        {
            bool hasChanges = false;

            Dictionary<string, string> discoveredEntityList = new Dictionary<string, string>();

            Dictionary<string, Dictionary<string, float>> discoveredEntitiesProbability = new Dictionary<string, Dictionary<string, float>>();

            RohGraph dataGraph = new RohGraph();
            dataGraph.LoadFromString(rdfFileRecon, new RdfXmlParser());

            RohRdfsReasoner reasoner = new RohRdfsReasoner();

            Dictionary<string, Dictionary<string, float>> namesScore = new Dictionary<string, Dictionary<string, float>>();

            Dictionary<string, HashSet<string>> discardDissambiguations = new Dictionary<string, HashSet<string>>();

            DiscoverCache discoverCache = new DiscoverCache();

            RohGraph dataGraphBBDD = new RohGraph();
            dataGraphBBDD.LoadFromString(rdfFile, new RdfXmlParser());
            Discover.mSparqlUtility = new SparqlUtilityMock(dataGraphBBDD);

            Discover.ReconciliateBBDD(ref hasChanges, ref discoveredEntityList, out discoveredEntitiesProbability, ref dataGraph, reasoner, namesScore, discardDissambiguations, discoverCache);
			if (hasChanges == true)
			{
				if (discoveredEntityList.ContainsKey("http://graph.um.es/res/person/aaad2a31-e285-459a-9d71-2d4a45329532") && discoveredEntityList.ContainsValue("http://graph.um.es/res/person/e12d2a31-e285-459a-9d71-2d4a45329532") && dataGraph.Triples.Count == 89)
				{
					Assert.True(true);
                }
                else
                {
					Assert.True(false);
                }
			}
		}


        private string databaseFile
        {
            get
            {
                string rdfFile = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE rdf:RDF [
        	<!ENTITY rdf 'http://www.w3.org/1999/02/22-rdf-syntax-ns#'>
		<!ENTITY rdfs 'http://www.w3.org/2000/01/rdf-schema#'>
			<!ENTITY xsd 'http://www.w3.org/2001/XMLSchema#'>
				]>
				<rdf:RDF xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#""
       					xmlns:xsd=""http://www.w3.org/2001/XMLSchema#""
       					xmlns:foaf=""http://purl.org/roh/mirror/foaf#""
       					xmlns:roh=""http://purl.org/roh#""
       					xmlns:bibo=""http://purl.org/roh/mirror/bibo#""
       					xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"">
					<foaf:Person rdf:about=""http://graph.um.es/res/person/aaa16863-a606-48e3-a858-9def000380c0"">
						<foaf:name rdf:datatype=""&xsd;string"">Diego Casado-Mansilla</foaf:name>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/aac8f70c-50ea-4de0-b3b6-5bcf4fdb829e""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/aac217c9-44c4-480d-9653-6d84b711a9b2""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/aa0d9d14-4cbb-4387-8220-5a6d5a43f2a0""/>
					</foaf:Person>
					<foaf:Person rdf:about=""http://graph.um.es/res/person/aa2d2a31-e285-459a-9d71-2d4a45329532"">
						<foaf:name rdf:datatype=""&xsd;string"">Diego López-de-Ipiña</foaf:name>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/aac8f70c-50ea-4de0-b3b6-5bcf4fdb829e""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/aac217c9-44c4-480d-9653-6d84b711a9b2""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/aa0d9d14-4cbb-4387-8220-5a6d5a43f2a0""/>
					</foaf:Person>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/aac8f70c-50ea-4de0-b3b6-5bcf4fdb829e"">
						<roh:title rdf:datatype=""&xsd;string"" >Exploring the computational cost of machine learning at the edge for human-centric Internet of Things.</roh:title>
						<bibo:authorList rdf:nodeID=""Naac461361fc640adaac6bcb1c7f54d0d"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/aac217c9-44c4-480d-9653-6d84b711a9b2"">
						<roh:title rdf:datatype=""&xsd;string"" >User perspectives in the design of interactive everyday objects for sustainable behaviour.</roh:title>
						<bibo:authorList rdf:nodeID=""Naad40eb0e46c40d083cf88d019323568"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/aa0d9d14-4cbb-4387-8220-5a6d5a43f2a0"">
						<roh:title rdf:datatype=""&xsd;string"" >Exploring the Application of the FOX Model to Foster Pro-Environmental Behaviours in Smart Environments.</roh:title>
						<bibo:authorList rdf:nodeID=""Naaf7938ae6494843a9622da4446bc608"" />
					</bibo:Document>
					<rdf:Seq rdf:nodeID=""Naac461361fc640adaac6bcb1c7f54d0d"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/aaa16863-a606-48e3-a858-9def000380c0"" />
						<rdf:_2 rdf:resource=""http://graph.um.es/res/person/aa2d2a31-e285-459a-9d71-2d4a45329532"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""Naad40eb0e46c40d083cf88d019323568"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/aaa16863-a606-48e3-a858-9def000380c0"" />
						<rdf:_2 rdf:resource=""http://graph.um.es/res/person/aa2d2a31-e285-459a-9d71-2d4a45329532"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""Naaf7938ae6494843a9622da4446bc608"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/aaa16863-a606-48e3-a858-9def000380c0"" />
						<rdf:_2 rdf:resource=""http://graph.um.es/res/person/aa2d2a31-e285-459a-9d71-2d4a45329532"" />
					</rdf:Seq>
				</rdf:RDF>
				";
                return rdfFile;
            }
        }


        private string rdfFile
        {
            get
            {
                string rdfFile = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE rdf:RDF [
        	<!ENTITY rdf 'http://www.w3.org/1999/02/22-rdf-syntax-ns#'>
		<!ENTITY rdfs 'http://www.w3.org/2000/01/rdf-schema#'>
			<!ENTITY xsd 'http://www.w3.org/2001/XMLSchema#'>
				]>
				<rdf:RDF xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#""
       					xmlns:xsd=""http://www.w3.org/2001/XMLSchema#""
       					xmlns:foaf=""http://purl.org/roh/mirror/foaf#""
       					xmlns:roh=""http://purl.org/roh#""
       					xmlns:bibo=""http://purl.org/roh/mirror/bibo#""
       					xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"">
					<foaf:Person rdf:about=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"">
						<foaf:name rdf:datatype=""&xsd;string"">Diego Casado-Mansilla</foaf:name>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/76c8f70c-50ea-4de0-b3b6-5bcf4fdb829e""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/eec217c9-44c4-480d-9653-6d84b711a9b2""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/990d9d14-4cbb-4387-8220-5a6d5a43f2a0""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/5d17fc89-8c73-4157-965c-bfacc6030171""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/584fc505-dbca-49f8-ac30-be7b44fc1957""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/3844c942-79ab-443c-bf59-4f74b2f1edeb""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/11826b3e-ce40-4097-a738-532561209b40""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/feb48b56-ac2a-4e52-aa81-c39f30dc18ae""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/a7db2203-49a1-4bb5-aac4-cda161c1468f""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/17d68fc4-0aa8-4ecd-a570-2946c42c5ade""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/86769078-ccc8-47dd-83aa-a472666b262f""/>
					</foaf:Person>
					<foaf:Person rdf:about=""http://graph.um.es/res/person/e12d2a31-e285-459a-9d71-2d4a45329532"">
						<foaf:name rdf:datatype=""&xsd;string"">Diego López-de-Ipiña</foaf:name>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/76c8f70c-50ea-4de0-b3b6-5bcf4fdb829e""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/eec217c9-44c4-480d-9653-6d84b711a9b2""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/990d9d14-4cbb-4387-8220-5a6d5a43f2a0""/>
					</foaf:Person>
					<foaf:Person rdf:about=""http://graph.um.es/res/person/799860d8-b7c5-4229-a4c3-44533871493d"">
						<foaf:name rdf:datatype=""&xsd;string"">Álvaro Palacios Marijuán</foaf:name>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/5d17fc89-8c73-4157-965c-bfacc6030171""/>
					</foaf:Person>
					<foaf:Person rdf:about=""http://graph.um.es/res/person/7a4898f2-195c-42d0-98c6-91ba121c14ea"">
						<foaf:name rdf:datatype=""&xsd;string"">Esteban Sota Leiva</foaf:name>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/584fc505-dbca-49f8-ac30-be7b44fc1957""/>
					</foaf:Person>
					<foaf:Person rdf:about=""http://graph.um.es/res/person/27b9fd2f-386c-4da6-bfe2-ce4fc6ace6a4"">
						<foaf:name rdf:datatype=""&xsd;string"">Oihane Gómez-Carmona</foaf:name>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/3844c942-79ab-443c-bf59-4f74b2f1edeb""/>
					</foaf:Person>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/76c8f70c-50ea-4de0-b3b6-5bcf4fdb829e"">
						<roh:title rdf:datatype=""&xsd;string"" >Exploring the computational cost of machine learning at the edge for human-centric Internet of Things.</roh:title>
						<bibo:authorList rdf:nodeID=""N57c461361fc640adaac6bcb1c7f54d0d"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/eec217c9-44c4-480d-9653-6d84b711a9b2"">
						<roh:title rdf:datatype=""&xsd;string"" >User perspectives in the design of interactive everyday objects for sustainable behaviour.</roh:title>
						<bibo:authorList rdf:nodeID=""N59d40eb0e46c40d083cf88d019323568"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/990d9d14-4cbb-4387-8220-5a6d5a43f2a0"">
						<roh:title rdf:datatype=""&xsd;string"" >Exploring the Application of the FOX Model to Foster Pro-Environmental Behaviours in Smart Environments.</roh:title>
						<bibo:authorList rdf:nodeID=""N55f7938ae6494843a9622da4446bc608"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/5d17fc89-8c73-4157-965c-bfacc6030171"">
						<roh:title rdf:datatype=""&xsd;string"" >Documento de prueba de Álvaro y Diego</roh:title>
						<bibo:authorList rdf:nodeID=""N57c461361fc640adaac6bcb1c7f54d0c"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/584fc505-dbca-49f8-ac30-be7b44fc1957"">
						<roh:title rdf:datatype=""&xsd;string"" >Documento de prueba con título inventado</roh:title>
						<bibo:authorList rdf:nodeID=""N57c461361fc640adaac6bcb1c7f54d22"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/3844c942-79ab-443c-bf59-4f74b2f1edeb"">
						<roh:title rdf:datatype=""&xsd;string"" >SmartWorkplace: A Privacy-based Fog Computing Approach to Boost Energy Efficiency and Wellness in Digital Workspaces.</roh:title>
						<bibo:authorList rdf:nodeID=""N57c461361fc640adaac6bcb1c7f54d0e"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/11826b3e-ce40-4097-a738-532561209b40"">
						<roh:title rdf:datatype=""&xsd;string"" >The DiY Smart Experiences Project - A European Endeavour Removing Barriers for User-generated Internet of Things Applications.</roh:title>
						<bibo:authorList rdf:nodeID=""N58dbc92831344c0b812c8a7d202a2399"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/feb48b56-ac2a-4e52-aa81-c39f30dc18ae"">
						<roh:title rdf:datatype=""&xsd;string"" >'Close the Loop': An iBeacon App to Foster Recycling Through Just-in-Time Feedback.</roh:title>
						<bibo:authorList rdf:nodeID=""N5a2ca30985bd4c4786717981a299f788"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/a7db2203-49a1-4bb5-aac4-cda161c1468f"">
						<roh:title rdf:datatype=""&xsd;string"" >To switch off the coffee-maker or not: that is the question to be energy-efficient at work.</roh:title>
						<bibo:authorList rdf:nodeID=""N5680b4d50ff4415883a761cbba34d2f7"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/17d68fc4-0aa8-4ecd-a570-2946c42c5ade"">
						<roh:title rdf:datatype=""&xsd;string"" >Validation of a CoAP to IEC 61850 Mapping and Benchmarking vs HTTP-REST and WS-SOAP.</roh:title>
						<bibo:authorList rdf:nodeID=""N5549bfe4dfdc496fafd47f3e77f64d7a"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/86769078-ccc8-47dd-83aa-a472666b262f"">
						<roh:title rdf:datatype=""&xsd;string"" >Design-insights for Devising Persuasive IoT Devices for Sustainability in the Workplace.</roh:title>
						<bibo:authorList rdf:nodeID=""N5c342df56893442da790bd591df1c245"" />
					</bibo:Document>
					<rdf:Seq rdf:nodeID=""N57c461361fc640adaac6bcb1c7f54d0d"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
						<rdf:_2 rdf:resource=""http://graph.um.es/res/person/e12d2a31-e285-459a-9d71-2d4a45329532"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""N59d40eb0e46c40d083cf88d019323568"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
						<rdf:_2 rdf:resource=""http://graph.um.es/res/person/e12d2a31-e285-459a-9d71-2d4a45329532"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""N55f7938ae6494843a9622da4446bc608"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
						<rdf:_2 rdf:resource=""http://graph.um.es/res/person/e12d2a31-e285-459a-9d71-2d4a45329532"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""N57c461361fc640adaac6bcb1c7f54d0c"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
						<rdf:_2 rdf:resource=""http://graph.um.es/res/person/799860d8-b7c5-4229-a4c3-44533871493d"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""N57c461361fc640adaac6bcb1c7f54d22"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
						<rdf:_2 rdf:resource=""http://graph.um.es/res/person/7a4898f2-195c-42d0-98c6-91ba121c14ea"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""N57c461361fc640adaac6bcb1c7f54d0e"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
						<rdf:_2 rdf:resource=""http://graph.um.es/res/person/27b9fd2f-386c-4da6-bfe2-ce4fc6ace6a4"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""N58dbc92831344c0b812c8a7d202a2399"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""N5a2ca30985bd4c4786717981a299f788"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""N5680b4d50ff4415883a761cbba34d2f7"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""N5549bfe4dfdc496fafd47f3e77f64d7a"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""N5c342df56893442da790bd591df1c245"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
					</rdf:Seq>
				</rdf:RDF>
				";
                return rdfFile;
            }
        }

		private string rdfFileRecon
		{
			get
			{
				string rdfFile = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE rdf:RDF [
        	<!ENTITY rdf 'http://www.w3.org/1999/02/22-rdf-syntax-ns#'>
		<!ENTITY rdfs 'http://www.w3.org/2000/01/rdf-schema#'>
			<!ENTITY xsd 'http://www.w3.org/2001/XMLSchema#'>
				]>
				<rdf:RDF xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#""
       					xmlns:xsd=""http://www.w3.org/2001/XMLSchema#""
       					xmlns:foaf=""http://purl.org/roh/mirror/foaf#""
       					xmlns:roh=""http://purl.org/roh#""
       					xmlns:bibo=""http://purl.org/roh/mirror/bibo#""
       					xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"">
					<foaf:Person rdf:about=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"">
						<foaf:name rdf:datatype=""&xsd;string"">Diego Casado-Mansilla</foaf:name>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/76c8f70c-50ea-4de0-b3b6-5bcf4fdb829e""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/eec217c9-44c4-480d-9653-6d84b711a9b2""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/990d9d14-4cbb-4387-8220-5a6d5a43f2a0""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/5d17fc89-8c73-4157-965c-bfacc6030171""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/584fc505-dbca-49f8-ac30-be7b44fc1957""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/3844c942-79ab-443c-bf59-4f74b2f1edeb""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/11826b3e-ce40-4097-a738-532561209b40""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/feb48b56-ac2a-4e52-aa81-c39f30dc18ae""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/a7db2203-49a1-4bb5-aac4-cda161c1468f""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/17d68fc4-0aa8-4ecd-a570-2946c42c5ade""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/86769078-ccc8-47dd-83aa-a472666b262f""/>
					</foaf:Person>
					<foaf:Person rdf:about=""http://graph.um.es/res/person/e12d2a31-e285-459a-9d71-2d4a45329532"">
						<foaf:name rdf:datatype=""&xsd;string"">Diego López-de-Ipiña</foaf:name>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/76c8f70c-50ea-4de0-b3b6-5bcf4fdb829e""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/eec217c9-44c4-480d-9653-6d84b711a9b2""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/990d9d14-4cbb-4387-8220-5a6d5a43f2a0""/>
					</foaf:Person>
					<foaf:Person rdf:about=""http://graph.um.es/res/person/aaad2a31-e285-459a-9d71-2d4a45329532"">
						<foaf:name rdf:datatype=""&xsd;string"">D. López-de-Ipiña</foaf:name>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/76c8f70c-50ea-4de0-b3b6-5bcf4fdb829e""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/eec217c9-44c4-480d-9653-6d84b711a9b2""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/990d9d14-4cbb-4387-8220-5a6d5a43f2a0""/>
					</foaf:Person>
					<foaf:Person rdf:about=""http://graph.um.es/res/person/799860d8-b7c5-4229-a4c3-44533871493d"">
						<foaf:name rdf:datatype=""&xsd;string"">Álvaro Palacios Marijuán</foaf:name>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/5d17fc89-8c73-4157-965c-bfacc6030171""/>
					</foaf:Person>
					<foaf:Person rdf:about=""http://graph.um.es/res/person/7a4898f2-195c-42d0-98c6-91ba121c14ea"">
						<foaf:name rdf:datatype=""&xsd;string"">Esteban Sota Leiva</foaf:name>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/584fc505-dbca-49f8-ac30-be7b44fc1957""/>
					</foaf:Person>
					<foaf:Person rdf:about=""http://graph.um.es/res/person/27b9fd2f-386c-4da6-bfe2-ce4fc6ace6a4"">
						<foaf:name rdf:datatype=""&xsd;string"">Oihane Gómez-Carmona</foaf:name>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/3844c942-79ab-443c-bf59-4f74b2f1edeb""/>
					</foaf:Person>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/76c8f70c-50ea-4de0-b3b6-5bcf4fdb829e"">
						<roh:title rdf:datatype=""&xsd;string"" >Exploring the computational cost of machine learning at the edge for human-centric Internet of Things.</roh:title>
						<bibo:authorList rdf:nodeID=""N57c461361fc640adaac6bcb1c7f54d0d"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/eec217c9-44c4-480d-9653-6d84b711a9b2"">
						<roh:title rdf:datatype=""&xsd;string"" >User perspectives in the design of interactive everyday objects for sustainable behaviour.</roh:title>
						<bibo:authorList rdf:nodeID=""N59d40eb0e46c40d083cf88d019323568"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/990d9d14-4cbb-4387-8220-5a6d5a43f2a0"">
						<roh:title rdf:datatype=""&xsd;string"" >Exploring the Application of the FOX Model to Foster Pro-Environmental Behaviours in Smart Environments.</roh:title>
						<bibo:authorList rdf:nodeID=""N55f7938ae6494843a9622da4446bc608"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/5d17fc89-8c73-4157-965c-bfacc6030171"">
						<roh:title rdf:datatype=""&xsd;string"" >Documento de prueba de Álvaro y Diego</roh:title>
						<bibo:authorList rdf:nodeID=""N57c461361fc640adaac6bcb1c7f54d0c"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/584fc505-dbca-49f8-ac30-be7b44fc1957"">
						<roh:title rdf:datatype=""&xsd;string"" >Documento de prueba con título inventado</roh:title>
						<bibo:authorList rdf:nodeID=""N57c461361fc640adaac6bcb1c7f54d22"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/3844c942-79ab-443c-bf59-4f74b2f1edeb"">
						<roh:title rdf:datatype=""&xsd;string"" >SmartWorkplace: A Privacy-based Fog Computing Approach to Boost Energy Efficiency and Wellness in Digital Workspaces.</roh:title>
						<bibo:authorList rdf:nodeID=""N57c461361fc640adaac6bcb1c7f54d0e"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/11826b3e-ce40-4097-a738-532561209b40"">
						<roh:title rdf:datatype=""&xsd;string"" >The DiY Smart Experiences Project - A European Endeavour Removing Barriers for User-generated Internet of Things Applications.</roh:title>
						<bibo:authorList rdf:nodeID=""N58dbc92831344c0b812c8a7d202a2399"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/feb48b56-ac2a-4e52-aa81-c39f30dc18ae"">
						<roh:title rdf:datatype=""&xsd;string"" >'Close the Loop': An iBeacon App to Foster Recycling Through Just-in-Time Feedback.</roh:title>
						<bibo:authorList rdf:nodeID=""N5a2ca30985bd4c4786717981a299f788"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/a7db2203-49a1-4bb5-aac4-cda161c1468f"">
						<roh:title rdf:datatype=""&xsd;string"" >To switch off the coffee-maker or not: that is the question to be energy-efficient at work.</roh:title>
						<bibo:authorList rdf:nodeID=""N5680b4d50ff4415883a761cbba34d2f7"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/17d68fc4-0aa8-4ecd-a570-2946c42c5ade"">
						<roh:title rdf:datatype=""&xsd;string"" >Validation of a CoAP to IEC 61850 Mapping and Benchmarking vs HTTP-REST and WS-SOAP.</roh:title>
						<bibo:authorList rdf:nodeID=""N5549bfe4dfdc496fafd47f3e77f64d7a"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/86769078-ccc8-47dd-83aa-a472666b262f"">
						<roh:title rdf:datatype=""&xsd;string"" >Design-insights for Devising Persuasive IoT Devices for Sustainability in the Workplace.</roh:title>
						<bibo:authorList rdf:nodeID=""N5c342df56893442da790bd591df1c245"" />
					</bibo:Document>
					<rdf:Seq rdf:nodeID=""N57c461361fc640adaac6bcb1c7f54d0d"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
						<rdf:_2 rdf:resource=""http://graph.um.es/res/person/e12d2a31-e285-459a-9d71-2d4a45329532"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""N59d40eb0e46c40d083cf88d019323568"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
						<rdf:_2 rdf:resource=""http://graph.um.es/res/person/e12d2a31-e285-459a-9d71-2d4a45329532"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""N55f7938ae6494843a9622da4446bc608"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
						<rdf:_2 rdf:resource=""http://graph.um.es/res/person/e12d2a31-e285-459a-9d71-2d4a45329532"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""N57c461361fc640adaac6bcb1c7f54d0c"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
						<rdf:_2 rdf:resource=""http://graph.um.es/res/person/799860d8-b7c5-4229-a4c3-44533871493d"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""N57c461361fc640adaac6bcb1c7f54d22"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
						<rdf:_2 rdf:resource=""http://graph.um.es/res/person/7a4898f2-195c-42d0-98c6-91ba121c14ea"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""N57c461361fc640adaac6bcb1c7f54d0e"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
						<rdf:_2 rdf:resource=""http://graph.um.es/res/person/27b9fd2f-386c-4da6-bfe2-ce4fc6ace6a4"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""N58dbc92831344c0b812c8a7d202a2399"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""N5a2ca30985bd4c4786717981a299f788"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""N5680b4d50ff4415883a761cbba34d2f7"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""N5549bfe4dfdc496fafd47f3e77f64d7a"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""N5c342df56893442da790bd591df1c245"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
					</rdf:Seq>
				</rdf:RDF>
				";
				return rdfFile;
			}


		}


		private string rdfFileReconciliateIDs
		{
			get
			{
				string rdfFile = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE rdf:RDF [
        	<!ENTITY rdf 'http://www.w3.org/1999/02/22-rdf-syntax-ns#'>
		<!ENTITY rdfs 'http://www.w3.org/2000/01/rdf-schema#'>
			<!ENTITY xsd 'http://www.w3.org/2001/XMLSchema#'>
				]>
				<rdf:RDF xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#""
       					xmlns:xsd=""http://www.w3.org/2001/XMLSchema#""
       					xmlns:foaf=""http://purl.org/roh/mirror/foaf#""
       					xmlns:roh=""http://purl.org/roh#""
       					xmlns:bibo=""http://purl.org/roh/mirror/bibo#""
       					xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"">
					<foaf:Person rdf:about=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"">
						<foaf:name rdf:datatype=""&xsd;string"">Diego Casado-Mansilla</foaf:name>
						<roh:ORCID rdf:datatype=""&xsd;string"">1234</roh:ORCID>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/76c8f70c-50ea-4de0-b3b6-5bcf4fdb829e""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/eec217c9-44c4-480d-9653-6d84b711a9b2""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/990d9d14-4cbb-4387-8220-5a6d5a43f2a0""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/5d17fc89-8c73-4157-965c-bfacc6030171""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/584fc505-dbca-49f8-ac30-be7b44fc1957""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/3844c942-79ab-443c-bf59-4f74b2f1edeb""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/11826b3e-ce40-4097-a738-532561209b40""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/feb48b56-ac2a-4e52-aa81-c39f30dc18ae""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/a7db2203-49a1-4bb5-aac4-cda161c1468f""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/17d68fc4-0aa8-4ecd-a570-2946c42c5ade""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/86769078-ccc8-47dd-83aa-a472666b262f""/>
					</foaf:Person>
					<foaf:Person rdf:about=""http://graph.um.es/res/person/e12d2a31-e285-459a-9d71-2d4a45329532"">
						<foaf:name rdf:datatype=""&xsd;string"">Diego López-de-Ipiña</foaf:name>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/76c8f70c-50ea-4de0-b3b6-5bcf4fdb829e""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/eec217c9-44c4-480d-9653-6d84b711a9b2""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/990d9d14-4cbb-4387-8220-5a6d5a43f2a0""/>
					</foaf:Person>
					<foaf:Person rdf:about=""http://graph.um.es/res/person/aaad2a31-e285-459a-9d71-2d4a45329532"">
						<foaf:name rdf:datatype=""&xsd;string"">D. López-de-Ipiña</foaf:name>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/76c8f70c-50ea-4de0-b3b6-5bcf4fdb829e""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/eec217c9-44c4-480d-9653-6d84b711a9b2""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/990d9d14-4cbb-4387-8220-5a6d5a43f2a0""/>
					</foaf:Person>
					<foaf:Person rdf:about=""http://graph.um.es/res/person/799860d8-b7c5-4229-a4c3-44533871493d"">
						<foaf:name rdf:datatype=""&xsd;string"">Álvaro Palacios Marijuán</foaf:name>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/5d17fc89-8c73-4157-965c-bfacc6030171""/>
					</foaf:Person>
					<foaf:Person rdf:about=""http://graph.um.es/res/person/7a4898f2-195c-42d0-98c6-91ba121c14ea"">
						<foaf:name rdf:datatype=""&xsd;string"">Esteban Sota Leiva</foaf:name>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/584fc505-dbca-49f8-ac30-be7b44fc1957""/>
					</foaf:Person>
					<foaf:Person rdf:about=""http://graph.um.es/res/person/27b9fd2f-386c-4da6-bfe2-ce4fc6ace6a4"">
						<foaf:name rdf:datatype=""&xsd;string"">Oihane Gómez-Carmona</foaf:name>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/3844c942-79ab-443c-bf59-4f74b2f1edeb""/>
					</foaf:Person>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/76c8f70c-50ea-4de0-b3b6-5bcf4fdb829e"">
						<roh:title rdf:datatype=""&xsd;string"" >Exploring the computational cost of machine learning at the edge for human-centric Internet of Things.</roh:title>
						<bibo:authorList rdf:nodeID=""N57c461361fc640adaac6bcb1c7f54d0d"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/eec217c9-44c4-480d-9653-6d84b711a9b2"">
						<roh:title rdf:datatype=""&xsd;string"" >User perspectives in the design of interactive everyday objects for sustainable behaviour.</roh:title>
						<bibo:authorList rdf:nodeID=""N59d40eb0e46c40d083cf88d019323568"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/990d9d14-4cbb-4387-8220-5a6d5a43f2a0"">
						<roh:title rdf:datatype=""&xsd;string"" >Exploring the Application of the FOX Model to Foster Pro-Environmental Behaviours in Smart Environments.</roh:title>
						<bibo:authorList rdf:nodeID=""N55f7938ae6494843a9622da4446bc608"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/5d17fc89-8c73-4157-965c-bfacc6030171"">
						<roh:title rdf:datatype=""&xsd;string"" >Documento de prueba de Álvaro y Diego</roh:title>
						<bibo:authorList rdf:nodeID=""N57c461361fc640adaac6bcb1c7f54d0c"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/584fc505-dbca-49f8-ac30-be7b44fc1957"">
						<roh:title rdf:datatype=""&xsd;string"" >Documento de prueba con título inventado</roh:title>
						<bibo:authorList rdf:nodeID=""N57c461361fc640adaac6bcb1c7f54d22"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/3844c942-79ab-443c-bf59-4f74b2f1edeb"">
						<roh:title rdf:datatype=""&xsd;string"" >SmartWorkplace: A Privacy-based Fog Computing Approach to Boost Energy Efficiency and Wellness in Digital Workspaces.</roh:title>
						<bibo:authorList rdf:nodeID=""N57c461361fc640adaac6bcb1c7f54d0e"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/11826b3e-ce40-4097-a738-532561209b40"">
						<roh:title rdf:datatype=""&xsd;string"" >The DiY Smart Experiences Project - A European Endeavour Removing Barriers for User-generated Internet of Things Applications.</roh:title>
						<bibo:authorList rdf:nodeID=""N58dbc92831344c0b812c8a7d202a2399"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/feb48b56-ac2a-4e52-aa81-c39f30dc18ae"">
						<roh:title rdf:datatype=""&xsd;string"" >'Close the Loop': An iBeacon App to Foster Recycling Through Just-in-Time Feedback.</roh:title>
						<bibo:authorList rdf:nodeID=""N5a2ca30985bd4c4786717981a299f788"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/a7db2203-49a1-4bb5-aac4-cda161c1468f"">
						<roh:title rdf:datatype=""&xsd;string"" >To switch off the coffee-maker or not: that is the question to be energy-efficient at work.</roh:title>
						<bibo:authorList rdf:nodeID=""N5680b4d50ff4415883a761cbba34d2f7"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/17d68fc4-0aa8-4ecd-a570-2946c42c5ade"">
						<roh:title rdf:datatype=""&xsd;string"" >Validation of a CoAP to IEC 61850 Mapping and Benchmarking vs HTTP-REST and WS-SOAP.</roh:title>
						<bibo:authorList rdf:nodeID=""N5549bfe4dfdc496fafd47f3e77f64d7a"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/86769078-ccc8-47dd-83aa-a472666b262f"">
						<roh:title rdf:datatype=""&xsd;string"" >Design-insights for Devising Persuasive IoT Devices for Sustainability in the Workplace.</roh:title>
						<bibo:authorList rdf:nodeID=""N5c342df56893442da790bd591df1c245"" />
					</bibo:Document>
					<rdf:Seq rdf:nodeID=""N57c461361fc640adaac6bcb1c7f54d0d"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
						<rdf:_2 rdf:resource=""http://graph.um.es/res/person/e12d2a31-e285-459a-9d71-2d4a45329532"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""N59d40eb0e46c40d083cf88d019323568"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
						<rdf:_2 rdf:resource=""http://graph.um.es/res/person/e12d2a31-e285-459a-9d71-2d4a45329532"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""N55f7938ae6494843a9622da4446bc608"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
						<rdf:_2 rdf:resource=""http://graph.um.es/res/person/e12d2a31-e285-459a-9d71-2d4a45329532"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""N57c461361fc640adaac6bcb1c7f54d0c"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
						<rdf:_2 rdf:resource=""http://graph.um.es/res/person/799860d8-b7c5-4229-a4c3-44533871493d"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""N57c461361fc640adaac6bcb1c7f54d22"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
						<rdf:_2 rdf:resource=""http://graph.um.es/res/person/7a4898f2-195c-42d0-98c6-91ba121c14ea"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""N57c461361fc640adaac6bcb1c7f54d0e"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
						<rdf:_2 rdf:resource=""http://graph.um.es/res/person/27b9fd2f-386c-4da6-bfe2-ce4fc6ace6a4"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""N58dbc92831344c0b812c8a7d202a2399"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""N5a2ca30985bd4c4786717981a299f788"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""N5680b4d50ff4415883a761cbba34d2f7"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""N5549bfe4dfdc496fafd47f3e77f64d7a"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""N5c342df56893442da790bd591df1c245"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/c8a16863-a606-48e3-a858-9def000380c0"" />
					</rdf:Seq>
				</rdf:RDF>
				";
				return rdfFile;
			}


		}

		private string databaseFileReconciliateIds
		{
			get
			{
				string rdfFile = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE rdf:RDF [
        	<!ENTITY rdf 'http://www.w3.org/1999/02/22-rdf-syntax-ns#'>
		<!ENTITY rdfs 'http://www.w3.org/2000/01/rdf-schema#'>
			<!ENTITY xsd 'http://www.w3.org/2001/XMLSchema#'>
				]>
				<rdf:RDF xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#""
       					xmlns:xsd=""http://www.w3.org/2001/XMLSchema#""
       					xmlns:foaf=""http://purl.org/roh/mirror/foaf#""
       					xmlns:roh=""http://purl.org/roh#""
       					xmlns:bibo=""http://purl.org/roh/mirror/bibo#""
       					xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"">
					<foaf:Person rdf:about=""http://graph.um.es/res/person/aaa16863-a606-48e3-a858-9def000380c0"">
						<foaf:name rdf:datatype=""&xsd;string"">Diego Casado-Mansilla</foaf:name>
						<roh:ORCID rdf:datatype=""&xsd;string"">1234</roh:ORCID>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/aac8f70c-50ea-4de0-b3b6-5bcf4fdb829e""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/aac217c9-44c4-480d-9653-6d84b711a9b2""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/aa0d9d14-4cbb-4387-8220-5a6d5a43f2a0""/>
					</foaf:Person>
					<foaf:Person rdf:about=""http://graph.um.es/res/person/aa2d2a31-e285-459a-9d71-2d4a45329532"">
						<foaf:name rdf:datatype=""&xsd;string"">Diego López-de-Ipiña</foaf:name>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/aac8f70c-50ea-4de0-b3b6-5bcf4fdb829e""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/aac217c9-44c4-480d-9653-6d84b711a9b2""/>
						<roh:correspondingAuthorOf rdf:resource=""http://graph.um.es/res/document/aa0d9d14-4cbb-4387-8220-5a6d5a43f2a0""/>
					</foaf:Person>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/aac8f70c-50ea-4de0-b3b6-5bcf4fdb829e"">
						<roh:title rdf:datatype=""&xsd;string"" >Exploring the computational cost of machine learning at the edge for human-centric Internet of Things.</roh:title>
						<bibo:authorList rdf:nodeID=""Naac461361fc640adaac6bcb1c7f54d0d"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/aac217c9-44c4-480d-9653-6d84b711a9b2"">
						<roh:title rdf:datatype=""&xsd;string"" >User perspectives in the design of interactive everyday objects for sustainable behaviour.</roh:title>
						<bibo:authorList rdf:nodeID=""Naad40eb0e46c40d083cf88d019323568"" />
					</bibo:Document>
					<bibo:Document rdf:about=""http://graph.um.es/res/document/aa0d9d14-4cbb-4387-8220-5a6d5a43f2a0"">
						<roh:title rdf:datatype=""&xsd;string"" >Exploring the Application of the FOX Model to Foster Pro-Environmental Behaviours in Smart Environments.</roh:title>
						<bibo:authorList rdf:nodeID=""Naaf7938ae6494843a9622da4446bc608"" />
					</bibo:Document>
					<rdf:Seq rdf:nodeID=""Naac461361fc640adaac6bcb1c7f54d0d"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/aaa16863-a606-48e3-a858-9def000380c0"" />
						<rdf:_2 rdf:resource=""http://graph.um.es/res/person/aa2d2a31-e285-459a-9d71-2d4a45329532"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""Naad40eb0e46c40d083cf88d019323568"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/aaa16863-a606-48e3-a858-9def000380c0"" />
						<rdf:_2 rdf:resource=""http://graph.um.es/res/person/aa2d2a31-e285-459a-9d71-2d4a45329532"" />
					</rdf:Seq>
					<rdf:Seq rdf:nodeID=""Naaf7938ae6494843a9622da4446bc608"">
						<rdf:_1 rdf:resource=""http://graph.um.es/res/person/aaa16863-a606-48e3-a858-9def000380c0"" />
						<rdf:_2 rdf:resource=""http://graph.um.es/res/person/aa2d2a31-e285-459a-9d71-2d4a45329532"" />
					</rdf:Seq>
				</rdf:RDF>
				";
				return rdfFile;
			}
		}

	}
}
