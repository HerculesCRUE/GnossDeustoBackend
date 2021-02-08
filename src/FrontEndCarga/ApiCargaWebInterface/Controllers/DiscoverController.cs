// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Controlador para gestionar el descubrimiento
using ApiCargaWebInterface.Models.Entities;
using ApiCargaWebInterface.Models.Services;
using ApiCargaWebInterface.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace ApiCargaWebInterface.Controllers
{
    /// <summary>
    /// Controlador para gestionar el descubrimiento
    /// </summary>
    public class DiscoverController : Controller
    {
        readonly DiscoverItemBDService _discoverItemService;
        readonly ICallEtlService _callEDtlPublishService;
        public DiscoverController(DiscoverItemBDService iIDiscoverItemService, ICallEtlService callEDtlPublishService)
        {
            _discoverItemService = iIDiscoverItemService;
            _callEDtlPublishService = callEDtlPublishService;
        }
        /// <summary>
        /// Obtiene los detalles de un error de descubrimiento
        /// </summary>
        /// <returns></returns>
        public IActionResult Details(Guid itemId)
        {
            var discovery = _discoverItemService.GetDiscoverItemById(itemId);
            DiscoverItemViewModel model = new DiscoverItemViewModel();
            model.DissambiguationProblems = new Dictionary<string, List<string>>();
            model.DissambiguationProblemsTitles = new Dictionary<string, string>();
            if (discovery.Status.Equals("Error"))
            {
                model.Error = discovery.Error;
                model.JobId = discovery.JobID;
                model.IdDiscoverItem = discovery.ID;
            }
            else
            {
                RohGraph dataGraph = new RohGraph();
                dataGraph.LoadFromString(discovery.DiscoverRdf, new RdfXmlParser());
                model.JobId = discovery.JobID;

                foreach (var item in discovery.DissambiguationProblems)
                {
                    model.IdDiscoverItem = item.DiscoverItemID;
                    if (!model.DissambiguationProblems.ContainsKey(item.IDOrigin))
                    {
                        model.DissambiguationProblems.Add(item.IDOrigin, new List<string>());
                        model.DissambiguationProblemsTitles.Add(item.IDOrigin, "");
                        SparqlResultSet sparqlResultSet = (SparqlResultSet)dataGraph.ExecuteQuery("select ?title where{<" + item.IDOrigin + "> ?prop ?title. FILTER(?prop in (<http://purl.org/roh#title>,<http://purl.org/roh/mirror/foaf#name>))}");
                        foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
                        {
                            model.DissambiguationProblemsTitles[item.IDOrigin] = ((LiteralNode)(sparqlResult["title"])).Value;
                        }
                    }
                    foreach (var problem in item.DissambiguationCandiates)
                    {
                        string opcion = $"{problem.IDCandidate} || {Math.Round(problem.Score, 3)}";
                        model.DissambiguationProblems[item.IDOrigin].Add(opcion);
                    }
                }
            }
            return View(model);
        }

        /// <summary>
        /// Obtiene los detalles de un rdf
        /// </summary>
        /// <returns></returns>
        public IActionResult DetailsRdf(Guid itemId)
        {
            //Cargamos la ontología
            RohGraph ontologyGraph = new RohGraph();
            ontologyGraph = _callEDtlPublishService.CallGetOntology();

            SparqlResultSet sparqlResultSetNombresPropiedades = (SparqlResultSet)ontologyGraph.ExecuteQuery("select distinct ?entidad ?nombre where { ?entidad <http://www.w3.org/2000/01/rdf-schema#label> ?nombre. FILTER(lang(?nombre) = 'es')}");

            //Guardamos todos los nombres de las propiedades en un diccionario
            Dictionary<string, string> communNamePropierties = new Dictionary<string, string>();
            foreach (SparqlResult sparqlResult in sparqlResultSetNombresPropiedades.Results)
            {
                communNamePropierties.Add(sparqlResult["entidad"].ToString(), ((LiteralNode)(sparqlResult["nombre"])).Value);
            }

            //Cargamos los datos
            DiscoverItem discoveryGraph = _discoverItemService.GetDiscoverItemById(itemId);
            RohGraph dataGraph = new RohGraph();
            dataGraph.LoadFromString(discoveryGraph.DiscoverRdf, new RdfXmlParser());

            //Guardamos todas las entidades que no son blankNodes
            List<String> entities = new List<string>();
            SparqlResultSet sparqlResultSetListaEntidadesNotBN = (SparqlResultSet)dataGraph.ExecuteQuery("select ?s count(?p) as ?num where { ?s ?p ?o. FILTER (!isBlank(?s)) }group by ?s order by desc(?num) ");
            foreach (SparqlResult sparqlResult in sparqlResultSetListaEntidadesNotBN.Results)
            {
                entities.Add(sparqlResult["s"].ToString());
            }
            List<DiscoverRdfViewModel> model = new List<DiscoverRdfViewModel>();

            //Guardamos todas las entidades
            List<String> allEntities = new List<string>();
            SparqlResultSet sparqlResultSetEntidades = (SparqlResultSet)dataGraph.ExecuteQuery("select distinct ?s where { ?s ?p ?o }");
            foreach (SparqlResult sparqlResult in sparqlResultSetEntidades.Results)
            {
                allEntities.Add(sparqlResult["s"].ToString());
            }
            SparqlResultSet sparqlResultSet = (SparqlResultSet)dataGraph.ExecuteQuery("select ?s ?p ?o where { ?s ?p ?o }");
            Dictionary<string, List<SparqlResult>> entitySparqlResult = new Dictionary<string, List<SparqlResult>>();
            foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
            {
                if(!entitySparqlResult.ContainsKey(sparqlResult["s"].ToString()))
                {
                    entitySparqlResult.Add(sparqlResult["s"].ToString(), new List<SparqlResult>());
                }
                entitySparqlResult[sparqlResult["s"].ToString()].Add(sparqlResult);
            }

            foreach (var idEntity in entities)
            {
                DiscoverRdfViewModel entidad = createDiscoverRdfViewModel(idEntity, entitySparqlResult, new List<string>(), allEntities, communNamePropierties, discoveryGraph.LoadedEntities);
                model.Add(entidad);
            }
            return View(model);
        }

        /// <summary>
        /// Crea un modelo DiscoverRdfViewModel
        /// </summary>
        /// <param name="idEntity">Identificador de la entidad de la que crear el modelo</param>
        /// <param name="entitySparqlResult">SparqlResult con todos los triples agrupados por el sujeto</param>
        /// <param name="parents">Lista de ancestros de la entidad</param>
        /// <param name="allEntities">Listado con todos los identificadores del RDF</param>
        /// <param name="communNameProperties">Diccionario con los nombres de las propiedades</param>
        /// <param name="loadedEntities">Lista de entidades cargadas en el triple store</param>
        /// <returns></returns>
        public DiscoverRdfViewModel createDiscoverRdfViewModel(string idEntity, Dictionary<string, List<SparqlResult>> entitySparqlResult, List<string> parents, List<string> allEntities, Dictionary<string, string> communNameProperties, List<string> loadedEntities)
        {
            //Obtenemos todos los triples de la entidad
            DiscoverRdfViewModel entidad = new DiscoverRdfViewModel();
            entidad.stringPropertiesEntity = new Dictionary<string, List<string>>();
            entidad.entitiesPropertiesEntity = new Dictionary<string, List<DiscoverRdfViewModel>>();
            entidad.uriEntity = idEntity;
            entidad.urisRdf = allEntities;
            entidad.communNamePropierties = communNameProperties;
            entidad.LoadedEntities = loadedEntities;
            if (entidad.LoadedEntities == null)
            {
                entidad.LoadedEntities = new List<string>();
            }
            if (entitySparqlResult.ContainsKey(idEntity))
            {
                foreach (SparqlResult sparqlResult in entitySparqlResult[idEntity])
                {

                    if (sparqlResult["o"] is BlankNode && !parents.Contains(sparqlResult["o"].ToString()))
                    {
                        if (!entidad.entitiesPropertiesEntity.ContainsKey(sparqlResult["p"].ToString()))
                        {
                            //Añadimos la propiedad a 'entitiesPropertiesEntity'
                            entidad.entitiesPropertiesEntity.Add(sparqlResult["p"].ToString(), new List<DiscoverRdfViewModel>());
                        }
                        parents.Add(idEntity);
                        entidad.entitiesPropertiesEntity[sparqlResult["p"].ToString()].Add(createDiscoverRdfViewModel(sparqlResult["o"].ToString(), entitySparqlResult, parents, allEntities, communNameProperties, loadedEntities));
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
            }
            return entidad;
        }
    }
}