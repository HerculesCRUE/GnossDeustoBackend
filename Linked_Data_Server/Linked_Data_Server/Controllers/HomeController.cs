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
        private readonly CallEtlApiService _callEtlApiService;
        private readonly static Config_Linked_Data_Server mLinked_Data_Server_Config = LoadLinked_Data_Server_Config();

        public HomeController(ILogger<HomeController> logger, CallEtlApiService callEtlApiService)
        {
            _callEtlApiService = callEtlApiService;
            _logger = logger;
        }

        [Produces("application/rdf+xml", "text/html")]
        public IActionResult Index()
        {
            //Obtenemos la URL de la entidad
            string url = Request.GetDisplayUrl();
            string urlParam = HttpUtility.ParseQueryString(Request.QueryString.Value).Get("url");
            if (!string.IsNullOrEmpty(urlParam))
            {
                url = urlParam;
            }
            ViewBag.UrlHome = mConfigService.GetUrlHome();

            //Customizamos Header
            if (!string.IsNullOrEmpty(mConfigService.GetConstrainedByUrl()))
            {
                HttpContext.Response.Headers.Add("Link", "<http://www.w3.org/ns/ldp#BasicContainer>; rel=\"type\", <http://www.w3.org/ns/ldp#Resource>; rel=\"type\", <" + mConfigService.GetConstrainedByUrl() + ">; rel=\"http://www.w3.org/ns/ldp#constrainedBy\"");
            }
            else
            {
                HttpContext.Response.Headers.Add("Link", "<http://www.w3.org/ns/ldp#BasicContainer>; rel=\"type\", <http://www.w3.org/ns/ldp#Resource>; rel=\"type\"");
            }
            HashSet<string> methodsAvailable = new HashSet<string>() { "GET", "HEAD", "OPTIONS" };
            HttpContext.Response.Headers.Add("allow", string.Join(", ", methodsAvailable));
            if (!methodsAvailable.Contains(Request.HttpContext.Request.Method))
            {
                return StatusCode(StatusCodes.Status405MethodNotAllowed);
            }

            //Cargamos la ontología
            RohGraph ontologyGraph = _callEtlApiService.CallGetOntology();
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
                HttpContext.Response.StatusCode = 404;
                ViewData["Title"] = "Error 404 página no encontrada";
                ViewData["NameTitle"] = mConfigService.GetNameTitle();
                return View(new EntityModelTemplate());
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
                    RohRdfsReasoner reasoner = new RohRdfsReasoner();
                    reasoner.Initialise(ontologyGraph);
                    RohGraph dataInferenceGraph = dataGraph.Clone();
                    reasoner.Apply(dataInferenceGraph);

                    //Obtenemos datos del resto de grafos (para los provenance)
                    Dictionary<string, List<Dictionary<string, SparqlObject.Data>>> sparqlObjectDictionaryGraphs = GetEntityDataGraphs(url);

                    //Obtenemos las tablas configuradas
                    List<Table> dataTables = GetDataTables(dataGraph, url);

                    //Obtenemos los arborGrah configurados
                    List<ArborGraph> dataArborGrahs = GetDataArborGraphs(dataInferenceGraph,dataGraph, url);

                    //Obtenemos las 10 primeras entidades que apuntan a la entidad
                    HashSet<string> inverseEntities = new HashSet<string>();
                    SparqlResultSet sparqlRdfType = (SparqlResultSet)dataInferenceGraph.ExecuteQuery("select distinct ?o where {<"+url+"> a ?o. }");
                    HashSet<string> rdfTypesEntity = new HashSet<string>();
                    foreach (SparqlResult sparqlResult in sparqlRdfType.Results)
                    {
                        rdfTypesEntity.Add(sparqlResult["o"].ToString());
                    }
                    if (mLinked_Data_Server_Config.ExcludeRelatedEntity.Intersect(rdfTypesEntity).Count() == 0)
                    {
                        inverseEntities = GetInverseEntities(dataGraph, new HashSet<string>() { url }, new HashSet<string>(sparqlObjectDictionary.Keys), new Dictionary<string, SparqlObject>(), 10);
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
                    LinkedDataRdfViewModel entidad = createLinkedDataRdfViewModel(url, dataGraph, sparqlObjectDictionaryGraphs, new List<string>(), allEntities, communNamePropierties);
                    modelEntities.Add(entidad);
                    KeyValuePair<string, List<string>> titulo = entidad.stringPropertiesEntity.FirstOrDefault(x => mConfigService.GetPropsTitle().Contains(x.Key));
                    ViewData["Title"] = "About: " + url;
                    if (titulo.Key != null)
                    {
                        ViewData["Title"] = "About: " + titulo.Value[0];
                    }
                    ViewData["NameTitle"] = mConfigService.GetNameTitle();

                    //Preparamos el modelo del resto de entidades
                    foreach (string entity in inverseEntities)
                    {
                        LinkedDataRdfViewModel entidadInversa = createLinkedDataRdfViewModel(entity, dataGraph, null, new List<string>(), allEntities, communNamePropierties);
                        modelEntities.Add(entidadInversa);
                    }


                    EntityModelTemplate entityModel = new EntityModelTemplate();
                    entityModel.linkedDataRDF = modelEntities;
                    entityModel.propsTransform = mConfigService.GetPropsTransform();
                    entityModel.tables = dataTables;
                    entityModel.arborGraphs = dataArborGrahs;
                    return View(entityModel);
                }
            }
        }

        /// <summary>
        /// Obtiene los datos de una entidad para su pintado completo
        /// </summary>
        /// <param name="pEntity">URL de la entidad</param>
        /// <returns>Diccionario con los datos de la entidad</returns>
        private Dictionary<string, SparqlObject> GetEntityData(string pEntity)
        {
            Dictionary<string, SparqlObject> sparqlObjectDictionary = new Dictionary<string, SparqlObject>();
            HashSet<string> entidadesCargar = new HashSet<string>() { pEntity };
            HashSet<string> entidadesCargadas = new HashSet<string>() { pEntity };
            while (entidadesCargar.Count > 0)
            {
                string consulta = "select ?s ?p ?o isBlank(?o) as ?blanknode where { ?s ?p ?o. FILTER(?s in(<>,<" + string.Join(">,<", entidadesCargar) + ">))}order by asc(?s) asc(?p) asc(?o)";
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

        /// <summary>
        /// Obtiene los datos de otros grafos para una entidad
        /// </summary>
        /// <param name="pEntity">URL de la entidad</param>
        /// <returns>Diccionario con los datos de la entidad en varios grafos</returns>
        private Dictionary<string, List<Dictionary<string, SparqlObject.Data>>> GetEntityDataGraphs(string pEntity)
        {
            Dictionary<string, List<Dictionary<string, SparqlObject.Data>>> sparqlObjectDictionary = new Dictionary<string, List<Dictionary<string, SparqlObject.Data>>>();

            string consulta = $@"
                    select ?predicado ?objeto ?fecha ?nameOrg where
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
            SparqlObject sparqlObject = SparqlUtility.SelectData(mConfigService.GetSparqlEndpoint(), "", consulta, mConfigService.GetSparqlQueryParam());
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
        private HashSet<string> GetInverseEntities(RohGraph pGraph, HashSet<string> pEntities, HashSet<string> pOmitir, Dictionary<string, SparqlObject> pSparqlObject, int? pMax = null)
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
                                            ?p in (<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>,<{string.Join(">,<", mConfigService.GetPropsTitle())}>) 
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
            SparqlObject sparqlObject = SparqlUtility.SelectData(mConfigService.GetSparqlEndpoint(), mConfigService.GetSparqlGraph(), consulta, mConfigService.GetSparqlQueryParam());

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
                entitiesNotBN.UnionWith(GetInverseEntities(pGraph, entitiesBN, new HashSet<string>(pOmitir.Union(entities)), pSparqlObject));
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
        /// <returns></returns>
        public LinkedDataRdfViewModel createLinkedDataRdfViewModel(string idEntity, RohGraph dataGraph, Dictionary<string, List<Dictionary<string, SparqlObject.Data>>> pSparqlObjectDictionaryGraphs, List<string> parents, List<string> allEntities, Dictionary<string, string> communNameProperties)
        {
            //Obtenemos todos los triples de la entidad
            SparqlResultSet sparqlResultSet = (SparqlResultSet)dataGraph.ExecuteQuery("select ?p ?o where { <" + idEntity + "> ?p ?o }");
            LinkedDataRdfViewModel entidad = new LinkedDataRdfViewModel();
            entidad.uriEntity = idEntity;
            entidad.urisRdf = allEntities;
            entidad.communNamePropierties = communNameProperties;
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
                    entitiesPropertiesEntityAux[sparqlResult["p"].ToString()].Add(createLinkedDataRdfViewModel(sparqlResult["o"].ToString(), dataGraph, null, parents, allEntities, communNameProperties));
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
            propsOrder.UnionWith(mConfigService.GetPropsTitle());
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
        private List<Table> GetDataTables(RohGraph pDataInferenceGraph, string pEntity)
        {
            List<Table> tableList = new List<Table>();

            foreach (Config_Linked_Data_Server.ConfigTable configtable in mLinked_Data_Server_Config.ConfigTables)
            {
                //Comprobamos si la entidad pEntity tiene algun tipo de configuración
                SparqlResultSet result = (SparqlResultSet)pDataInferenceGraph.ExecuteQuery("select * where {<" + pEntity + "> ?p <" + configtable.rdfType + "> }");
                if (result.Count() > 0)
                {
                    //Obtiene los datos para las tablas
                    tableList.AddRange(LoadTables(pEntity, configtable));
                }
            }
            return tableList;
        }


        /// <summary>
        /// Obtiene de la BBDD los datos configurados para pintar los gráficos
        /// </summary>
        /// <param name="pDataInferenceGraph">Grafo que contiene los datos (con inferencia)</param>
        /// <param name="pDataGraph">Grafo que contiene los datos</param>
        /// <param name="pEntity">URL de la entidad</param>
        /// <returns></returns>
        private List<ArborGraph> GetDataArborGraphs(RohGraph pDataInferenceGraph, RohGraph pDataGraph, string pEntity)
        {
            List<ArborGraph> arborGraphList = new List<ArborGraph>();           
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
                        //Obtenemos el nombre a mostrar
                        SparqlResultSet resultName = (SparqlResultSet)pDataInferenceGraph.ExecuteQuery("select ?o where {<" + pEntity + "> <" + arborGraphRdfType.propName + "> ?o }");                        
                        if (resultName.Results.Count > 0)
                        {
                            arborGraphList.AddRange(LoadGraphs(pEntity, arborGraphRdfType, ((LiteralNode)(resultName.Results[0]["o"])).Value, rdfType));
                        }
                    }

                }

            }

            return arborGraphList;
        }

        /// <summary>
        /// Obtiene las tablas 
        /// </summary>
        /// <param name="pEntity">URL de la entidad</param>
        /// <param name="pConfigtables">Tablas de configuración para la entidad</param>
        /// <returns></returns>
        private List<Table> LoadTables(string pEntity, Config_Linked_Data_Server.ConfigTable pConfigtables)
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
                SparqlObject sparqlObject = SparqlUtility.SelectData(mConfigService.GetSparqlEndpoint(), mConfigService.GetSparqlGraph(), consulta, mConfigService.GetSparqlQueryParam());

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
        /// <returns></returns>
        public List<ArborGraph> LoadGraphs(string pEntity, Config_Linked_Data_Server.ConfigArborGraph.ArborGraphRdfType pArborGraphRdfType, string pNameEntity, string pRdfType)
        {
            //TODO devolver ArborGraph
            List<ArborGraph> arborGraphs = new List<ArborGraph>();

            foreach (Config_Linked_Data_Server.ConfigArborGraph.ArborGraphRdfType.ArborGraph arborGrahConfig in pArborGraphRdfType.arborGraphs)
            {
                ArborGraph arborGraph = new ArborGraph();
                arborGraph.Name = arborGrahConfig.name;
                arborGraph.nodes = new Dictionary<string, ArborGraph.Node>();
                arborGraph.edges = new Dictionary<string, Dictionary<string, ArborGraph.Relation>>();
                Dictionary<string, ArborGraph.Relation> relations = new Dictionary<string, ArborGraph.Relation>();

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
                    ArborGraph.Node nodePrincipal = new ArborGraph.Node() { color = "red", label = pNameEntity, image = iconPrincipal , link = pEntity};
                    arborGraph.nodes.Add(pEntity, nodePrincipal);
                }
                else
                {
                    ArborGraph.Node nodePrincipal = new ArborGraph.Node() { color = "red", label = pNameEntity, link = pEntity };
                    arborGraph.nodes.Add(pEntity, nodePrincipal);
                }

                //Cargamos los nodos de la consulta, en la consulta ?id ?nombre 
                foreach (Config_Linked_Data_Server.ConfigArborGraph.ArborGraphRdfType.ArborGraph.Property property in arborGrahConfig.properties)
                {
                    string consulta = property.query.Replace("{ENTITY_ID}", pEntity);
                    SparqlObject sparqlObject = SparqlUtility.SelectData(mConfigService.GetSparqlEndpoint(), mConfigService.GetSparqlGraph(), consulta, mConfigService.GetSparqlQueryParam());
                    foreach (var result in sparqlObject.results.bindings)
                    {
                        //Pintamos icono segun el rdfType
                        string icon = "";
                        foreach (var iconGraph in mLinked_Data_Server_Config.ConfigArborGraphs.icons)
                        {
                            if (iconGraph.rdfType == result["rdftype"].value)
                            {
                                icon = iconGraph.icon;
                            }
                        }
                        if (icon != "")
                        {
                            ArborGraph.Node node = new ArborGraph.Node() { label = result["name"].value, image = icon, link = result["id"].value};
                            arborGraph.nodes.Add(result["name"].value, node);
                        }
                        else
                        {
                            ArborGraph.Node node = new ArborGraph.Node() { label = result["name"].value , link = result["id"].value };
                            arborGraph.nodes.Add(result["name"].value, node);
                        }
                        relations.Add(result["name"].value, new ArborGraph.Relation(property.name));
                    }
                }
                if(arborGraph.nodes.Count==1)
                {
                    //i no se ha añadido ningún nodo además del principal no lo cargamos
                    continue;
                }
                //cargar los edges en ela entidad actual la relacion con todos los demas
                arborGraph.edges.Add(pEntity, relations);
                arborGraphs.Add(arborGraph);
            }
            return arborGraphs;
        }
    }


}
