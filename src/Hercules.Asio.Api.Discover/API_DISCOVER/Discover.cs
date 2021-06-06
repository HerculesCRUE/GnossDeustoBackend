using API_DISCOVER.Models;
using API_DISCOVER.Models.Entities;
using API_DISCOVER.Models.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using System;
using System.Collections;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using VDS.RDF;
using System.Collections.Generic;
using VDS.RDF.Parsing;
using VDS.RDF.Query.Inference;
using API_DISCOVER.Models.Entities.Discover;
using API_DISCOVER.Utility;
using System.Linq;
using VDS.RDF.Query;
using VDS.RDF.Update;
using System.Threading;
using API_DISCOVER.Models.Logging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using API_DISCOVER.Models.Entities.ExternalAPIs;

namespace API_DISCOVER
{
    /// <summary>
    /// Clase para ejecutar las tareas de descubrimiento
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Discover
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        private readonly static string mPropertySGIRohCrisIdentifier = "http://purl.org/roh#crisIdentifier";
        private RohGraph _ontologyGraph { get; set; }
        public static DiscoverCacheGlobal _discoverCacheGlobal { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="serviceScopeFactory">Factoría de servicios</param>
        public Discover(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        /// <summary>
        /// Procesa un item de descubrimiento
        /// </summary>
        /// <param name="itemIDstring">Identificador del item</param>
        /// <returns></returns>
        public bool ProcessItem(string itemIDstring)
        {
            Guid itemID = JsonConvert.DeserializeObject<Guid>(itemIDstring);
            try
            {
                DiscoverItemBDService discoverItemBDService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<DiscoverItemBDService>();
                ProcessDiscoverStateJobBDService processDiscoverStateJobBDService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ProcessDiscoverStateJobBDService>();
                CallCronApiService callCronApiService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<CallCronApiService>();
                CallEtlApiService callEtlApiService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<CallEtlApiService>();
                CallUrisFactoryApiService callUrisFactoryApiService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<CallUrisFactoryApiService>();

                DiscoverItem discoverItem = discoverItemBDService.GetDiscoverItemById(itemID);

                if (discoverItem != null)
                {
                    //Aplicamos el proceso de descubrimiento
                    DiscoverResult resultado = Init(discoverItem, callEtlApiService, callUrisFactoryApiService);
                    Process(discoverItem, resultado,
                        discoverItemBDService,
                        callCronApiService,
                        callUrisFactoryApiService,
                        processDiscoverStateJobBDService
                        );
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
                //Se ha producido un error al aplicar el descubrimiento
                //Modificamos los datos del DiscoverItem que ha fallado
                DiscoverItemBDService discoverItemBDService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<DiscoverItemBDService>();
                DiscoverItem discoverItemBBDD = discoverItemBDService.GetDiscoverItemById(itemID);
                discoverItemBBDD.UpdateError($"{ex.Message}\n{ex.StackTrace}\n");
                discoverItemBDService.ModifyDiscoverItem(discoverItemBBDD);

                if (!string.IsNullOrEmpty(discoverItemBBDD.JobID))
                {
                    //Si viene de una tarea actualizamos su estado de descubrimiento
                    ProcessDiscoverStateJobBDService processDiscoverStateJobBDService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ProcessDiscoverStateJobBDService>();
                    ProcessDiscoverStateJob processDiscoverStateJob = processDiscoverStateJobBDService.GetProcessDiscoverStateJobByIdJob(discoverItemBBDD.JobID);
                    if (processDiscoverStateJob != null)
                    {
                        processDiscoverStateJob.State = "Error";
                        processDiscoverStateJobBDService.ModifyProcessDiscoverStateJob(processDiscoverStateJob);
                    }
                    else
                    {
                        processDiscoverStateJob = new ProcessDiscoverStateJob() { State = "Error", JobId = discoverItemBBDD.JobID };
                        processDiscoverStateJobBDService.AddProcessDiscoverStateJob(processDiscoverStateJob);
                    }
                }
            }
            return true;

        }

        //TODO mover a otro sitio


        /// <summary>
        /// Realiza el proceso completo de desubrimiento sobre un RDF
        /// </summary>
        /// <param name="pDiscoverItem">Item de descubrimiento</param>
        /// <param name="pCallEtlApiService">Servicio para hacer llamadas a los métodos del controlador etl del API_CARGA </param>
        /// <param name="pCallUrisFactoryApiService">Servicio para hacer llamadas a los métodos del Uris Factory</param>
        /// <returns>DiscoverResult con los datos del descubrimiento</returns>
        private DiscoverResult Init(DiscoverItem pDiscoverItem, CallEtlApiService pCallEtlApiService, CallUrisFactoryApiService pCallUrisFactoryApiService)
        {
            #region Cargamos configuraciones
            ConfigSparql ConfigSparql = new ConfigSparql();
            string SGI_SPARQLEndpoint = ConfigSparql.GetEndpoint();
            string SGI_SPARQLGraph = ConfigSparql.GetGraph();
            string SGI_SPARQLQueryParam = ConfigSparql.GetQueryParam();
            string SGI_SPARQLUsername = ConfigSparql.GetUsername();
            string SGI_SPARQLPassword = ConfigSparql.GetPassword();
            string Unidata_SPARQLEndpoint = ConfigSparql.GetUnidataEndpoint();
            string Unidata_SPARQLGraph = ConfigSparql.GetUnidataGraph();
            string Unidata_SPARQLQueryParam = ConfigSparql.GetUnidataQueryParam();
            string Unidata_SPARQLUsername = ConfigSparql.GetUnidataUsername();
            string Unidata_SPARQLPassword = ConfigSparql.GetUnidataPassword();
            ConfigService ConfigService = new ConfigService();
            float MaxScore = ConfigService.GetMaxScore();
            float MinScore = ConfigService.GetMinScore();
            string UnidataDomain = ConfigService.GetUnidataDomain();
            ConfigScopus ConfigScopus = new ConfigScopus();
            string ScopusApiKey = ConfigScopus.GetScopusApiKey();
            string ScopusUrl = ConfigScopus.GetScopusUrl();
            ConfigCrossref ConfigCrossref = new ConfigCrossref();
            string CrossrefUserAgent = ConfigCrossref.GetCrossrefUserAgent();
            ConfigWOS ConfigWOS = new ConfigWOS();
            string WOSAuthorization = ConfigWOS.GetWOSAuthorization();
            #endregion

            DiscoverUtility discoverUtility = new DiscoverUtility();

            DateTime discoverInitTime = DateTime.Now;

            //Cargamos la ontología  
            if (_ontologyGraph == null)
            {
                _ontologyGraph = pCallEtlApiService.CallGetOntology();
            }

            //Cargamos datos del RDF
            RohGraph dataGraph = new RohGraph();
            Dictionary<string, HashSet<string>> discardDissambiguations = new Dictionary<string, HashSet<string>>();
            if (!string.IsNullOrEmpty(pDiscoverItem.DiscoverRdf))
            {
                //Si tenemos valor en DiscoverRdf, trabajamos con este RDF, ya que estamos reprocesando un rdf validado
                dataGraph.LoadFromString(pDiscoverItem.DiscoverRdf, new RdfXmlParser());
                if (pDiscoverItem.DiscardDissambiguations != null)
                {
                    foreach (DiscoverItem.DiscardDissambiguation discardDissambiguation in pDiscoverItem.DiscardDissambiguations)
                    {
                        if (!discardDissambiguations.ContainsKey(discardDissambiguation.IDOrigin))
                        {
                            discardDissambiguations.Add(discardDissambiguation.IDOrigin, new HashSet<string>());
                        }
                        discardDissambiguations[discardDissambiguation.IDOrigin].UnionWith(discardDissambiguation.DiscardCandidates);
                    }
                }
            }
            else
            {
                dataGraph.LoadFromString(pDiscoverItem.Rdf, new RdfXmlParser());
            }


            //Cargamos el razonador para inferir datos en la ontología
            RohRdfsReasoner reasoner = new RohRdfsReasoner();
            reasoner.Initialise(_ontologyGraph);

            //Cargamos los datos con inferencia
            RohGraph dataInferenceGraph = dataGraph.Clone();
            reasoner.Apply(dataInferenceGraph);

            //Datos para trabajar con la reconciliación
            ReconciliationData reconciliationData = new ReconciliationData();

            //Datos para trabajar con el descubrimiento de enlaces
            DiscoverLinkData discoverLinkData = new DiscoverLinkData();

            //Almacenamos las entidades con dudas acerca de su reonciliación
            Dictionary<string, Dictionary<string, float>> reconciliationEntitiesProbability = new Dictionary<string, Dictionary<string, float>>();

            //Cargamos la caché global
            if (_discoverCacheGlobal == null)
            {
                _discoverCacheGlobal = new DiscoverCacheGlobal();
                discoverUtility.LoadPersonWithName(_discoverCacheGlobal, SGI_SPARQLEndpoint, SGI_SPARQLGraph, SGI_SPARQLQueryParam, SGI_SPARQLUsername, SGI_SPARQLPassword);
                discoverUtility.LoadEntitiesWithTitle(_discoverCacheGlobal, SGI_SPARQLEndpoint, SGI_SPARQLGraph, SGI_SPARQLQueryParam, SGI_SPARQLUsername, SGI_SPARQLPassword);
            }


            if (!pDiscoverItem.DissambiguationProcessed)
            {
                bool hasChanges = true;

                //Cache del proceso de descubrimiento
                DiscoverCache discoverCache = new DiscoverCache();

                //Se realizarán este proceso iterativamente hasta que no haya ningún cambio en lo que a reconciliaciones se refiere
                while (hasChanges)
                {
                    hasChanges = false;

                    //Preparamos los datos para proceder con la reconciliazción
                    discoverUtility.PrepareData(dataGraph, reasoner, out dataInferenceGraph,
                        out Dictionary<string, HashSet<string>> entitiesRdfTypes,
                        out Dictionary<string, string> entitiesRdfType,
                        out Dictionary<string, List<DisambiguationData>> disambiguationDataRdf, false);

                    //Carga los scores de las personas
                    //Aquí se almacenarán los nombres de las personas del RDF, junto con los candidatos de la BBDD y su score
                    Dictionary<string, Dictionary<string, float>> namesScore = new Dictionary<string, Dictionary<string, float>>();
                    discoverUtility.LoadNamesScore(ref namesScore, dataInferenceGraph, discoverCache, _discoverCacheGlobal, MinScore, MaxScore);

                    //0.- Macamos como reconciliadas aquellas que ya estén cargadas en la BBDD con los mismos identificadores
                    List<string> entidadesCargadas = discoverUtility.LoadEntitiesDB(entitiesRdfType.Keys.ToList().Except(reconciliationData.reconciliatedEntityList.Keys.Union(reconciliationData.reconciliatedEntityList.Values)), SGI_SPARQLEndpoint, SGI_SPARQLGraph, SGI_SPARQLQueryParam, SGI_SPARQLUsername, SGI_SPARQLPassword).Keys.ToList();
                    foreach (string entitiID in entidadesCargadas)
                    {
                        reconciliationData.reconciliatedEntityList.Add(entitiID, entitiID);
                        reconciliationData.reconciliatedEntitiesWithSubject.Add(entitiID);
                    }

                    //1.- Realizamos reconciliación con los identificadores configurados (y el roh:identifier) y marcamos como reconciliadas las entidades seleccionadas para no intentar reconciliarlas posteriormente
                    discoverUtility.ReconciliateIDs(ref hasChanges, ref reconciliationData, entitiesRdfType, disambiguationDataRdf, discardDissambiguations, _ontologyGraph, ref dataGraph, discoverCache, SGI_SPARQLEndpoint, SGI_SPARQLQueryParam, SGI_SPARQLGraph, SGI_SPARQLUsername, SGI_SPARQLPassword);

                    //2.- Realizamos la reconciliación con los datos del Propio RDF
                    discoverUtility.ReconciliateRDF(ref hasChanges, ref reconciliationData, _ontologyGraph, ref dataGraph, reasoner, discardDissambiguations, discoverCache, _discoverCacheGlobal, MinScore, MaxScore);

                    //3.- Realizamos la reconciliación con los datos de la BBDD
                    discoverUtility.ReconciliateBBDD(ref hasChanges, ref reconciliationData, out reconciliationEntitiesProbability, _ontologyGraph, ref dataGraph, reasoner, namesScore, discardDissambiguations, discoverCache, _discoverCacheGlobal, MinScore, MaxScore, SGI_SPARQLEndpoint, SGI_SPARQLQueryParam, SGI_SPARQLGraph, SGI_SPARQLUsername, SGI_SPARQLPassword);

                    //4.- Realizamos la reconciliación con los datos de las integraciones externas
                    discoverUtility.ExternalIntegration(ref hasChanges, ref reconciliationData, ref discoverLinkData, ref reconciliationEntitiesProbability, ref dataGraph, reasoner, namesScore,entitiesWithTitle, ontologyGraph, out Dictionary<string, ReconciliationData.ReconciliationScore> entidadesReconciliadasConIntegracionExternaAux, discardDissambiguations, discoverCache,discoverCacheGlobal, ScopusApiKey, ScopusUrl, CrossrefUserAgent, WOSAuthorization, MinScore, MaxScore, SGI_SPARQLEndpoint, SGI_SPARQLQueryParam, SGI_SPARQLGraph, SGI_SPARQLUsername, SGI_SPARQLPassword,pCallUrisFactoryApiService);

                    //Eliminamos de las probabilidades aquellos que ya estén reconciliados
                    foreach (string key in reconciliationData.reconciliatedEntityList.Keys)
                    {
                        reconciliationEntitiesProbability.Remove(key);
                    }
                }

                //5.-Realizamos la detección de equivalencias con Unidata
                //TODO descomentar cuando esté habilitaado Unidata
                //TODO descomentar y revisar en unidata no tienen roh:identifier
                //discoverUtility.EquivalenceDiscover(ontologyGraph, ref dataGraph, reasoner, discoverCache, ref reconciliationEntitiesProbability, discardDissambiguations, UnidataDomain, MinScore, MaxScore, Unidata_SPARQLEndpoint, Unidata_SPARQLQueryParam, Unidata_SPARQLGraph, Unidata_SPARQLUsername, Unidata_SPARQLPassword);
            }
            //TODO comrpobar cuando esté habilitaado Unidata
            DateTime discoverEndTime = DateTime.Now;
            DiscoverResult resultado = new DiscoverResult(dataGraph, dataInferenceGraph, _ontologyGraph, reconciliationData, reconciliationEntitiesProbability, discoverInitTime, discoverEndTime, discoverLinkData);
            
            return resultado;
        }


        /// <summary>
        /// Procesa los resultados del descubrimiento
        /// </summary>
        /// <param name="pDiscoverItem">Objeto con los datos de com procesar el proeso de descubrimiento</param>
        /// <param name="pDiscoverResult">Resultado de la aplicación del descubrimiento</param>
        /// <param name="pDiscoverItemBDService">Clase para gestionar las operaciones de las tareas de descubrimiento</param>
        /// <param name="pCallCronApiService">Servicio para hacer llamadas a los métodos del apiCron</param>
        /// <param name="pCallUrisFactoryApiService">Servicio para hacer llamadas a los métodos del Uris Factory</param>
        /// <param name="pProcessDiscoverStateJobBDService">Clase para gestionar los estados de descubrimiento de las tareas</param>
        /// <returns></returns>
        public void Process(DiscoverItem pDiscoverItem, DiscoverResult pDiscoverResult, DiscoverItemBDService pDiscoverItemBDService, CallCronApiService pCallCronApiService, CallUrisFactoryApiService pCallUrisFactoryApiService, ProcessDiscoverStateJobBDService pProcessDiscoverStateJobBDService)
        {
            #region Cargamos configuraciones
            ConfigSparql ConfigSparql = new ConfigSparql();
            string SGI_SPARQLEndpoint = ConfigSparql.GetEndpoint();
            string SGI_SPARQLGraph = ConfigSparql.GetGraph();
            string SGI_SPARQLQueryParam = ConfigSparql.GetQueryParam();
            string SGI_SPARQLUsername = ConfigSparql.GetUsername();
            string SGI_SPARQLPassword = ConfigSparql.GetPassword();
            string Unidata_SPARQLEndpoint = ConfigSparql.GetUnidataEndpoint();
            string Unidata_SPARQLGraph = ConfigSparql.GetUnidataGraph();
            string Unidata_SPARQLQueryParam = ConfigSparql.GetUnidataQueryParam();
            string Unidata_SPARQLUsername = ConfigSparql.GetUnidataUsername();
            string Unidata_SPARQLPassword = ConfigSparql.GetUnidataPassword();
            ConfigService ConfigService = new ConfigService();
            string UnidataDomain = ConfigService.GetUnidataDomain();
            string UnidataUriTransform = ConfigService.GetUnidataUriTransform();
            #endregion

            /*
             En función del resultado obtenido se realiza una de las siguientes acciones:
                Si para alguna entidad hay más de un candidato que supere el umbral máximo o hay alguna entidad que supere el umbral mínimo pero no alcance el máximo se agregará el RDF a una BBDD junto con todos los datos necesarios para que el administrador decida como proceder.
                Si no estamos en el punto anterior
                    Se obtienen las entidades principales del RDF y se eliminan todos los triples que haya en la BBDD en los que aparezcan como sujeto u objeto.
                    Se eliminan todos los triples cuyo sujeto y predicado estén en el RDF a cargar y estén marcados como monovaluados.
                    Se vuelcan los triples a la BBDD.


             */

            if (pDiscoverItem.Publish)
            {
                if (pDiscoverResult.discoveredEntitiesProbability.Count > 0)
                {
                    //Hay dudas en la desambiguación, por lo que lo actualizamos en la BBDD con su estado correspondiente    
                    pDiscoverItem.UpdateDissambiguationProblems(
                        pDiscoverResult.discoveredEntitiesProbability,
                        pDiscoverResult.reconciliationData.reconciliatedEntitiesWithBBDD.Values.Select(x => x.uri).Union(pDiscoverResult.reconciliationData.reconciliatedEntitiesWithIds.Values).Union(pDiscoverResult.reconciliationData.reconciliatedEntitiesWithSubject).Union(pDiscoverResult.reconciliationData.reconciliatedEntitiesWithExternalIntegration.Values.Select(x => x.uri)).ToList(),
                        pDiscoverResult.GetDataGraphRDF());
                    pDiscoverItemBDService.ModifyDiscoverItem(pDiscoverItem);
                }
                else
                {
                    string urlDiscoverAgent = pCallUrisFactoryApiService.GetUri("Agent", "discover");

                    //Creamos los SameAs hacia unidata para las entidades que NO lo tengan hacia Unidata                       
                    //TODO descomentar cuando esté habilitaado Unidata
                    //pDiscoverResult.dataGraph = AsioPublication.CreateUnidataSameAs(pDiscoverResult.dataGraph, UnidataDomain, UnidataUriTransform);

                    //Publicamos en el SGI
                    AsioPublication asioPublication = new AsioPublication(SGI_SPARQLEndpoint, SGI_SPARQLQueryParam, SGI_SPARQLGraph, SGI_SPARQLUsername, SGI_SPARQLPassword, _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<RabbitMQService>());

                    asioPublication.PublishRDF(_serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<RabbitMQService>(), pDiscoverResult.dataGraph, pDiscoverResult.ontologyGraph, new KeyValuePair<string, string>(urlDiscoverAgent, "Algoritmos de descubrimiento"), pDiscoverResult.start, pDiscoverResult.end, pDiscoverResult.discoverLinkData, pCallUrisFactoryApiService);

                    //TODO Lógica nombres de personas
                    SparqlResultSet sparqlResultSet = (SparqlResultSet)pDiscoverResult.dataGraph.ExecuteQuery("select ?s ?name where {?s a <http://purl.org/roh/mirror/foaf#Person>. ?s <http://purl.org/roh/mirror/foaf#name> ?name}");
                    foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
                    {
                        string s = sparqlResult["s"].ToString();
                        string nombre = sparqlResult["name"].ToString();
                        if (sparqlResult["name"] is ILiteralNode)
                        {
                            nombre = ((ILiteralNode)sparqlResult["name"]).Value;
                        }
                        _discoverCacheGlobal.PersonsNormalizedNames[s] = DiscoverUtility.NormalizeName(nombre);
                    }

                    //TODO Lógica titles
                    SparqlResultSet sparqlResultSetTitles = (SparqlResultSet)pDiscoverResult.dataGraph.ExecuteQuery("select * where {?s a ?rdftype. ?s <http://purl.org/roh#title> ?title}");
                    foreach (SparqlResult sparqlResult in sparqlResultSetTitles.Results)
                    {
                        string s = sparqlResult["s"].ToString();
                        string rdftype = sparqlResult["rdftype"].ToString();
                        string title = sparqlResult["title"].ToString();
                        if (sparqlResult["title"] is ILiteralNode)
                        {
                            title = ((ILiteralNode)sparqlResult["title"]).Value;
                        }
                        title = DiscoverUtility.NormalizeTitle(title);
                        if (!_discoverCacheGlobal.EntitiesNormalizedTitles.ContainsKey(rdftype))
                        {
                            _discoverCacheGlobal.EntitiesNormalizedTitles.Add(rdftype, new Dictionary<string, HashSet<string>>());
                        }
                        if (!_discoverCacheGlobal.EntitiesNormalizedTitles[rdftype].ContainsKey(title))
                        {
                            _discoverCacheGlobal.EntitiesNormalizedTitles[rdftype].Add(title, new HashSet<string>());
                        }
                        _discoverCacheGlobal.EntitiesNormalizedTitles[rdftype][title].Add(s);
                    }

                    //TODO descomentar cuando esté habilitaado Unidata
                    if (false)
                    {
                        //Publicamos en UNIDATA
                        AsioPublication asioPublicationUnidata = new AsioPublication(Unidata_SPARQLEndpoint, Unidata_SPARQLQueryParam, Unidata_SPARQLGraph, Unidata_SPARQLUsername, Unidata_SPARQLPassword, _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<RabbitMQService>());
                        // Prepara el grafo para su carga en Unidata, para ello coge las URIs de Unidata del SameAs y la aplica a los sujetos y los antiguos sujetos se agregan al SameAs
                        RohGraph unidataGraph = AsioPublication.TransformUrisToUnidata(pDiscoverResult.dataGraph, UnidataDomain, UnidataUriTransform);
                        //Eliminamos los triples de crisIdentifier ya que no hay que volcarlos en unidata
                        {
                            TripleStore store = new TripleStore();
                            store.Add(unidataGraph);
                            SparqlUpdateParser parser = new SparqlUpdateParser();
                            //Actualizamos los sujetos
                            SparqlUpdateCommandSet updateSubject = parser.ParseFromString(
                                    @"  DELETE { ?s ?p ?o. }
                                    WHERE 
                                    {
                                        ?s ?p ?o. FILTER(?p =<" + mPropertySGIRohCrisIdentifier + @">)
                                    }");
                            LeviathanUpdateProcessor processor = new LeviathanUpdateProcessor(store);
                            processor.ProcessCommandSet(updateSubject);
                        }
                        asioPublicationUnidata.PublishRDF(_serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<RabbitMQService>(), unidataGraph, pDiscoverResult.ontologyGraph, new KeyValuePair<string, string>(urlDiscoverAgent, "Algoritmos de descubrimiento"), pDiscoverResult.start, pDiscoverResult.end, pDiscoverResult.discoverLinkData, pCallUrisFactoryApiService);
                    }

                    //Lo marcamos como procesado en la BBDD y eliminamos sus metadatos
                    pDiscoverItem.UpdateProcessed();
                    pDiscoverItemBDService.ModifyDiscoverItem(pDiscoverItem);
                }

                //Actualizamos el estado de descubrimiento de la tarea si el estado encolado esta en estado Succeeded o Failed (ha finalizado)
                //TODO cambiar por query a BBDD
                string statusQueueJob = pCallCronApiService.GetJob(pDiscoverItem.JobID).State;
                if ((statusQueueJob == "Failed" || statusQueueJob == "Succeeded"))
                {
                    ProcessDiscoverStateJob processDiscoverStateJob = pProcessDiscoverStateJobBDService.GetProcessDiscoverStateJobByIdJob(pDiscoverItem.JobID);
                    string state;
                    //Actualizamos a error si existen items en estado error o con problemas de desambiguación 
                    if (pDiscoverItemBDService.ExistsDiscoverItemsErrorOrDissambiguatinProblems(pDiscoverItem.JobID))
                    {
                        state = "Error";
                    }
                    else if (pDiscoverItemBDService.ExistsDiscoverItemsPending(pDiscoverItem.JobID))
                    {
                        //Actualizamos a 'Pending' si aún existen items pendientes
                        state = "Pending";
                    }
                    else
                    {
                        //Actualizamos a Success si no existen items en estado error ni con problemas de desambiguación y no hay ninguno pendiente
                        state = "Success";
                    }
                    if (processDiscoverStateJob != null)
                    {
                        processDiscoverStateJob.State = state;
                        pProcessDiscoverStateJobBDService.ModifyProcessDiscoverStateJob(processDiscoverStateJob);
                    }
                    else
                    {
                        processDiscoverStateJob = new ProcessDiscoverStateJob() { State = state, JobId = pDiscoverItem.JobID };
                        pProcessDiscoverStateJobBDService.AddProcessDiscoverStateJob(processDiscoverStateJob);
                    }
                }
            }
            else
            {
                //Actualizamos en BBDD
                DiscoverItem discoverItemBD = pDiscoverItemBDService.GetDiscoverItemById(pDiscoverItem.ID);

                //Reporte de descubrimiento
                string discoverReport = "Time processed (seconds): " + pDiscoverResult.secondsProcessed + "\n";
                if (pDiscoverResult.reconciliationData.reconciliatedEntitiesWithSubject != null && pDiscoverResult.reconciliationData.reconciliatedEntitiesWithSubject.Count > 0)
                {
                    discoverReport += "Entities discover with the same uri: " + pDiscoverResult.reconciliationData.reconciliatedEntitiesWithSubject.Count + "\n";
                    foreach (string uri in pDiscoverResult.reconciliationData.reconciliatedEntitiesWithSubject)
                    {
                        discoverReport += "\t" + uri + "\n";
                    }
                }
                if (pDiscoverResult.reconciliationData.reconciliatedEntitiesWithIds != null && pDiscoverResult.reconciliationData.reconciliatedEntitiesWithIds.Count > 0)
                {
                    discoverReport += "Entities discover with some common identifier: " + pDiscoverResult.reconciliationData.reconciliatedEntitiesWithIds.Count + "\n";
                    foreach (string uri in pDiscoverResult.reconciliationData.reconciliatedEntitiesWithIds.Keys)
                    {
                        discoverReport += "\t" + uri + " --> " + pDiscoverResult.reconciliationData.reconciliatedEntitiesWithIds[uri] + "\n";
                    }
                }
                if (pDiscoverResult.reconciliationData.reconciliatedEntitiesWithBBDD != null && pDiscoverResult.reconciliationData.reconciliatedEntitiesWithBBDD.Count > 0)
                {
                    discoverReport += "Entities discover with reconciliation config: " + pDiscoverResult.reconciliationData.reconciliatedEntitiesWithBBDD.Count + "\n";
                    foreach (string uri in pDiscoverResult.reconciliationData.reconciliatedEntitiesWithBBDD.Keys)
                    {
                        discoverReport += "\t" + uri + " --> " + pDiscoverResult.reconciliationData.reconciliatedEntitiesWithBBDD[uri] + "\n";
                    }
                }
                if (pDiscoverResult.reconciliationData.reconciliatedEntitiesWithExternalIntegration != null && pDiscoverResult.reconciliationData.reconciliatedEntitiesWithExternalIntegration.Count > 0)
                {
                    discoverReport += "Entities discover with external integrations config: " + pDiscoverResult.reconciliationData.reconciliatedEntitiesWithExternalIntegration.Count + "\n";
                    foreach (string uri in pDiscoverResult.reconciliationData.reconciliatedEntitiesWithExternalIntegration.Keys)
                    {
                        discoverReport += "\t" + uri + " --> " + pDiscoverResult.reconciliationData.reconciliatedEntitiesWithExternalIntegration[uri] + "\n";
                    }
                }
                if (pDiscoverResult.discoverLinkData != null && pDiscoverResult.discoverLinkData.entitiesProperties != null && pDiscoverResult.discoverLinkData.entitiesProperties.Count > 0)
                {
                    discoverReport += "Entities with identifiers obtained with External integration: " + pDiscoverResult.discoverLinkData.entitiesProperties.Count + "\n";
                    foreach (string uri in pDiscoverResult.discoverLinkData.entitiesProperties.Keys)
                    {
                        foreach (DiscoverLinkData.PropertyData property in pDiscoverResult.discoverLinkData.entitiesProperties[uri])
                        {
                            foreach (string value in property.valueProvenance.Keys)
                            {
                                foreach (string provenance in property.valueProvenance[value])
                                {
                                    discoverReport += "\t" + uri + " - " + property.property + " - " + value + " --> " + provenance + "\n";
                                }
                            }
                        }
                    }
                }
                discoverItemBD.UpdateReport(pDiscoverResult.discoveredEntitiesProbability, pDiscoverResult.GetDataGraphRDF(), discoverReport);
                pDiscoverItemBDService.ModifyDiscoverItem(discoverItemBD);
            }
        }

        /// <summary>
        /// Aplica el descubrimiento sobre las entidades cargadas en el SGI
        /// </summary>
        /// <param name="pSecondsSleep">Segundos para dormir después de procesar una entidad</param>
        /// <param name="pCallUrisFactoryApiService">Servicio para hacer llamadas a los métodos del Uris Factory</param>
        public void ApplyDiscoverLoadedEntities(int pSecondsSleep, CallUrisFactoryApiService pCallUrisFactoryApiService)
        {
            CallEtlApiService callEtlApiService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<CallEtlApiService>();

            #region Cargamos configuraciones
            ConfigSparql ConfigSparql = new ConfigSparql();
            string SGI_SPARQLEndpoint = ConfigSparql.GetEndpoint();
            string SGI_SPARQLGraph = ConfigSparql.GetGraph();
            string SGI_SPARQLQueryParam = ConfigSparql.GetQueryParam();
            string SGI_SPARQLUsername = ConfigSparql.GetUsername();
            string SGI_SPARQLPassword = ConfigSparql.GetPassword();
            string Unidata_SPARQLEndpoint = ConfigSparql.GetUnidataEndpoint();
            string Unidata_SPARQLGraph = ConfigSparql.GetUnidataGraph();
            string Unidata_SPARQLQueryParam = ConfigSparql.GetUnidataQueryParam();
            string Unidata_SPARQLUsername = ConfigSparql.GetUnidataUsername();
            string Unidata_SPARQLPassword = ConfigSparql.GetUnidataPassword();

            ConfigService ConfigService = new ConfigService();
            string UnidataDomain = ConfigService.GetUnidataDomain();
            string UnidataUriTransform = ConfigService.GetUnidataUriTransform();
            float MaxScore = ConfigService.GetMaxScore();
            float MinScore = ConfigService.GetMinScore();

            ConfigScopus ConfigScopus = new ConfigScopus();
            string ScopusApiKey = ConfigScopus.GetScopusApiKey();
            string ScopusUrl = ConfigScopus.GetScopusUrl();
            ConfigCrossref ConfigCrossref = new ConfigCrossref();
            string CrossrefUserAgent = ConfigCrossref.GetCrossrefUserAgent();
            ConfigWOS ConfigWOS = new ConfigWOS();
            string WOSAuthorization = ConfigWOS.GetWOSAuthorization();
            #endregion

            DiscoverUtility discoverUtility = new DiscoverUtility();

            //Cargar todas las personas en la lista de manera aleatoria.
            List<string> personList = discoverUtility.GetPersonList(SGI_SPARQLEndpoint, SGI_SPARQLGraph, SGI_SPARQLQueryParam, SGI_SPARQLUsername, SGI_SPARQLPassword);
            List<string> randomPersonList = GetRandomOrderList(personList);
            RohGraph ontologyGraph = callEtlApiService.CallGetOntology();
            foreach (string person in randomPersonList)
            {
                try
                {
                    //Hora de inicio de la ejecución
                    DateTime startTime = DateTime.Now;

                    //Obtener el RohGraph de una única persona.
                    RohGraph dataGraph = discoverUtility.GetDataGraphPersonLoadedForDiscover(person, SGI_SPARQLEndpoint, SGI_SPARQLGraph, SGI_SPARQLQueryParam, SGI_SPARQLUsername, SGI_SPARQLPassword);
                    //Clonamos el grafo original para hacer luego comprobaciones
                    RohGraph originalDataGraph = dataGraph.Clone();
                    
                    RohRdfsReasoner reasoner = new RohRdfsReasoner();
                    reasoner.Initialise(ontologyGraph);
                    RohGraph dataInferenceGraph = dataGraph.Clone();
                    reasoner.Apply(dataInferenceGraph);

                    bool hasChanges = false;
                    //Dictionary<string, string> discoveredEntityList = new Dictionary<string, string>();
                    Dictionary<string, Dictionary<string, float>> discoveredEntitiesProbability = new Dictionary<string, Dictionary<string, float>>();
                    Dictionary<string, ReconciliationData.ReconciliationScore> entidadesReconciliadasConIntegracionExternaAux;
                    Dictionary<string, HashSet<string>> discardDissambiguations = new Dictionary<string, HashSet<string>>();
                    DiscoverCache discoverCache = new DiscoverCache();
                    DiscoverCacheGlobal discoverCacheGlobal = new DiscoverCacheGlobal();

                    //Obtención de la integración externa
                    ReconciliationData reconciliationData = new ReconciliationData();
                    DiscoverLinkData discoverLinkData = new DiscoverLinkData();
                    Dictionary<string, List<DiscoverLinkData.PropertyData>> integration = discoverUtility.ExternalIntegration(ref hasChanges, ref reconciliationData, ref discoverLinkData, ref discoveredEntitiesProbability, ref dataGraph, reasoner, null, ontologyGraph, out entidadesReconciliadasConIntegracionExternaAux, discardDissambiguations, discoverCache, discoverCacheGlobal, ScopusApiKey, ScopusUrl, CrossrefUserAgent, WOSAuthorization, MinScore, MaxScore, SGI_SPARQLEndpoint, SGI_SPARQLGraph, SGI_SPARQLQueryParam, SGI_SPARQLUsername, SGI_SPARQLPassword, pCallUrisFactoryApiService, false);

                    //Limpiamos 'integration' para no insertar triples en caso de que ya estén cargados
                    foreach (string entity in integration.Keys.ToList())
                    {
                        foreach (DiscoverLinkData.PropertyData propertyData in integration[entity].ToList())
                        {
                            string p = propertyData.property;
                            HashSet<string> objetos = new HashSet<string>(propertyData.valueProvenance.Keys.ToList());
                            foreach (string o in objetos)
                            {
                                if (((SparqlResultSet)originalDataGraph.ExecuteQuery($@"ASK
                                        WHERE 
                                        {{
                                            ?s ?p ?o.
                                            FILTER(?s=<{entity}>)
                                            FILTER(?p=<{p}>)
                                            FILTER(str(?o)='{o}')
                                        }}")).Result)
                                {
                                    //Elimiamos el valor porque ya estaba cargado
                                    propertyData.valueProvenance.Remove(o);
                                }
                            }
                            if (propertyData.valueProvenance.Count == 0)
                            {
                                integration[entity].Remove(propertyData);
                            }
                        }
                        if (integration[entity].Count == 0)
                        {
                            integration.Remove(entity);
                        }
                    }


                    //Creación de dataGraph con el contenido de 'integration' + RdfTypes + SameAS
                    RohGraph dataGraphIntegration = new RohGraph();
                    foreach (string sujeto in integration.Keys)
                    {
                        IUriNode s = dataGraphIntegration.CreateUriNode(UriFactory.Create(sujeto));

                        //Agregamos SameAs y RDFType de las entidades
                        SparqlResultSet sparqlResultSet = (SparqlResultSet)dataGraph.ExecuteQuery("select ?rdftype ?sameas where {?s a ?rdftype. OPTIONAL{?s <http://www.w3.org/2002/07/owl#sameAs> ?sameAS} FILTER(?s=<" + sujeto + ">)}");
                        foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
                        {
                            string rdfType = sparqlResult["rdftype"].ToString();
                            IUriNode pRdfType = dataGraphIntegration.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));
                            IUriNode oRdfType = dataGraphIntegration.CreateUriNode(UriFactory.Create(rdfType));
                            dataGraphIntegration.Assert(new Triple(s, pRdfType, oRdfType));
                            if (sparqlResult.Variables.Contains("sameas"))
                            {
                                string sameas = sparqlResult["sameas"].ToString();
                                IUriNode pSameAs = dataGraphIntegration.CreateUriNode(UriFactory.Create("http://www.w3.org/2002/07/owl#sameAs"));
                                IUriNode oSameAs = dataGraphIntegration.CreateUriNode(UriFactory.Create(sameas));
                                dataGraphIntegration.Assert(new Triple(s, pSameAs, oSameAs));
                            }
                        }

                        foreach (DiscoverLinkData.PropertyData propertyData in integration[sujeto])
                        {
                            foreach (string valor in propertyData.valueProvenance.Keys)
                            {
                                IUriNode p = dataGraphIntegration.CreateUriNode(UriFactory.Create(propertyData.property));
                                if (Uri.IsWellFormedUriString(valor, UriKind.Absolute))
                                {
                                    IUriNode uriNode = dataGraphIntegration.CreateUriNode(UriFactory.Create(propertyData.property));
                                    dataGraphIntegration.Assert(new Triple(s, p, uriNode));
                                }
                                else
                                {
                                    ILiteralNode literalNode = dataGraphIntegration.CreateLiteralNode(valor, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                    dataGraphIntegration.Assert(new Triple(s, p, literalNode));
                                }

                                foreach (string org in propertyData.valueProvenance[valor])
                                {
                                    //Agregamos los datos de las organizaciones y los grafos
                                    SparqlResultSet sparqlResultSetOrgs = (SparqlResultSet)dataGraph.ExecuteQuery("select ?s ?p ?o where {?s ?p ?o. FILTER(?s in(<" + pCallUrisFactoryApiService.GetUri("http://purl.org/roh/mirror/foaf#Organization", org) + ">,<" + pCallUrisFactoryApiService.GetUri("Graph", org) + "> ))}");
                                    foreach (SparqlResult sparqlResult in sparqlResultSetOrgs.Results)
                                    {
                                        INode sOrg = dataGraphIntegration.CreateUriNode(UriFactory.Create(sparqlResult["s"].ToString()));
                                        INode pOrg = dataGraphIntegration.CreateUriNode(UriFactory.Create(sparqlResult["p"].ToString()));
                                        if (sparqlResult["o"] is UriNode)
                                        {
                                            INode oOrg = dataGraphIntegration.CreateUriNode(UriFactory.Create(sparqlResult["o"].ToString()));
                                            dataGraphIntegration.Assert(new Triple(sOrg, pOrg, oOrg));
                                        }
                                        else if (sparqlResult["o"] is LiteralNode)
                                        {
                                            INode oOrg = dataGraphIntegration.CreateLiteralNode(((LiteralNode)sparqlResult["o"]).Value, ((LiteralNode)sparqlResult["o"]).DataType);
                                            dataGraphIntegration.Assert(new Triple(sOrg, pOrg, oOrg));
                                        }

                                    }
                                }
                            }
                        }
                    }
                    //Hora fin de la ejecución
                    DateTime endTime = DateTime.Now;
                    if (integration.Count > 0)
                    {
                        //Si hay datos nuevos los cargamos
                        string urlDiscoverAgent = pCallUrisFactoryApiService.GetUri("Agent", "discover");

                        //Publicamos en el SGI
                        AsioPublication asioPublication = new AsioPublication(SGI_SPARQLEndpoint, SGI_SPARQLQueryParam, SGI_SPARQLGraph, SGI_SPARQLUsername, SGI_SPARQLPassword, _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<RabbitMQService>());
                        asioPublication.PublishRDF(_serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<RabbitMQService>(), dataGraphIntegration, null, new KeyValuePair<string, string>(urlDiscoverAgent, "Algoritmos de descubrimiento"), startTime, endTime, discoverLinkData, pCallUrisFactoryApiService);

                        //TODO descomentar cuando esté habilitaado Unidata

                        //Preparamos los datos para cargarlos en Unidata
                        //RohGraph unidataGraph = dataGraphIntegration.Clone();
                        //#region Si no tiene un sameAs apuntando a Unidata lo eliminamos, no hay que cargar la entidad
                        //SparqlResultSet sparqlResultSet = (SparqlResultSet)unidataGraph.ExecuteQuery("select ?s ?rdftype ?sameas where {?s a ?rdftype. OPTIONAL{?s <http://www.w3.org/2002/07/owl#sameAs> ?sameAS} }");
                        //Dictionary<string, bool> entidadesConSameAsUnidata = new Dictionary<string, bool>();
                        //foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
                        //{
                        //    string s = sparqlResult["s"].ToString();
                        //    if (!entidadesConSameAsUnidata.ContainsKey(s))
                        //    {
                        //        entidadesConSameAsUnidata.Add(s, false);
                        //    }
                        //    if (sparqlResult.Variables.Contains("sameas"))
                        //    {
                        //        if (sparqlResult["sameas"].ToString().StartsWith(UnidataDomain))
                        //        {
                        //            entidadesConSameAsUnidata[s] = true;
                        //        }
                        //    }
                        //}
                        //TripleStore store = new TripleStore();
                        //store.Add(unidataGraph);
                        //foreach (string entity in entidadesConSameAsUnidata.Keys)
                        //{
                        //    if (!entidadesConSameAsUnidata[entity])
                        //    {
                        //        //Cambiamos candidato.Key por entityID
                        //        SparqlUpdateParser parser = new SparqlUpdateParser();
                        //        SparqlUpdateCommandSet delete = parser.ParseFromString(@"DELETE { ?s ?p ?o. }
                        //                                                        WHERE 
                        //                                                        {
                        //                                                            ?s ?p ?o. 
                        //                                                            FILTER(?s = <" + entity + @">)

                        //                                                        }");
                        //        LeviathanUpdateProcessor processor = new LeviathanUpdateProcessor(store);
                        //        processor.ProcessCommandSet(delete);
                        //    }
                        //}
                        //#endregion


                        ////Si hay triples para cargar en Unidata procedemos
                        //if (unidataGraph.Triples.ToList().Count > 0)
                        //{
                        //    //Publicamos en UNIDATA
                        //    AsioPublication asioPublicationUnidata = new AsioPublication(Unidata_SPARQLEndpoint, Unidata_SPARQLQueryParam, Unidata_SPARQLGraph, Unidata_SPARQLUsername, Unidata_SPARQLPassword);
                        //    // Prepara el grafo para su carga en Unidata, para ello coge las URIs de Unidata del SameAs y la aplica a los sujetos y los antiguos sujetos se agregan al SameAs
                        //    unidataGraph = AsioPublication.TransformUrisToUnidata(unidataGraph, UnidataDomain, UnidataUriTransform);
                        //    asioPublicationUnidata.PublishRDF(unidataGraph, null, new KeyValuePair<string, string>(urlDiscoverAgent, "Algoritmos de descubrimiento"), startTime, endTime, discoverLinkData,pCallUrisFactoryApiService);
                        //}
                    }
                }
                catch (Exception exception)
                {
                    Logging.Error(exception);
                }
                Thread.Sleep(pSecondsSleep * 1000);
            }
        }


        /// <summary>
        /// Desordena un listado que se le pasa por parámetro.
        /// </summary>
        /// <param name="pList">Lista</param>
        /// <returns>Lista desordenada</returns>
        private List<string> GetRandomOrderList(List<string> pList)
        {
            List<string> rngList = new List<string>();
            List<string> personListAux = new List<string>();
            Random rng = new Random();
            personListAux.AddRange(pList);
            while (personListAux.Count > 0)
            {
                int val = rng.Next(0, personListAux.Count - 1);
                rngList.Add(personListAux[val]);
                personListAux.RemoveAt(val);
            }
            return rngList;
        }
    }
}
