using Hercules.Asio.LinkedDataServer.Utility;
using Linked_Data_Server.Models;
using Linked_Data_Server.Models.Entities;
using Linked_Data_Server.Models.Services;
using Linked_Data_Server.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Query.Inference;
using VDS.RDF.Writing;

namespace Linked_Data_Server.Controllers
{

    public class HomeController : Controller
    {
        private readonly static ConfigService mConfigService = new ConfigService();
        private readonly ILogger<HomeController> _logger;
        private readonly static Config_Linked_Data_Server mLinked_Data_Server_Config = LoadLinked_Data_Server_Config();
        private readonly ISparqlUtility _sparqlUtility;
        private static RohGraph ontologyGraph;

        public HomeController(ISparqlUtility sparqlUtility, ILogger<HomeController> logger)
        {
            _logger = logger;
            _sparqlUtility = sparqlUtility;
        }

        [Produces("application/rdf+xml", "text/html")]
        public IActionResult Index()
        {
            string pXAppServer = "";
            //Obtenemos la URL de la entidad
            string url = Request.GetEncodedUrl();
            //string url = Request.GetDisplayUrl();
            string urlParam = HttpUtility.ParseQueryString(Request.QueryString.Value).Get("url");
            if (!string.IsNullOrEmpty(urlParam))
            {
                url = urlParam;
            }
            EntityModelTemplate model = GenerarModeloDeUrl(url, ref pXAppServer);
            if (model.Status405)
            {
                return StatusCode(StatusCodes.Status405MethodNotAllowed);
            }
            else if (!string.IsNullOrEmpty(model.Rdf))
            {
                return File(Encoding.UTF8.GetBytes(model.Rdf), "application/rdf+xml");
            }
            return View(model);
        }

