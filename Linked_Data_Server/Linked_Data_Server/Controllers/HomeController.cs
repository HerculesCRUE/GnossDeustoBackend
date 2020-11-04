using Linked_Data_Server.Models;
using Linked_Data_Server.Models.Entities;
using Linked_Data_Server.Models.Services;
using Linked_Data_Server.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Writing;
using Microsoft.AspNetCore.Http.Extensions;
using System.Linq;
using System.Net.Http;

namespace Linked_Data_Server.Controllers
{

    public class HomeController : Controller
    {
        private readonly static ConfigService mConfigService = new ConfigService();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        [Produces("application/rdf+xml", "text/html")]
        public IActionResult Index()
        {
            //Obtenemos la URL de la entidad
            string url = Request.GetDisplayUrl();

            //Customizamos Header
            HttpContext.Response.Headers.Add("Link", "<http://www.w3.org/ns/ldp#BasicContainer>; rel=\"type\", <http://www.w3.org/ns/ldp#Resource>; rel=\"type\", <" + mConfigService.GetConstrainedByUrl() + ">; rel=\"http://www.w3.org/ns/ldp#constrainedBy\"");
            HashSet<string> methodsAvailable = new HashSet<string>() { "GET", "HEAD", "OPTIONS" };
            HttpContext.Response.Headers.Add("allow", string.Join(", ", methodsAvailable));
            if (!methodsAvailable.Contains(Request.HttpContext.Request.Method))
            {
                return StatusCode(StatusCodes.Status405MethodNotAllowed);
            }


            //Cargamos la ontología
            RohGraph ontologyGraph = new RohGraph();
            ontologyGraph.LoadFromFile("Config/Ontology/roh-v2.owl");
            SparqlResultSet sparqlResultSetNombresPropiedades = (SparqlResultSet)ontologyGraph.ExecuteQuery("select distinct ?entidad ?nombre where { ?entidad <http://www.w3.org/2000/01/rdf-schema#label> ?nombre. FILTER(lang(?nombre) = 'es')}");

            //Guardamos todos los nombres de las propiedades en un diccionario
            Dictionary<string, string> communNamePropierties = new Dictionary<string, string>();
            foreach (SparqlResult sparqlResult in sparqlResultSetNombresPropiedades.Results)
            {
                communNamePropierties.Add(sparqlResult["entidad"].ToString(), ((LiteralNode)(sparqlResult["nombre"])).Value);
            }

            //Cargamos las entidades propias
            Dictionary<string, SparqlObject> sparqlObjectDictionary = GetEntityData(url);
            if (sparqlObjectDictionary.Count == 1 && sparqlObjectDictionary[url].results.bindings.Count == 0)
            {
                //No existe la entidad
                return StatusCode(StatusCodes.Status404NotFound);
            }
            //Cargamos los datos en un grafo en Local
            RohGraph dataGraph = createDataGraph(url, new List<string>(), false, new RohGraph(), sparqlObjectDictionary);


            //Generamos el RDF
            System.IO.StringWriter sw = new System.IO.StringWriter();
            RdfXmlWriter rdfXmlWriter = new RdfXmlWriter();
            rdfXmlWriter.Save(dataGraph, sw);
            string rdf = sw.ToString();

            //Añadimos la etiquetqa ETag al header
            using (SHA256 sha256Hash = SHA256.Create())
            {
                string etag = GetHash(sha256Hash, rdf);
                string ifNoneMatch = HttpContext.Request.Headers["If-None-Match"];
                if (ifNoneMatch == etag)
                {
                    HttpContext.Response.StatusCode = 304;
                }
                HttpContext.Response.Headers.Add("ETag", etag);
            }

            if (HttpContext.Request.ContentType == "application/rdf+xml")
            {
                //Devolvemos en formato RDF
                return File(Encoding.UTF8.GetBytes(rdf), "text/xml");
            }
            else
            {
                //Obtenemos las 10 primeras entidades que apuntan a la entidad
                List<string> inverseEntities = GetInverseEntities(url, 10);
                foreach (string inverseEntity in inverseEntities)
                {
                    Dictionary<string, SparqlObject> sparqlObjectDictionaryInverse = GetEntityData(inverseEntity);
                    RohGraph inverseDataGraph = createDataGraph(inverseEntity, new List<string>(), false, new RohGraph(), sparqlObjectDictionaryInverse);
                    dataGraph.Merge(inverseDataGraph);
                }


                //Devolvemos en formato HTML
                List<String> allEntities = new List<string>();
                SparqlResultSet sparqlResultSetEntidades = (SparqlResultSet)dataGraph.ExecuteQuery("select distinct ?p ?o where { ?s ?p ?o. FILTER (!isBlank(?o)) }");
                foreach (SparqlResult sparqlResult in sparqlResultSetEntidades.Results)
                {
                    if ((sparqlResult["o"] is UriNode) && (sparqlResult["p"].ToString() != "http://www.w3.org/1999/02/22-rdf-syntax-ns#type"))
                    {
                        allEntities.Add(sparqlResult["o"].ToString());
                    }
                }

                //Preparamos el modelo de la entidad principal
                List<DiscoverRdfViewModel> modelEntities = new List<DiscoverRdfViewModel>();
                DiscoverRdfViewModel entidad = createDiscoverRdfViewModel(url, dataGraph, new List<string>(), allEntities, communNamePropierties);
                modelEntities.Add(entidad);
                KeyValuePair<string, List<string>> titulo = entidad.stringPropertiesEntity.FirstOrDefault(x => x.Key == "http://purl.org/roh#title" || x.Key == "http://purl.org/roh/mirror/foaf#name");
                ViewData["Title"] = "About: " + url;
                if (titulo.Key != null)
                {
                    ViewData["Title"] = "About: " + titulo.Value[0];
                }
                ViewData["NameTitle"] = mConfigService.GetNameTitle();

                //Preparamos el modelo del resto de entidades
                foreach (string entity in inverseEntities)
                {
                    DiscoverRdfViewModel entidadInversa = createDiscoverRdfViewModel(entity, dataGraph, new List<string>(), allEntities, communNamePropierties);
                    modelEntities.Add(entidadInversa);
                }


                return View(modelEntities);
            }
        }

