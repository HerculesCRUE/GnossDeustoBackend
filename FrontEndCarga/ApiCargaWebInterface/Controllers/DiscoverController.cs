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
        public DiscoverController(DiscoverItemBDService iIDiscoverItemService)
        {
            _discoverItemService = iIDiscoverItemService;
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
            ontologyGraph.LoadFromFile("Config/Ontology/roh-v2.owl");
            SparqlResultSet sparqlResultSet3 = (SparqlResultSet)ontologyGraph.ExecuteQuery("select distinct ?entidad ?nombre where { ?entidad <http://www.w3.org/2000/01/rdf-schema#label> ?nombre. FILTER(lang(?nombre) = 'es')}");
            
            //Guardamos todos los nombres de las propiedades en un diccionario
            Dictionary<string, string> communNamePropierties = new Dictionary<string, string>();
            foreach (SparqlResult sparqlResult in sparqlResultSet3.Results)
            {
                communNamePropierties.Add(sparqlResult["entidad"].ToString(), ((LiteralNode)(sparqlResult["nombre"])).Value);
            }

            //Cargamos los datos
            DiscoverItem discovery = _discoverItemService.GetDiscoverItemById(itemId);
            discovery.LoadedEntities.Add("http://graph.um.es/res/article/00c26ede-f2f1-4fda-b3fe-96fb1759aaf8");
            discovery.LoadedEntities.Add("http://graph.um.es/res/article/04587ab5-5f6b-4da9-8297-ef086490b003");
            RohGraph dataGraph = new RohGraph();
            dataGraph.LoadFromString(discovery.DiscoverRdf, new RdfXmlParser());

            //Guardamos todas las entidades menos blankNodes
            List<String> entities = new List<string>();
            SparqlResultSet sparqlResultSet = (SparqlResultSet)dataGraph.ExecuteQuery("select ?s count(?p) as ?num where { ?s ?p ?o. FILTER (!isBlank(?s)) }group by ?s order by desc(?num) ");
            foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
            {
                entities.Add(sparqlResult["s"].ToString());
            }
            List<DiscoverRdfViewModel> model = new List<DiscoverRdfViewModel>();

            //Guardamos todas las entidades
            List<String> allEntities = new List<string>();
            SparqlResultSet sparqlResultSet2 = (SparqlResultSet)dataGraph.ExecuteQuery("select distinct ?s where { ?s ?p ?o }");
            foreach (SparqlResult sparqlResult2 in sparqlResultSet2.Results)
            {
                allEntities.Add(sparqlResult2["s"].ToString());
            }

            foreach (var idEntity in entities)
            {
                DiscoverRdfViewModel entidad = createDiscoverRdfViewModel(idEntity, dataGraph, new List<string>(), allEntities, communNamePropierties, discovery.LoadedEntities);
                model.Add(entidad);
            }
            return View(model);
        }

        /// <summary>
        /// Crea un rdf 
        /// </summary>
        /// <returns></returns>
        public DiscoverRdfViewModel createDiscoverRdfViewModel(string idEntity, RohGraph dataGraph,List<string> parents, List<String> allEntities, Dictionary<String, String> communNamePropierties, List<string> loadedEntities)
        {
            SparqlResultSet sparqlResultSet = (SparqlResultSet)dataGraph.ExecuteQuery("select ?p ?o where { <" + idEntity + "> ?p ?o }");
            DiscoverRdfViewModel entidad = new DiscoverRdfViewModel();
            entidad.stringPropertiesEntity = new Dictionary<string, List<string>>();
            entidad.entitiesPropertiesEntity = new Dictionary<string, List<DiscoverRdfViewModel>>();
            entidad.uriEntity = idEntity;
            entidad.urisRdf = allEntities;
            entidad.communNamePropierties = communNamePropierties;
            entidad.LoadedEntities = loadedEntities;

            foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
            {
                if (!(sparqlResult["o"] is BlankNode) && !entidad.stringPropertiesEntity.ContainsKey(sparqlResult["p"].ToString()))
                {
                    entidad.stringPropertiesEntity.Add(sparqlResult["p"].ToString(), new List<string>());
                }
                if ((sparqlResult["o"] is BlankNode) && !entidad.entitiesPropertiesEntity.ContainsKey(sparqlResult["p"].ToString()) )
                {
                    if (parents.Contains(sparqlResult["o"].ToString()))
                    {
                        entidad.stringPropertiesEntity.Add(sparqlResult["p"].ToString(), new List<string>());
                    }
                    else
                    {
                        entidad.entitiesPropertiesEntity.Add(sparqlResult["p"].ToString(), new List<DiscoverRdfViewModel>());
                    }
                }

                if (sparqlResult["o"] is LiteralNode)
                {
                    entidad.stringPropertiesEntity[sparqlResult["p"].ToString()].Add(((LiteralNode)(sparqlResult["o"])).Value);
                }
                else if ((sparqlResult["o"] is BlankNode) && !parents.Contains(sparqlResult["o"].ToString()))
                {
                    var childEntity = sparqlResult["o"].ToString();
                    parents.Add(idEntity);
                    entidad.entitiesPropertiesEntity[sparqlResult["p"].ToString()].Add(createDiscoverRdfViewModel(childEntity, dataGraph, parents, allEntities, communNamePropierties, loadedEntities));
                }
                else
                {
                    entidad.stringPropertiesEntity[sparqlResult["p"].ToString()].Add(sparqlResult["o"].ToString());
                }
            }
            return entidad;
        }
    }
}