        public EntityModelTemplate GenerarModeloDeUrl(string url, ref string pXAppServer)
        {
            ViewBag.UrlHome = mConfigService.GetUrlHome();
            EntityModelTemplate entityModel = new EntityModelTemplate();

            //Customizamos Header
            if (!string.IsNullOrEmpty(mConfigService.GetConstrainedByUrl()) && HttpContext != null)
            {
                HttpContext.Response.Headers.Add("Link", "<http://www.w3.org/ns/ldp#BasicContainer>; rel=\"type\", <http://www.w3.org/ns/ldp#Resource>; rel=\"type\", <" + mConfigService.GetConstrainedByUrl() + ">; rel=\"http://www.w3.org/ns/ldp#constrainedBy\"");
            }
            else if (HttpContext != null)
            {
                HttpContext.Response.Headers.Add("Link", "<http://www.w3.org/ns/ldp#BasicContainer>; rel=\"type\", <http://www.w3.org/ns/ldp#Resource>; rel=\"type\"");
            }
            HashSet<string> methodsAvailable = new HashSet<string>() { "GET", "HEAD", "OPTIONS" };
            if (HttpContext != null)
            {
                HttpContext.Response.Headers.Add("allow", string.Join(", ", methodsAvailable));
            }
            if (Request != null && !methodsAvailable.Contains(Request.HttpContext.Request.Method))
            {
                entityModel.Status405 = true;
            }

            //Cargamos la ontología y obtenemos la afinidad
            RohGraph ontologyGraph = LoadGraph(mConfigService.GetOntologyGraph(), mConfigService, ref pXAppServer);
            SparqlResultSet sparqlResultSetNombresPropiedades = (SparqlResultSet)ontologyGraph.ExecuteQuery(@"select distinct ?entidad ?nombre lang(?nombre) as ?lang where 
                                                                                                            { 
                                                                                                                ?entidad <http://www.w3.org/2000/01/rdf-schema#label> ?nombre. 
                                                                                                            }");

            //Guardamos todos los nombres de las propiedades en un diccionario
            Dictionary<string, string> communNamePropierties = new Dictionary<string, string>();
            foreach (SparqlResult sparqlResult in sparqlResultSetNombresPropiedades.Results)
            {
                string entity = sparqlResult["entidad"].ToString();
                if (!communNamePropierties.ContainsKey(entity))
                {
                    List<SparqlResult> filas = sparqlResultSetNombresPropiedades.Results.Where(x => x["entidad"].ToString() == entity).ToList();
                    if (filas.FirstOrDefault(x => x["lang"].ToString() == "es") != null)
                    {
                        communNamePropierties[entity] = ((LiteralNode)filas.FirstOrDefault(x => x["lang"].ToString() == "es")["nombre"]).Value.ToString();
                    }
                    else if (filas.FirstOrDefault(x => x["lang"].ToString() == "en") != null)
                    {
                        communNamePropierties[entity] = ((LiteralNode)filas.FirstOrDefault(x => x["lang"].ToString() == "en")["nombre"]).Value.ToString();
                    }
                    else if (filas.FirstOrDefault(x => string.IsNullOrEmpty(x["lang"].ToString())) != null)
                    {
                        communNamePropierties[entity] = ((LiteralNode)filas.FirstOrDefault(x => string.IsNullOrEmpty(x["lang"].ToString()))["nombre"]).Value.ToString();
                    }
                }
            }

            //Cargamos las entidades propias
            Dictionary<string, string> entitiesNames;
            Dictionary<string, SparqlObject> sparqlObjectDictionary = GetEntityData(url, out entitiesNames, mConfigService, ref pXAppServer);
            if (sparqlObjectDictionary.Count == 1 && sparqlObjectDictionary[url].results.bindings.Count == 0)
            {
                //No existe la entidad
                HttpContext.Response.StatusCode = 404;
                ViewData["Title"] = "Error 404 página no encontrada para la entidad " + url;
                ViewData["NameTitle"] = mConfigService.GetNameTitle();
                return new EntityModelTemplate();
            }
            else
            {
                //Cargamos los datos en un grafo en Local
                RohGraph dataGraph = new RohGraph();
                createDataGraph(url, new List<string>(), false, dataGraph, sparqlObjectDictionary);

                //Generamos el RDF
                System.IO.StringWriter sw = new System.IO.StringWriter();
                RdfXmlWriter rdfXmlWriter = new RdfXmlWriter();
                rdfXmlWriter.Save(dataGraph, sw);
                string rdf = sw.ToString();

                Microsoft.Extensions.Primitives.StringValues stringvalues;
                if (HttpContext != null)
                {
                    HttpContext.Request.Headers.TryGetValue("accept", out stringvalues);
                }
                if (stringvalues == "application/rdf+xml" && HttpContext != null)
                {
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

                    //Devolvemos en formato RDF
                    entityModel.Rdf = rdf;
                }
                else
                {
                    RohRdfsReasoner reasoner = new RohRdfsReasoner();
                    reasoner.Initialise(ontologyGraph);
                    RohGraph dataInferenceGraph = dataGraph.Clone();
                    reasoner.Apply(dataInferenceGraph);

                    //Obtenemos datos del resto de grafos (para los provenance)
                    Dictionary<string, List<Dictionary<string, SparqlObject.Data>>> sparqlObjectDictionaryGraphs = GetEntityDataGraphs(url, mConfigService, ref pXAppServer);

                    //Obtenemos las tablas configuradas
                    List<Table> dataTables = GetDataTables(dataInferenceGraph, url, mConfigService, ref pXAppServer);

                    //Obtenemos los arborGrah configurados
                    List<ArborGraph> dataArborGrahs = GetDataArborGraphs(dataInferenceGraph, dataGraph, url, mConfigService, ref pXAppServer);

                    //Obtenemos las 10 primeras entidades que apuntan a la entidad
                    HashSet<string> inverseEntities = new HashSet<string>();
                    SparqlResultSet sparqlRdfType = (SparqlResultSet)dataInferenceGraph.ExecuteQuery("select distinct ?o where {<" + url + "> a ?o. }");
                    HashSet<string> rdfTypesEntity = new HashSet<string>();
                    foreach (SparqlResult sparqlResult in sparqlRdfType.Results)
                    {
                        rdfTypesEntity.Add(sparqlResult["o"].ToString());
                    }
                    if (mLinked_Data_Server_Config.ExcludeRelatedEntity.Intersect(rdfTypesEntity).Count() == 0)
                    {
                        inverseEntities = GetInverseEntities(dataGraph, new HashSet<string>() { url }, new HashSet<string>(sparqlObjectDictionary.Keys), new Dictionary<string, SparqlObject>(), mConfigService, ref pXAppServer, 10);
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
                    List<LinkedDataRdfViewModel> modelEntities = new List<LinkedDataRdfViewModel>();
                    LinkedDataRdfViewModel entidad = createLinkedDataRdfViewModel(url, dataGraph, sparqlObjectDictionaryGraphs, new List<string>(), allEntities, communNamePropierties, entitiesNames);
                    modelEntities.Add(entidad);
                    KeyValuePair<string, List<string>> titulo = entidad.stringPropertiesEntity.FirstOrDefault(x => mLinked_Data_Server_Config.PropsTitle.Contains(x.Key));
                    ViewData["Title"] = "About: " + url;
                    if (titulo.Key != null)
                    {
                        ViewData["Title"] = "About: " + titulo.Value[0];
                    }
                    ViewData["NameTitle"] = mConfigService.GetNameTitle();

                    //Preparamos el modelo del resto de entidades
                    foreach (string entity in inverseEntities)
                    {
                        LinkedDataRdfViewModel entidadInversa = createLinkedDataRdfViewModel(entity, dataGraph, null, new List<string>(), allEntities, communNamePropierties, entitiesNames);
                        modelEntities.Add(entidadInversa);
                    }



                    entityModel.linkedDataRDF = modelEntities;
                    entityModel.propsTransform = mLinked_Data_Server_Config.PropsTransform;
                    entityModel.tables = dataTables;
                    entityModel.arborGraphs = dataArborGrahs;

                    //Añadimos la etiquetqa ETag al header
                    using (SHA256 sha256Hash = SHA256.Create())
                    {
                        string stringToHash = JsonConvert.SerializeObject(entityModel.linkedDataRDF);
                        stringToHash += JsonConvert.SerializeObject(entityModel.propsTransform);
                        stringToHash += JsonConvert.SerializeObject(entityModel.tables);
                        stringToHash += JsonConvert.SerializeObject(entityModel.arborGraphs);
                        string etag = GetHash(sha256Hash, stringToHash);
                        if (HttpContext != null)
                        {
                            string ifNoneMatch = HttpContext.Request.Headers["If-None-Match"];
                            if (ifNoneMatch == etag)
                            {
                                HttpContext.Response.StatusCode = 304;
                            }
                            HttpContext.Response.Headers.Add("ETag", etag);
                        }
                    }

                    return entityModel;
                }
            }
            return entityModel;
        }

        /// <summary>
        /// Obtiene los datos de una entidad para su pintado completo
        /// </summary>
        /// <param name="pEntity">URL de la entidad</param>
        /// <param name="pEntitiesNames">Lista con los nombres de las entidades a las que se apunta</param>
        /// <returns>Diccionario con los datos de la entidad</returns>
        private Dictionary<string, SparqlObject> GetEntityData(string pEntity, out Dictionary<string, string> pEntitiesNames, ConfigService pConfigService, ref string pXAppServer)
        {
            pEntitiesNames = new Dictionary<string, string>();
            Dictionary<string, SparqlObject> sparqlObjectDictionary = new Dictionary<string, SparqlObject>();
            HashSet<string> entidadesCargar = new HashSet<string>() { pEntity };
            HashSet<string> entidadesCargadas = new HashSet<string>() { pEntity };
            bool mock = _sparqlUtility is SparqlUtilityMock;
            while (entidadesCargar.Count > 0)
            {
                string queryAux = @$" FILTER(?s in(<>,<{ string.Join(">,<", entidadesCargar) }>)).";
                if (mock)
                {
                    if (entidadesCargar.First().StartsWith("_:"))
                    {
                        HashSet<string> entidadesCargarAux = new HashSet<string>(entidadesCargar);
                        entidadesCargar = new HashSet<string>();
                        foreach (string s in entidadesCargarAux)
                        {
                            List<Triple> listatriples = ((SparqlUtilityMock)_sparqlUtility)._dataGraph.Triples.Where(x => x.Subject.ToString() == s).ToList();
                            SparqlObject sparqlObjectAux = new SparqlObject();
                            sparqlObjectAux.results = new SparqlObject.Results();
                            sparqlObjectAux.results.bindings = new List<Dictionary<string, SparqlObject.Data>>();
                            foreach (Triple triple in listatriples)
                            {
                                Dictionary<string, SparqlObject.Data> dict = new Dictionary<string, SparqlObject.Data>();
                                {
                                    SparqlObject.Data data = new SparqlObject.Data();
                                    data.type = "bnode";
                                    data.value = triple.Subject.ToString(); 
                                    dict.Add("s", data);
                                }
                                {
                                    SparqlObject.Data data = new SparqlObject.Data();
                                    data.type = "uri";
                                    data.value = triple.Predicate.ToString();
                                    dict.Add("p", data);
                                }
                                {
                                    SparqlObject.Data data = new SparqlObject.Data();
                                    if (triple.Object is UriNode)
                                    {
                                        data.type = "uri";
                                        data.value = triple.Object.ToString();
                                    }
                                    else if (triple.Object is BlankNode)
                                    {
                                        data.type = "bnode";
                                        data.value = triple.Object.ToString();
                                    }
                                    else if (triple.Object is LiteralNode)
                                    {
                                        data.type = "literal";
                                        data.value = ((LiteralNode)(triple.Object)).Value;
                                    }
                                    dict.Add("o", data);
                                }
                                sparqlObjectAux.results.bindings.Add(dict);
                            }

                            sparqlObjectDictionary.Add(s, sparqlObjectAux);

                            List<string> oTitles = listatriples.Select(x => x.Object.ToString()).Distinct().ToList();
                            foreach (string oTitle in oTitles)
                            {
                                List<Triple> oTitleTriples = ((SparqlUtilityMock)_sparqlUtility)._dataGraph.Triples.Where(x => x.Subject.ToString() == oTitle && mLinked_Data_Server_Config.PropsTitle.Contains(x.Predicate.ToString())).ToList();
                                if (oTitleTriples.Count > 0 && oTitleTriples.First().Object is LiteralNode)
                                {
                                    string title = ((LiteralNode)(oTitleTriples.First().Object)).Value;
                                    pEntitiesNames[oTitle] = title;
                                }
                            }
                            List<string> oBlankNodes = listatriples.Where(x => x.Object.ToString().StartsWith("_:")).Select(x => x.Object.ToString()).Distinct().ToList();
                            foreach (string oBlankNode in oBlankNodes)
                            {
                                if (!entidadesCargadas.Contains(oBlankNode))
                                {
                                    entidadesCargar.Add(oBlankNode);
                                    entidadesCargadas.Add(oBlankNode);
                                }
                            }
                        }
                        continue;
                    }
                    else
                    {
                        queryAux = @$" FILTER(?s in(<{ string.Join(">,<", entidadesCargar) }>)).";
                    }
                }
                string consulta = @$"select distinct ?s ?p ?o ?oTitle isBlank(?o) as ?blanknode 
                                    where 
                                    {{ 
                                        ?s ?p ?o. 
                                        {queryAux}
                                        OPTIONAL
                                        {{
                                            ?o ?propTitle ?oTitle
                                            FILTER(?propTitle in(<{ string.Join(">,<", mLinked_Data_Server_Config.PropsTitle) }>)).
                                        }}
                                    }}order by asc(?s) asc(?p) asc(?o)";
                SparqlObject sparqlObject = _sparqlUtility.SelectData(pConfigService, mConfigService.GetSparqlGraph(), consulta, ref pXAppServer);
                foreach (string pendiente in entidadesCargar)
                {                    
                    sparqlObjectDictionary.Add(pendiente, sparqlObject);
                }
                entidadesCargar = new HashSet<string>();
                foreach (Dictionary<string, SparqlObject.Data> row in sparqlObject.results.bindings)
                {
                    if (row.ContainsKey("oTitle") && row.ContainsKey("o"))
                    {
                        pEntitiesNames[row["o"].value] = row["oTitle"].value;
                    }
                    if ((row["blanknode"].value == "1" || row["blanknode"].value == "true") && !entidadesCargadas.Contains(row["o"].value))
                    {
                        entidadesCargar.Add(row["o"].value);
                        entidadesCargadas.Add(row["o"].value);
                    }
                }
            }
            if (mock)
            {
                foreach (string entityId in pEntitiesNames.Keys.ToList())
                {
                    if(entityId.StartsWith("_:"))
                    {
                        pEntitiesNames[entityId.Replace("_:", "")] = pEntitiesNames[entityId];
                        pEntitiesNames.Remove(entityId);
                    }
                }

                foreach(string entityId in sparqlObjectDictionary.Keys.ToList())
                {
                    foreach(Dictionary<string,SparqlObject.Data> fila in sparqlObjectDictionary[entityId].results.bindings)
                    {
                        if(fila["s"].type=="bnode")
                        {
                            fila["s"].value = fila["s"].value.Replace("_:","");
                        }
                        if (fila["o"].type == "bnode")
                        {
                            fila["o"].value = fila["o"].value.Replace("_:", "");
                        }
                    }
                    if (entityId.StartsWith("_:"))
                    {
                        sparqlObjectDictionary[entityId.Replace("_:", "")] = sparqlObjectDictionary[entityId];
                        sparqlObjectDictionary.Remove(entityId);
                    }
                }
            }

            return sparqlObjectDictionary;
        }

        /// <summary>
        /// Obtiene los datos de otros grafos para una entidad
        /// </summary>
        /// <param name="pEntity">URL de la entidad</param>
        /// <returns>Diccionario con los datos de la entidad en varios grafos</returns>
        private Dictionary<string, List<Dictionary<string, SparqlObject.Data>>> GetEntityDataGraphs(string pEntity, ConfigService pConfigService, ref string pXAppServer)
        {
            Dictionary<string, List<Dictionary<string, SparqlObject.Data>>> sparqlObjectDictionary = new Dictionary<string, List<Dictionary<string, SparqlObject.Data>>>();

            string consulta = $@"
                    select distinct ?predicado ?objeto ?fecha ?nameOrg where
                    {{
                        <{pEntity}> <http://www.w3.org/ns/prov#wasUsedBy> ?action.
                        ?action a <http://www.w3.org/ns/prov#Activity>.
                        ?action <http://www.w3.org/1999/02/22-rdf-syntax-ns#predicate> ?predicado.
                        ?action <http://www.w3.org/1999/02/22-rdf-syntax-ns#object> ?objeto.
                        ?action <http://www.w3.org/ns/prov#endedAtTime> ?fecha.
                        OPTIONAL
                        {{ 
                           ?action <http://www.w3.org/ns/prov#wasAssociatedWith> ?org.
                           ?org a <http://www.w3.org/ns/prov#Organization>.
                           ?org <http://purl.org/roh/mirror/foaf#name> ?nameOrg
                        }}
                    }}order by ?action";
            SparqlObject sparqlObject = _sparqlUtility.SelectData(pConfigService, "", consulta, ref pXAppServer);
            if (sparqlObject.results.bindings.Count > 0)
            {
                sparqlObjectDictionary.Add(pEntity, new List<Dictionary<string, SparqlObject.Data>>());
                foreach (Dictionary<string, SparqlObject.Data> row in sparqlObject.results.bindings)
                {
                    sparqlObjectDictionary[pEntity].Add(row);
                }
            }
            return sparqlObjectDictionary;
        }

        /// <summary>
        /// Obtiene los datos de las entidades que apuntan a la entidad seleccionada y los carga en pGraph
        /// </summary>
        /// <param name="pGraph">Grafo en memoria</param>
        /// <param name="pEntities">Entidades a las que buscar como objeto</param>
        /// <param name="pOmitir">Omitir la carga de las entidades pasadas como parámetro</param>
        /// <param name="pSparqlObject">Diccionario con los datos de las entidades cargadas</param>
        /// <param name="pMax">Máximo de entidades a buscar</param>
        /// <returns></returns>
        private HashSet<string> GetInverseEntities(RohGraph pGraph, HashSet<string> pEntities, HashSet<string> pOmitir, Dictionary<string, SparqlObject> pSparqlObject, ConfigService pConfigService, ref string pXAppServer, int? pMax = null)
        {
            HashSet<string> entities = new HashSet<string>();
            HashSet<string> entitiesNotBN = new HashSet<string>();
            HashSet<string> entitiesBN = new HashSet<string>();
            string consulta = @$"select distinct ?s ?p ?o isBlank(?s) as ?blanknode where 
                                {{ 
                                    ?s ?p ?o. 
                                    ?s ?p2 ?o2 . 
                                    ?s ?a ?rdfType . 
                                    FILTER(
                                        (
                                            ?p in (<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>,<{string.Join(">,<", mLinked_Data_Server_Config.PropsTitle)}>) 
                                            OR
                                            (?p=?p2 AND ?o =?o2)
                                        )                                          
                                        AND ?o2 
                                        in(<>,<{ string.Join(">,<", pEntities) }>)
                                    )
                                }} ";
            if (pMax != null)
            {
                consulta += " order by asc(?rdfType) asc(?s) asc(?p) asc(?o) limit " + pMax.Value * 5;
            }
            SparqlObject sparqlObject = _sparqlUtility.SelectData(pConfigService, mConfigService.GetSparqlGraph(), consulta, ref pXAppServer);

            foreach (Dictionary<string, SparqlObject.Data> row in sparqlObject.results.bindings)
            {
                if (pOmitir.Contains(row["s"].value))
                {
                    continue;
                }
                if (pMax.HasValue && entities.Count == pMax.Value && !entities.Contains(row["s"].value))
                {
                    continue;
                }
                if (row["blanknode"].value == "1")
                {
                    entitiesBN.Add(row["s"].value);
                }
                else
                {
                    entitiesNotBN.Add(row["s"].value);
                }
                entities.Add(row["s"].value);
                pSparqlObject[row["s"].value] = sparqlObject;
            }

            foreach (string entity in entitiesNotBN)
            {
                //Insertamos los triples de las entidades en el grafo
                createDataGraph(entity, new List<string>(), entitiesBN.Contains(entity), pGraph, pSparqlObject);
            }
            if (entitiesBN.Except(pOmitir).Count() > 0)
            {
                //Recuperamos mas datos de BBDD
                entitiesNotBN.UnionWith(GetInverseEntities(pGraph, entitiesBN, new HashSet<string>(pOmitir.Union(entities)), pSparqlObject, pConfigService, ref pXAppServer));
            }
            return entitiesNotBN;
        }


        /// <summary>
        /// Crea un grafo en el que se cargan los datos
        /// </summary>
        /// <param name="idEntity">Identificador de la entidad</param>
        /// <param name="parents">Lista de ancestros de la entidad</param>
        /// <param name="isBlank">Booleano que indica si la entidad es un blank node</param>
        /// <param name="datagraph">Grafo donde se cargan los datos</param>
        /// <returns></returns>
        public void createDataGraph(string idEntity, List<string> parents, Boolean isBlank, RohGraph datagraph, Dictionary<string, SparqlObject> sparqlObject)
        {
            foreach (Dictionary<string, SparqlObject.Data> row in sparqlObject[idEntity].results.bindings)
            {
                if (row["s"].value == idEntity)
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
                        else if (row["o"].type == "typed-literal" || row["o"].type == "literal")
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
                        else if (row["o"].type == "typed-literal" || row["o"].type == "literal")
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
        }

        /// <summary>
        /// Crea un modelo DiscoverRdfViewModel
        /// </summary>
        /// <param name="idEntity">Identificador de la entidad de la que crear el modelo</param>
        /// <param name="dataGraph">Grafo que contiene los datos</param>
        /// <param name="pSparqlObjectDictionaryGraphs">Objeto con los datos de otros grafos</param>
        /// <param name="parents">Lista de ancestros de la entidad</param>
        /// <param name="allEntities">Listado con todos los identificadores del RDF</param>
        /// <param name="communNameProperties">Diccionario con los nombres de las propiedades</param>
        /// <param name="entitiesNames">Nombres de las entiadades a las que se apunta</param>
        /// <returns></returns>
        public LinkedDataRdfViewModel createLinkedDataRdfViewModel(string idEntity, RohGraph dataGraph, Dictionary<string, List<Dictionary<string, SparqlObject.Data>>> pSparqlObjectDictionaryGraphs, List<string> parents, List<string> allEntities, Dictionary<string, string> communNameProperties, Dictionary<string, string> entitiesNames)
        {
            //Obtenemos todos los triples de la entidad
            SparqlResultSet sparqlResultSet = (SparqlResultSet)dataGraph.ExecuteQuery("select ?p ?o where { <" + idEntity + "> ?p ?o }");
            LinkedDataRdfViewModel entidad = new LinkedDataRdfViewModel();
            entidad.uriEntity = idEntity;
            entidad.urisRdf = allEntities;
            entidad.communNamePropierties = communNameProperties;
            entidad.entitiesNames = entitiesNames;
            Dictionary<string, List<string>> stringPropertiesEntityAux = new Dictionary<string, List<string>>();
            Dictionary<string, List<LinkedDataRdfViewModel>> entitiesPropertiesEntityAux = new Dictionary<string, List<LinkedDataRdfViewModel>>();
            foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
            {
                if (sparqlResult["o"] is BlankNode && !parents.Contains(sparqlResult["o"].ToString()))
                {
                    if (!entitiesPropertiesEntityAux.ContainsKey(sparqlResult["p"].ToString()))
                    {
                        //Añadimos la propiedad a 'entitiesPropertiesEntity'
                        entitiesPropertiesEntityAux.Add(sparqlResult["p"].ToString(), new List<LinkedDataRdfViewModel>());
                    }
                    parents.Add(idEntity);
                    entitiesPropertiesEntityAux[sparqlResult["p"].ToString()].Add(createLinkedDataRdfViewModel(sparqlResult["o"].ToString(), dataGraph, null, parents, allEntities, communNameProperties, entitiesNames));
                }
                else
                {
                    if (!stringPropertiesEntityAux.ContainsKey(sparqlResult["p"].ToString()))
                    {
                        //Añadimos la propiedad a 'stringPropertiesEntity'
                        stringPropertiesEntityAux.Add(sparqlResult["p"].ToString(), new List<string>());
                    }
                    if (sparqlResult["o"] is LiteralNode)
                    {
                        stringPropertiesEntityAux[sparqlResult["p"].ToString()].Add(((LiteralNode)(sparqlResult["o"])).Value);
                    }
                    else
                    {
                        stringPropertiesEntityAux[sparqlResult["p"].ToString()].Add(sparqlResult["o"].ToString());
                    }
                }
            }
            entidad.stringPropertiesEntity = new Dictionary<string, List<string>>();
            entidad.entitiesPropertiesEntity = new Dictionary<string, List<LinkedDataRdfViewModel>>();
            //Ordenamos 'stringPropertiesEntity', primero el rdf:type, después el título y después el resto
            HashSet<string> propsOrder = new HashSet<string>();
            propsOrder.Add("http://www.w3.org/1999/02/22-rdf-syntax-ns#type");
            propsOrder.UnionWith(mLinked_Data_Server_Config.PropsTitle);
            foreach (string propOrder in propsOrder)
            {
                if (stringPropertiesEntityAux.ContainsKey(propOrder))
                {
                    entidad.stringPropertiesEntity.Add(propOrder, stringPropertiesEntityAux[propOrder]);
                    stringPropertiesEntityAux.Remove(propOrder);
                }
                if (entitiesPropertiesEntityAux.ContainsKey(propOrder))
                {
                    entidad.entitiesPropertiesEntity.Add(propOrder, entitiesPropertiesEntityAux[propOrder]);
                    entitiesPropertiesEntityAux.Remove(propOrder);
                }
            }
            entidad.stringPropertiesEntity = entidad.stringPropertiesEntity.Concat(stringPropertiesEntityAux).ToDictionary(s => s.Key, s => s.Value);
            entidad.entitiesPropertiesEntity = entidad.entitiesPropertiesEntity.Concat(entitiesPropertiesEntityAux).ToDictionary(s => s.Key, s => s.Value);
            entidad.provenanceData = new List<LinkedDataRdfViewModel.ProvenanceData>();
            if (pSparqlObjectDictionaryGraphs != null && pSparqlObjectDictionaryGraphs.ContainsKey(entidad.uriEntity))
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in pSparqlObjectDictionaryGraphs[entidad.uriEntity])
                {
                    LinkedDataRdfViewModel.ProvenanceData provenanceData = new LinkedDataRdfViewModel.ProvenanceData();
                    provenanceData.property = fila["predicado"].value;
                    provenanceData.value = fila["objeto"].value;
                    provenanceData.date = Convert.ToDateTime(fila["fecha"].value, CultureInfo.InvariantCulture);
                    if (fila.ContainsKey("nameOrg"))
                    {
                        provenanceData.organization = fila["nameOrg"].value;
                    }
                    entidad.provenanceData.Add(provenanceData);
                }
            }
            return entidad;

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Generación de Hash para la etiqueta 'ETag'
        /// </summary>
        /// <param name="hashAlgorithm"></param>
        /// <param name="input"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Cargamos las configuraciones 
        /// </summary>
        /// <returns></returns>
        private static Config_Linked_Data_Server LoadLinked_Data_Server_Config()
        {
            return JsonConvert.DeserializeObject<Config_Linked_Data_Server>(System.IO.File.ReadAllText("Config/Linked_Data_Server_Config.json"));
        }


        /// <summary>
        /// Obtiene de la BBDD los datos configurados para pintar las tablas
        /// </summary>
        /// <param name="pDataInferenceGraph">Grafo que contiene los datos (con inferencia)</param>
        /// <param name="pEntity">URL de la entidad</param>
        /// <returns></returns>
        private List<Table> GetDataTables(RohGraph pDataInferenceGraph, string pEntity, ConfigService pConfigService, ref string pXAppServer)
        {
            List<Table> tableList = new List<Table>();
            try
            {
                foreach (Config_Linked_Data_Server.ConfigTable configtable in mLinked_Data_Server_Config.ConfigTables)
                {
                    //Comprobamos si la entidad pEntity tiene algun tipo de configuración
                    SparqlResultSet result = (SparqlResultSet)pDataInferenceGraph.ExecuteQuery("select * where {<" + pEntity + "> ?p <" + configtable.rdfType + "> }");
                    if (result.Count() > 0)
                    {
                        //Obtiene los datos para las tablas
                        tableList.AddRange(LoadTables(pEntity, configtable, pConfigService, ref pXAppServer));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return tableList;
        }


        /// <summary>
        /// Obtiene de la BBDD los datos configurados para pintar los gráficos
        /// </summary>
        /// <param name="pDataInferenceGraph">Grafo que contiene los datos (con inferencia)</param>
        /// <param name="pDataGraph">Grafo que contiene los datos</param>
        /// <param name="pEntity">URL de la entidad</param>
        /// <returns>Lista con los datos para pintar los gráficos</returns>
        private List<ArborGraph> GetDataArborGraphs(RohGraph pDataInferenceGraph, RohGraph pDataGraph, string pEntity, ConfigService pConfigService, ref string pXAppServer)
        {
            List<ArborGraph> arborGraphList = new List<ArborGraph>();
            try
            {
                string rdfType = "";
                //Obtenemos el rdfType
                SparqlResultSet resultRdfType = (SparqlResultSet)pDataGraph.ExecuteQuery("select ?o where {<" + pEntity + "> a ?o }");
                if (resultRdfType.Results.Count > 0)
                {
                    rdfType = resultRdfType.Results[0]["o"].ToString();
                }

                if (mLinked_Data_Server_Config.ConfigArborGraphs.arborGraphsRdfType.Count > 0)
                {
                    foreach (Config_Linked_Data_Server.ConfigArborGraph.ArborGraphRdfType arborGraphRdfType in mLinked_Data_Server_Config.ConfigArborGraphs.arborGraphsRdfType)
                    {
                        //Comprobamos si la entidad pEntity tiene algun tipo de configuración
                        SparqlResultSet result = (SparqlResultSet)pDataInferenceGraph.ExecuteQuery("select * where {<" + pEntity + "> ?p <" + arborGraphRdfType.rdfType + "> }");
                        if (result.Count() > 0)
                        {
                            HashSet<string> propsTitle = new HashSet<string>();
                            foreach (var propTitle in mLinked_Data_Server_Config.PropsTitle)
                            {
                                propsTitle.Add(propTitle);
                            }
                            //Obtenemos el nombre a mostrar
                            SparqlResultSet resultName = (SparqlResultSet)pDataInferenceGraph.ExecuteQuery("select ?o where {<" + pEntity + "> ?propTitle ?o. FILTER(?propTitle in(<" + string.Join(">,<", propsTitle) + ">)) }");
                            if (resultName.Results.Count > 0)
                            {
                                arborGraphList.AddRange(LoadGraphs(pEntity, arborGraphRdfType, ((LiteralNode)(resultName.Results[0]["o"])).Value, rdfType, pConfigService, ref pXAppServer));
                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return arborGraphList;
        }

        /// <summary>
        /// Obtiene las tablas 
        /// </summary>
        /// <param name="pEntity">URL de la entidad</param>
        /// <param name="pConfigtables">Tablas de configuración para la entidad</param>
        /// <returns>Lista con las tablas</returns>
        private List<Table> LoadTables(string pEntity, Config_Linked_Data_Server.ConfigTable pConfigtables, ConfigService pConfigService, ref string pXAppServer)
        {
            List<Table> tableList = new List<Table>();

            foreach (var tableConfig in pConfigtables.tables)
            {
                Table table = new Table();
                table.Rows = new List<Table.Row>();
                table.Header = new List<string>();
                table.Name = tableConfig.name;
                foreach (var field in tableConfig.fields)
                {
                    table.Header.Add(field);
                }
                string consulta = tableConfig.query.Replace("{ENTITY_ID}", pEntity);
                SparqlObject sparqlObject = _sparqlUtility.SelectData(pConfigService, mConfigService.GetSparqlGraph(), consulta, ref pXAppServer);

                foreach (var result in sparqlObject.results.bindings)
                {
                    Table.Row rowlist = new Table.Row();
                    rowlist.fields = new List<string>();
                    foreach (var row in result)
                    {
                        rowlist.fields.Add(row.Value.value);
                    }
                    table.Rows.Add(rowlist);
                }
                tableList.Add(table);
            }
            return tableList;
        }

        /// <summary>
        /// Obtiene los arborGraphs para pintar
        /// </summary>
        /// <param name="pEntity">URL de la entidad</param>
        /// <param name="pArborGraphRdfType">COnfiguracion de arborGraph</param>
        /// <param name="pNameEntity">Nombre de la entidad</param>
        /// <param name="pRdfType">RdfTypes de la entidad</param>
        /// <returns>Lista de los arborGraphs rellenados</returns>
        private List<ArborGraph> LoadGraphs(string pEntity, Config_Linked_Data_Server.ConfigArborGraph.ArborGraphRdfType pArborGraphRdfType, string pNameEntity, string pRdfType, ConfigService pConfigService, ref string pXAppServer)
        {
            List<ArborGraph> arborGraphs = new List<ArborGraph>();

            foreach (Config_Linked_Data_Server.ConfigArborGraph.ArborGraphRdfType.ArborGraph arborGrahConfig in pArborGraphRdfType.arborGraphs)
            {
                ArborGraph arborGraph = new ArborGraph();
                arborGraph.Name = arborGrahConfig.name;
                arborGraph.nodes = new Dictionary<string, ArborGraph.Node>();
                arborGraph.edges = new Dictionary<string, Dictionary<string, ArborGraph.Relation>>();
                Dictionary<string, Dictionary<string, string>> relationNodes = new Dictionary<string, Dictionary<string, string>>();

                //Cargamos el nodo del elemento actual
                string iconPrincipal = "";
                foreach (var iconGraph in mLinked_Data_Server_Config.ConfigArborGraphs.icons)
                {
                    if (iconGraph.rdfType == pRdfType)
                    {
                        iconPrincipal = iconGraph.icon;
                    }
                }
                if (iconPrincipal != "")
                {
                    ArborGraph.Node nodePrincipal = new ArborGraph.Node() { main = true, color = "#af1a2e", label = pNameEntity, image = iconPrincipal, link = pEntity };
                    arborGraph.nodes.Add(pEntity, nodePrincipal);
                }
                else
                {
                    ArborGraph.Node nodePrincipal = new ArborGraph.Node() { main = true, color = "#af1a2e", label = pNameEntity, link = pEntity };
                    arborGraph.nodes.Add(pEntity, nodePrincipal);
                }

                //Cargamos los nodos de la consulta, en la consulta ?id ?nombre 
                foreach (Config_Linked_Data_Server.ConfigArborGraph.ArborGraphRdfType.ArborGraph.Property property in arborGrahConfig.properties)
                {
                    string consulta = property.query.Replace("{ENTITY_ID}", pEntity);
                    SparqlObject sparqlObject = _sparqlUtility.SelectData(pConfigService, mConfigService.GetSparqlGraph(), consulta, ref pXAppServer);

                    Dictionary<string, string> nodesName = new Dictionary<string, string>();
                    Dictionary<string, string> nodesRdfType = new Dictionary<string, string>();
                    // Guardamos los nombres y los rdfTypes de cada entidad
                    foreach (var result in sparqlObject.results.bindings)
                    {
                        foreach (string key in result.Keys)
                        {
                            if (key.StartsWith("level"))
                            {
                                nodesName[result[key].value] = null;
                                nodesRdfType[result[key].value] = null;
                            }
                        }
                    }

                    string consultaNamesRdfType = "select distinct ?s ?name ?rdftype where {?s a ?rdftype.?s ?propTitle ?name. FILTER(?s in (<" + string.Join(">,<", nodesName.Keys) + ">)) Filter(?propTitle in (<" + string.Join(">,<", mLinked_Data_Server_Config.PropsTitle) + ">))}";
                    SparqlObject sparqlObjecDataNamesRdfType = _sparqlUtility.SelectData(pConfigService, mConfigService.GetSparqlGraph(), consultaNamesRdfType, ref pXAppServer);
                    foreach (var result in sparqlObjecDataNamesRdfType.results.bindings)
                    {
                        nodesName[result["s"].value] = result["name"].value;
                        nodesRdfType[result["s"].value] = result["rdftype"].value;
                    }

                    foreach (var result in sparqlObject.results.bindings)
                    {
                        foreach (string key in result.Keys)
                        {

                            if (key.StartsWith("level"))
                            {
                                if (!arborGraph.nodes.ContainsKey(result[key].value))
                                {
                                    string icon = "";
                                    //Pintamos icono segun el rdfType
                                    foreach (var iconGraph in mLinked_Data_Server_Config.ConfigArborGraphs.icons)
                                    {
                                        if (iconGraph.rdfType == nodesRdfType[result[key].value])
                                        {
                                            icon = iconGraph.icon;
                                        }
                                    }
                                    // Agregamos los nodos según si tienen icono
                                    if (icon != "")
                                    {
                                        ArborGraph.Node node = new ArborGraph.Node() { label = nodesName[result[key].value], image = icon, link = result[key].value };
                                        arborGraph.nodes.Add(result[key].value, node);
                                    }
                                    else
                                    {
                                        ArborGraph.Node node = new ArborGraph.Node() { label = nodesName[result[key].value], link = result[key].value };
                                        arborGraph.nodes.Add(result[key].value, node);
                                    }
                                }

                                if (key == "level1")
                                {
                                    // Cargamos la relación principal
                                    if (!relationNodes.ContainsKey(pEntity))
                                    {
                                        relationNodes.Add(pEntity, new Dictionary<string, string>());
                                    }
                                    if (!relationNodes[pEntity].ContainsKey(result[key].value))
                                    {
                                        relationNodes[pEntity].Add(result[key].value, key);
                                    }

                                }
                                else
                                {
                                    // Cargamos las relaciones según el nivel
                                    int levelActual = int.Parse(key.Replace("level", ""));
                                    string levelAnterior = "level" + (levelActual - 1);
                                    if (!relationNodes.ContainsKey(result[levelAnterior].value))
                                    {
                                        relationNodes.Add(result[levelAnterior].value, new Dictionary<string, string>());
                                    }
                                    if (!relationNodes[result[levelAnterior].value].ContainsKey(result[key].value))
                                    {

                                        relationNodes[result[levelAnterior].value].Add(result[key].value, key);
                                    }
                                }

                            }
                        }

                    }
                }
                if (arborGraph.nodes.Count == 1)
                {
                    //si no se ha añadido ningún nodo además del principal no lo cargamos
                    continue;
                }

                // Colores para pintar las relaciones según el nivel
                List<string> colors = new List<string>();
                colors.Add("#af1a2e");
                colors.Add("#ccc");
                colors.Add("blue");
                colors.Add("green");
                colors.Add("yellow");
                colors.Add("orange");

                //cargar los edges 
                foreach (var relation in relationNodes)
                {
                    Dictionary<string, ArborGraph.Relation> relationNode = new Dictionary<string, ArborGraph.Relation>();
                    foreach (var node in relation.Value)
                    {
                        relationNode.Add(node.Key, new ArborGraph.Relation("", colors[int.Parse(node.Value.Replace("level", "")) - 1]));
                    }
                    arborGraph.edges.Add(relation.Key, relationNode);
                }
                arborGraphs.Add(arborGraph);
            }
            return arborGraphs;
        }


        private RohGraph LoadGraph(string pGraph, ConfigService pConfigService, ref string pXAppServer)
        {
            if(ontologyGraph!=null)
            {
                return ontologyGraph;
            }else
            { 
                RohGraph dataGraph = new RohGraph();
                string consulta = "select ?s ?p ?o where { ?s ?p ?o. }";
                SparqlObject sparqlObject = _sparqlUtility.SelectData(pConfigService, pGraph, consulta, ref pXAppServer);

                foreach (Dictionary<string, SparqlObject.Data> row in sparqlObject.results.bindings)
                {
                    SparqlObject.Data sDB = row["s"];
                    SparqlObject.Data pDB = row["p"];
                    SparqlObject.Data oDB = row["o"];
                    #region S
                    INode sG = null;
                    if (sDB.type == "bnode")
                    {
                        sG = dataGraph.CreateBlankNode(sDB.value);
                    }
                    else if (sDB.type == "uri")
                    {
                        sG = dataGraph.CreateUriNode(UriFactory.Create(sDB.value));
                    }
                    #endregion
                    #region P
                    INode pG = dataGraph.CreateUriNode(UriFactory.Create(pDB.value));
                    #endregion
                    #region O
                    INode oG = null;
                    if (oDB.type == "bnode")
                    {
                        oG = dataGraph.CreateBlankNode(oDB.value);
                    }
                    else if (oDB.type == "uri")
                    {
                        oG = dataGraph.CreateUriNode(UriFactory.Create(oDB.value));
                    }
                    else if (oDB.type == "typed-literal" || oDB.type == "literal")
                    {
                        if (!string.IsNullOrEmpty(oDB.lang))
                        {
                            oG = dataGraph.CreateLiteralNode(row["o"].value, oDB.lang);
                        }
                        else if (!string.IsNullOrEmpty(oDB.datatype))
                        {
                            oG = dataGraph.CreateLiteralNode(row["o"].value, new Uri(oDB.datatype));
                        }
                        else
                        {
                            oG = dataGraph.CreateLiteralNode(row["o"].value);
                        }
                    }
                    #endregion
                    dataGraph.Assert(sG, pG, oG);
                }
                ontologyGraph = dataGraph;
                return ontologyGraph;
            }

        }
    }
}