        private Dictionary<string, SparqlObject> GetEntityData(string pEntity)
        {
            Dictionary<string, SparqlObject> sparqlObjectDictionary = new Dictionary<string, SparqlObject>();
            HashSet<string> entidadesCargar = new HashSet<string>() { pEntity };
            HashSet<string> entidadesCargadas = new HashSet<string>() { pEntity };
            while (entidadesCargar.Count > 0)
            {
                string consulta = "select ?s ?p ?o isBlank(?o) as ?blanknode where { ?s ?p ?o. FILTER(?s in(<>,<" + string.Join(">,<", entidadesCargar) + ">))}";
                SparqlObject sparqlObject = SparqlUtility.SelectData(mConfigService.GetSparqlEndpoint(), mConfigService.GetSparqlGraph(), consulta, mConfigService.GetSparqlQueryParam());
                foreach (string pendiente in entidadesCargar)
                {
                    sparqlObjectDictionary.Add(pendiente, sparqlObject);
                }
                entidadesCargar = new HashSet<string>();
                foreach (Dictionary<string, SparqlObject.Data> row in sparqlObject.results.bindings)
                {
                    if (row["blanknode"].value == "1" && !entidadesCargadas.Contains(row["o"].value))
                    {
                        entidadesCargar.Add(row["o"].value);
                        entidadesCargadas.Add(row["o"].value);
                    }
                }
            }
            return sparqlObjectDictionary;
        }

