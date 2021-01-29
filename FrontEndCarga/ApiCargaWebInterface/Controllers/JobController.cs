// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Controlador tareas
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ApiCargaWebInterface.Models.Entities;
using ApiCargaWebInterface.Models.Services;
using ApiCargaWebInterface.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NCrontab;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Update;
using VDS.RDF.Writing;

namespace ApiCargaWebInterface.Controllers
{
    /// <summary>
    /// Controlador para gestionar las llamadas relacionandas con el Api cron
    /// </summary>
    public class JobController : Controller
    {
        readonly CallCronApiService _serviceApi;
        readonly ConfigUnidataPrefix _unidataPrefix;
        readonly DiscoverItemBDService _discoverItemService;
        readonly ProcessDiscoverStateJobBDService _processDiscoverStateJobBDService;
        readonly ICallEtlService _callEDtlPublishService;
        public JobController(DiscoverItemBDService iIDiscoverItemService, ProcessDiscoverStateJobBDService iProcessDiscoverStateJobBDService, CallCronApiService serviceApi, ICallEtlService callEDtlPublishService, ConfigUnidataPrefix unidataPrefix)
        {
            _serviceApi = serviceApi;
            _discoverItemService = iIDiscoverItemService;
            _processDiscoverStateJobBDService = iProcessDiscoverStateJobBDService;
            _callEDtlPublishService = callEDtlPublishService;
            _unidataPrefix = unidataPrefix;
        }
        /// <summary>
        /// Devuelve una página principal con una lista de tareas vacía
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            List<CreateRecurringJobViewModel> lista = new List<CreateRecurringJobViewModel>();
            return View(lista);
        }
        /// <summary>
        /// Devuelve la página de creación de una tarea con el idetificador del repositorio asociado
        /// </summary>
        /// <param name="IdRepository">Identificador del repositorio</param>
        /// <returns></returns>
        public IActionResult Create(Guid? IdRepository = null)
        {
            if (IdRepository.HasValue)
            {
                CreateJobViewModel createJobViewModel = new CreateJobViewModel()
                {
                    IdRepository = IdRepository.Value
                };
                return View(createJobViewModel);
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// Obtiene los detalles de una tarea
        /// </summary>
        /// <param name="id">Identificador de una tarea</param>
        /// <returns></returns>
        [HttpGet("[Controller]/{id}")]
        public IActionResult DetailsJob(string id, Guid repository_id)
        {
            var job = _serviceApi.GetJob(id);
            ProcessDiscoverStateJob stateJob = _processDiscoverStateJobBDService.GetProcessDiscoverStateJobByIdJob(job.Id);
            if (stateJob != null)
            {
                job.DiscoverState = stateJob.State;
            }
            if (!Guid.Empty.Equals(repository_id))
            {
                job.IdRepository = repository_id;
            }
            job.DiscoverStates = _discoverItemService.GetDiscoverItemsStatesByJob(id);
            var discoverItemsErrorMini = _discoverItemService.GetDiscoverItemsErrorByJobMini(id);
            job.DiscoverItemsMini = discoverItemsErrorMini;
            return View(job);
        }

        /// <summary>
        /// Obtiene los detalles de una tarea recurrente
        /// </summary>
        /// <param name="name">Nombre de la tarea recurrente</param>
        /// <returns></returns>
        [HttpGet("[Controller]/recurring/{name}")]
        public IActionResult DetailsRecurringJob(string name)
        {
            var recurringJob = _serviceApi.GetRecurringJob(name);
            RecurringJobWebViewModel recurringJobViewModel = new RecurringJobWebViewModel();
            recurringJobViewModel.RecurringJobViewModel = recurringJob;
            recurringJobViewModel.ListJobs = _serviceApi.GetJobsOfRecurringJob(name);
            return View(recurringJobViewModel);
        }

        /// <summary>
        /// Crea una tarea nueva
        /// </summary>
        /// <param name="jobModel">Detalles de la tarea a crear</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create(CreateJobViewModel jobModel)
        {

            if (jobModel.IdRepository.Equals(Guid.Empty))
            {
                ModelState.AddModelError("IdRepository", "id del repositorio no válido");
            }
            if ((jobModel.Nombre_job != null && jobModel.CronExpression == null) || (jobModel.CronExpression != null && jobModel.Nombre_job == null))
            {
                ModelState.AddModelError("Nombre_job", "faltan datos para crear un job recurrente");
            }
            else if (!string.IsNullOrEmpty(jobModel.Nombre_job) && !string.IsNullOrEmpty(jobModel.CronExpression))
            {
                var correct = CrontabSchedule.TryParse(jobModel.CronExpression);
                if (correct == null)
                {
                    ModelState.AddModelError("CronExpression", "expresión del cron inválida");
                }
            }
            if (!ModelState.IsValid)
            {
                return View("Create", jobModel);
            }
            else
            {
                if (jobModel.Nombre_job != null)
                {
                    _serviceApi.CreateRecurringJob(jobModel);
                    resultCreated item = new resultCreated()
                    {
                        Id = jobModel.Nombre_job
                    };
                    return RedirectToAction("Details", "RepositoryConfig", new { id = jobModel.IdRepository });
                }
                else
                {
                    string id = _serviceApi.CreateJob(jobModel);
                    resultCreated item = new resultCreated()
                    {
                        Id = id
                    };
                    return RedirectToAction("Details", "RepositoryConfig", new { id = jobModel.IdRepository });
                }
            }

        }

        /// <summary>
        /// Elimina una tarea
        /// </summary>
        /// <param name="id">identificador de la tarea</param>
        /// <param name="job">tipo de tarea (programada: scheduled o recurrente: recurring)</param>
        /// <returns></returns>
        public IActionResult Delete(string id, string job)
        {
            if (job.Equals("scheduled"))
            {
                _serviceApi.DeleteScheduledJob(id);
            }
            else if (job.Equals("recurring"))
            {
                _serviceApi.DeleteRecurringJob(id);
            }
            return View("Deleted", id);
        }

        /// <summary>
        /// Crea una tarea de sincronización para un repositorio
        /// </summary>
        /// <param name="repositoryId">Identificador del repositorio</param>
        /// <returns></returns>
        public IActionResult Syncro(Guid repositoryId)
        {
            CreateJobViewModel jobModel = new CreateJobViewModel() { IdRepository = repositoryId };
            string id = _serviceApi.CreateJob(jobModel);
            resultCreated item = new resultCreated()
            {
                Id = id
            };
            return View("Created", item);
        }

        /// <summary>
        /// Vuelve a encolar una tarea
        /// </summary>
        /// <param name="idJob">Identificador de la tarea</param>
        /// <returns></returns>
        public IActionResult ReQueue(string idJob)
        {
            _serviceApi.ReQueueJob(idJob);
            var job = _serviceApi.GetJob(idJob);
            return View("DetailsJob", job);
        }
        /// <summary>
        /// Comprueba que una expresión cron es válida
        /// </summary>
        /// <param name="CronExpression">Expresión a comprobar</param>
        /// <returns></returns>
        public IActionResult CronValid(string CronExpression)
        {
            var correct = CrontabSchedule.TryParse(CronExpression);
            if (correct != null)
            {
                return Json(true);
            }
            return Json(false);
        }

        /// <summary>
        /// Resuelve un problema de descubrimiento de un job
        /// </summary>
        /// <param name="DissambiguationProblemsResolve">Resolución con los problemas de desambiguación</param>
        /// <param name="IdDiscoverItem">Identificador del item de descubrimiento</param>
        /// <param name="idJob">Identificador de la tarea a la que eprtenece el item de descubrimiento</param>
        /// <returns></returns>
        [HttpPost("[Controller]/{idJob}/resolve/{IdDiscoverItem}")]
        public IActionResult ResolveDiscover(string idJob, string IdDiscoverItem, Dictionary<string, string> DissambiguationProblemsResolve)
        {
            DiscoverItem item = _discoverItemService.GetDiscoverItemById(new Guid(IdDiscoverItem));

            //Cargamos el RDF
            RohGraph dataGraph = new RohGraph();
            dataGraph.LoadFromString(item.DiscoverRdf, new RdfXmlParser());

            //Modificamos el RDF
            TripleStore store = new TripleStore();
            store.Add(dataGraph);
            //Cambiamos candidato.Key por entityID
            foreach (string uriOriginal in DissambiguationProblemsResolve.Keys)
            {
                if (!string.IsNullOrEmpty(DissambiguationProblemsResolve[uriOriginal]))
                {
                    //En caso de que la resolución sea una URI de Unidata añadimos el SameAs                
                    if (!string.IsNullOrEmpty(_unidataPrefix.GetUnidataDomain()) && DissambiguationProblemsResolve[uriOriginal].StartsWith(_unidataPrefix.GetUnidataDomain()))
                    {
                        IUriNode t_subject = dataGraph.CreateUriNode(UriFactory.Create(uriOriginal));
                        IUriNode t_predicate = dataGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/2002/07/owl#sameAs"));
                        IUriNode t_object = dataGraph.CreateUriNode(UriFactory.Create(DissambiguationProblemsResolve[uriOriginal]));
                        dataGraph.Assert(new Triple(t_subject, t_predicate, t_object));
                    }
                    else
                    {
                        //En caso de que la resolución NO sea una URI de Unidata modificamos las URLs
                        SparqlUpdateParser parser = new SparqlUpdateParser();
                        //Actualizamos los sujetos
                        SparqlUpdateCommandSet updateSubject = parser.ParseFromString(@"DELETE { ?s ?p ?o. }
                                                                    INSERT{<" + DissambiguationProblemsResolve[uriOriginal] + @"> ?p ?o.}
                                                                    WHERE 
                                                                    {
                                                                        ?s ?p ?o.   FILTER(?s = <" + uriOriginal + @">)
                                                                    }");
                        //Actualizamos los objetos
                        SparqlUpdateCommandSet updateObject = parser.ParseFromString(@"DELETE { ?s ?p ?o. }
                                                                    INSERT{?s ?p <" + DissambiguationProblemsResolve[uriOriginal] + @">.}
                                                                    WHERE 
                                                                    {
                                                                        ?s ?p ?o.   FILTER(?o = <" + uriOriginal + @">)
                                                                    }");
                        LeviathanUpdateProcessor processor = new LeviathanUpdateProcessor(store);
                        processor.ProcessCommandSet(updateSubject);
                        processor.ProcessCommandSet(updateObject);
                    }
                }
            }

            System.IO.StringWriter sw = new System.IO.StringWriter();
            RdfXmlWriter rdfXmlWriter = new RdfXmlWriter();
            rdfXmlWriter.Save(dataGraph, sw);
            string rdfXml = sw.ToString();
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(rdfXml));
            FormFile file = new FormFile(stream, 0, stream.Length, "rdfFile", "rdf.xml");

            //Actualizamos el item
            Dictionary<string, List<string>> discards = new Dictionary<string, List<string>>();
            foreach (DiscoverItem.DiscoverDissambiguation dissambiguation in item.DissambiguationProblems)
            {
                if (DissambiguationProblemsResolve.ContainsKey(dissambiguation.IDOrigin) && DissambiguationProblemsResolve[dissambiguation.IDOrigin] == null)
                {
                    discards.Add(dissambiguation.IDOrigin, dissambiguation.DissambiguationCandiates.Select(x => x.IDCandidate).ToList());
                }
            }

            item.UpdateDissambiguationDiscards(discards, rdfXml);
            item.DiscoverRdf = rdfXml;
            item.Status = "Pending";

            _discoverItemService.ModifyDiscoverItem(item);

            //Lo reencolamos corregido junto con su identificador
            _callEDtlPublishService.CallDataPublish(file, idJob, false, IdDiscoverItem);

            return RedirectToAction("DetailsJob", "Job", new { id = idJob });
        }

        /// <summary>
        /// Vuelve a encolar un problema de descubrimiento que haya fallado
        /// </summary>
        /// <param name="IdDiscoverItem">Identificador del item de descubrimiento</param>
        /// <param name="idJob">Identificador de la tarea a la que eprtenece el item de descubrimiento</param>
        /// <returns></returns>
        [HttpPost("[Controller]/{idJob}/retry/{IdDiscoverItem}")]
        public IActionResult RetryDiscover(string idJob, string IdDiscoverItem)
        {
            DiscoverItem item = _discoverItemService.GetDiscoverItemById(new Guid(IdDiscoverItem));
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(item.Rdf));
            FormFile file = new FormFile(stream, 0, stream.Length, "rdfFile", "rdf.xml");
            //Lo reencolamos sin corregir para que lo vuelva a intentar
            _callEDtlPublishService.CallDataPublish(file, idJob, false);

            //Eliminamos el item
            _discoverItemService.RemoveDiscoverItem(item.ID);
            return RedirectToAction("DetailsJob", "Job", new { id = idJob });
        }

        /// <summary>
        /// Descarta un problema de descubrimiento que haya fallado
        /// </summary>
        /// <param name="IdDiscoverItem">Identificador del item de descubrimiento</param>
        /// <param name="idJob">Identificador de la tarea a la que eprtenece el item de descubrimiento</param>
        /// <returns></returns>
        [HttpPost("[Controller]/{idJob}/discard/{IdDiscoverItem}")]
        public IActionResult DiscardDiscover(string idJob, string IdDiscoverItem)
        {
            //Eliminamos el item
            _discoverItemService.RemoveDiscoverItem(new Guid(IdDiscoverItem));
            return RedirectToAction("DetailsJob", "Job", new { id = idJob });

        }
    }
}