        private List<string> GetInverseEntities(string pEntity, int pMax)
        {
            //Cargamos las primeras 10 entidades (que no sen blanknodes) que apuntan a la entidad            
            Dictionary<string, bool> entitiesInverseBN = new Dictionary<string, bool>();
            HashSet<string> nodesEntitiesLoaded = new HashSet<string>();
            string consulta = "select distinct ?s isBlank(?s) as ?blanknode where { ?s ?p ?o.  FILTER(?o in(<" + pEntity + ">))} limit " + pMax;
            SparqlObject sparqlObject = SparqlUtility.SelectData(mConfigService.GetSparqlEndpoint(), mConfigService.GetSparqlGraph(), consulta, mConfigService.GetSparqlQueryParam());
            foreach (Dictionary<string, SparqlObject.Data> row in sparqlObject.results.bindings)
            {
                entitiesInverseBN.Add(row["s"].value, row["blanknode"].value == "1");
            }
            while (entitiesInverseBN.Where(x => x.Value == true).Count() > 0)
            {
                consulta = "select distinct ?s isBlank(?s) as ?blanknode where { ?s ?p ?o.  FILTER(?o in(<>,<" + string.Join(">,<", entitiesInverseBN.Where(x => x.Value == true).ToList().Select(x => x.Key)) + ">))}";

                entitiesInverseBN = entitiesInverseBN.Where(pair => pair.Value == false)
                                     .ToDictionary(pair => pair.Key,
                                                 pair => pair.Value);
                sparqlObject = SparqlUtility.SelectData(mConfigService.GetSparqlEndpoint(), mConfigService.GetSparqlGraph(), consulta, mConfigService.GetSparqlQueryParam());
                foreach (Dictionary<string, SparqlObject.Data> row in sparqlObject.results.bindings)
                {
                    if (!nodesEntitiesLoaded.Contains(row["s"].value))
                    {
                        entitiesInverseBN[row["s"].value] = row["blanknode"].value == "1";
                    }
                    nodesEntitiesLoaded.Add(row["s"].value);
                }
            }
            entitiesInverseBN.Remove(pEntity);
            return entitiesInverseBN.Keys.ToList();
        }


        /// <summary>
        /// Crea un grafo en el que se cargan los datos
        /// </summary>
        /// <param name="idEntity">Identificador de la entidad</param>
        /// <param name="parents">Lista de ancestros de la entidad</param>
        /// <param name="isBlank">Booleano que indica si la entidad es un blank node</param>
        /// <param name="datagraph">Grafo donde se cargan los datos</param>
        /// <returns></returns>
        public RohGraph createDataGraph(string idEntity, List<string> parents, Boolean isBlank, RohGraph datagraph, Dictionary<string, SparqlObject> sparqlObject)
        {
            foreach (Dictionary<string, SparqlObject.Data> row in sparqlObject[idEntity].results.bindings)
            {
                if (!isBlank)
                {
                    IUriNode s = datagraph.CreateUriNode(UriFactory.Create(idEntity));
                    IUriNode p = datagraph.CreateUriNode(UriFactory.Create(row["p"].value));

                    if (row["o"].type == "bnode" && !parents.Contains(row["o"].value))
                    {
                        IBlankNode o = datagraph.CreateBlankNode(row["o"].value);
                        datagraph.Assert(new Triple(s, p, o));
                        parents.Add(row["o"].value);
                        createDataGraph(row["o"].value, parents, true, datagraph, sparqlObject);
                    }
                    else if (row["o"].type == "typed-literal")
                    {
                        ILiteralNode o = datagraph.CreateLiteralNode(row["o"].value, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                        datagraph.Assert(new Triple(s, p, o));
                    }
                    else
                    {
                        IUriNode o = datagraph.CreateUriNode(UriFactory.Create(row["o"].value));
                        datagraph.Assert(new Triple(s, p, o));
                    }
                }
                else
                {
                    if (row["s"].value == idEntity)
                    {
                        IBlankNode s = datagraph.CreateBlankNode(idEntity);
                        IUriNode p = datagraph.CreateUriNode(UriFactory.Create(row["p"].value));

                        if (row["o"].type == "bnode")
                        {
                            if (!parents.Contains(row["o"].value))
                            {
                                IBlankNode o = datagraph.CreateBlankNode(row["o"].value);
                                datagraph.Assert(new Triple(s, p, o));
                                parents.Add(row["o"].value);
                                createDataGraph(row["o"].value, parents, true, datagraph, sparqlObject);
                            }
                            else
                            {
                                IBlankNode o = datagraph.CreateBlankNode(row["o"].value);
                                datagraph.Assert(new Triple(s, p, o));
                            }
                        }
                        else if (row["o"].type == "typed-literal")
                        {
                            ILiteralNode o = datagraph.CreateLiteralNode(row["o"].value, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                            datagraph.Assert(new Triple(s, p, o));
                        }
                        else
                        {
                            IUriNode o = datagraph.CreateUriNode(UriFactory.Create(row["o"].value));
                            datagraph.Assert(new Triple(s, p, o));
                        }
                    }

                }
            }
            return datagraph;
        }

        /// <summary>
        /// Crea un modelo DiscoverRdfViewModel
        /// </summary>
        /// <param name="idEntity">Identificador de la entidad de la que crear el modelo</param>
        /// <param name="dataGraph">Grafo que contiene los datos</param>
        /// <param name="parents">Lista de ancestros de la entidad</param>
        /// <param name="allEntities">Listado con todos los identificadores del RDF</param>
        /// <param name="communNameProperties">Diccionario con los nombres de las propiedades</param>
        /// <returns></returns>
        public DiscoverRdfViewModel createDiscoverRdfViewModel(string idEntity, RohGraph dataGraph, List<string> parents, List<string> allEntities, Dictionary<string, string> communNameProperties)
        {
            //Obtenemos todos los triples de la entidad
            SparqlResultSet sparqlResultSet = (SparqlResultSet)dataGraph.ExecuteQuery("select ?p ?o where { <" + idEntity + "> ?p ?o }");
            DiscoverRdfViewModel entidad = new DiscoverRdfViewModel();
            entidad.stringPropertiesEntity = new Dictionary<string, List<string>>();
            entidad.entitiesPropertiesEntity = new Dictionary<string, List<DiscoverRdfViewModel>>();
            entidad.uriEntity = idEntity;
            entidad.urisRdf = allEntities;
            entidad.communNamePropierties = communNameProperties;

            foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
            {
                if (sparqlResult["o"] is BlankNode && !parents.Contains(sparqlResult["o"].ToString()))
                {
                    if (!entidad.entitiesPropertiesEntity.ContainsKey(sparqlResult["p"].ToString()))
                    {
                        //Añadimos la propiedad a 'entitiesPropertiesEntity'
                        entidad.entitiesPropertiesEntity.Add(sparqlResult["p"].ToString(), new List<DiscoverRdfViewModel>());
                    }
                    parents.Add(idEntity);
                    entidad.entitiesPropertiesEntity[sparqlResult["p"].ToString()].Add(createDiscoverRdfViewModel(sparqlResult["o"].ToString(), dataGraph, parents, allEntities, communNameProperties));
                }
                else
                {
                    if (!entidad.stringPropertiesEntity.ContainsKey(sparqlResult["p"].ToString()))
                    {
                        //Añadimos la propiedad a 'stringPropertiesEntity'
                        entidad.stringPropertiesEntity.Add(sparqlResult["p"].ToString(), new List<string>());
                    }
                    if (sparqlResult["o"] is LiteralNode)
                    {
                        entidad.stringPropertiesEntity[sparqlResult["p"].ToString()].Add(((LiteralNode)(sparqlResult["o"])).Value);
                    }
                    else
                    {
                        entidad.stringPropertiesEntity[sparqlResult["p"].ToString()].Add(sparqlResult["o"].ToString());
                    }
                }
            }
            return entidad;

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}
