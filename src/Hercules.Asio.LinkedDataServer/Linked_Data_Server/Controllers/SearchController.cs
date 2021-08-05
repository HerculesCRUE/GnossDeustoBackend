using Hercules.Asio.LinkedDataServer.Utility;
using Linked_Data_Server.Models.Entities;
using Linked_Data_Server.Models.Services;
using Linked_Data_Server.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Query;

namespace Linked_Data_Server.Controllers
{
    public class SearchController : Controller
    {
        private readonly static ConfigService mConfigService = new ConfigService();
        private readonly static Config_Linked_Data_Server mLinked_Data_Server_Config = LoadLinked_Data_Server_Config();
        private readonly ISparqlUtility _sparqlUtility;
        private static RohGraph ontologyGraph;

        public SearchController(ISparqlUtility sparqlUtility)
        {
            _sparqlUtility = sparqlUtility;
        }
        [HttpGet]
        public IActionResult Index(string q,string concept,string etiqueta, int pagina)
        {
            SearchModelTemplate searchModelTemplate = GenerateSearchTemplate(q,concept, etiqueta, pagina);

            
            return View(searchModelTemplate);
        }
        [NonAction]
        public SearchModelTemplate GenerateSearchTemplate(string q, string concept,string etiqueta, int pagina)
        {
            string pXAppServer = "";
            RohGraph ontologyGraph = LoadGraph(mConfigService.GetOntologyGraph(), mConfigService, ref pXAppServer);
            SparqlResultSet sparqlResultSetNombresPropiedades = (SparqlResultSet)ontologyGraph.ExecuteQuery(
                @"select distinct ?entidad ?nombre lang(?nombre) as ?lang where 
                { 
                    ?entidad <http://www.w3.org/2000/01/rdf-schema#label> ?nombre. 
                }"
            );

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


            int numResultadosPagina = 10;
            ViewBag.UrlHome = mConfigService.GetUrlHome();
            SearchModelTemplate searchModelTemplate = new SearchModelTemplate();
            searchModelTemplate.entidades = new Dictionary<string, SearchModelTemplate.Entidad>();
            if (pagina == 0)
            {
                pagina = 1;
            }
            if (!string.IsNullOrEmpty(q))
            {
                //No buscamos en http://www.w3.org/2004/02/skos/core#prefLabel
                string consulta = @$" select * where
                                {{    
                                    select distinct ?s ?o ?rdfType where 
                                    {{      
                                        ?s a ?rdfType.
                                        {{
                                            FILTER(?p in (<{string.Join(">,<", mLinked_Data_Server_Config.PropsTitle.Except(new List<string> { "http://www.w3.org/2004/02/skos/core#prefLabel" }))}>))
                                            ?s ?p ?o.
                                            {SparqlUtility.GetSearchBuscador(q,"o","sc")}
                                        }}union
                                        {{
                                            FILTER(?p in (<{string.Join(">,<", mLinked_Data_Server_Config.PropsTitle.Except(new List<string> { "http://www.w3.org/2004/02/skos/core#prefLabel" }))}>))
                                            ?s ?p ?o.
                                            ?s <http://purl.org/roh#freetextSearch> ?search.
                                            {SparqlUtility.GetSearchBuscador(q,"search","sc2")}
                                        }}
                                    }}order by desc(?sc) desc(?sc2) asc(?o) asc (?s)
                                }} OFFSET {(pagina - 1) * numResultadosPagina} limit {numResultadosPagina} ";

                SparqlObject sparqlObject = _sparqlUtility.SelectData(mConfigService, mConfigService.GetSparqlGraph(), consulta, ref pXAppServer);
                foreach (Dictionary<string, SparqlObject.Data> row in sparqlObject.results.bindings)
                {
                    if (!searchModelTemplate.entidades.ContainsKey(row["s"].value))
                    {
                        string entityName = row["rdfType"].value;
                        if (communNamePropierties.ContainsKey(entityName))
                        {
                            entityName = communNamePropierties[entityName];
                        }
                        searchModelTemplate.entidades.Add(row["s"].value, new SearchModelTemplate.Entidad(row["o"].value, entityName));
                    }
                }
                //No buscamos en http://www.w3.org/2004/02/skos/core#prefLabel
                string consultaNumero = @$"     select count(distinct ?s) as ?num where 
                                    {{      
                                        ?s a ?rdfType.
                                        {{
                                            FILTER(?p in (<{string.Join(">,<", mLinked_Data_Server_Config.PropsTitle.Except(new List<string> { "http://www.w3.org/2004/02/skos/core#prefLabel" }))}>))
                                            ?s ?p ?o.
                                            {SparqlUtility.GetSearchBuscador(q,"o","sc")}
                                        }}union
                                        {{
                                            FILTER(?p in (<{string.Join(">,<", mLinked_Data_Server_Config.PropsTitle.Except(new List<string> { "http://www.w3.org/2004/02/skos/core#prefLabel" }))}>))
                                            ?s ?p ?o.
                                            ?s <http://purl.org/roh#freetextSearch> ?search.
                                            {SparqlUtility.GetSearchBuscador(q, "search","sc2")}
                                        }}
                                    }} ";
                SparqlObject sparqlObjectNumero = _sparqlUtility.SelectData(mConfigService, mConfigService.GetSparqlGraph(), consultaNumero, ref pXAppServer);
                searchModelTemplate.numResultados = int.Parse(sparqlObjectNumero.results.bindings[0]["num"].value);
                searchModelTemplate.numResultadosPagina = numResultadosPagina;
                searchModelTemplate.paginaActual = pagina;
                ViewData["Title"] = searchModelTemplate.numResultados + " Resultados para '" + q + "'";
            }
            else if(!string.IsNullOrEmpty(concept))
            {
                //Buscamos el nombre del área de conocimiento
                string nombreArea = concept;
                string consultaNombre = @$" 
                                    select distinct ?o where 
                                    {{      
                                        FILTER(?p in (<{string.Join(">,<", mLinked_Data_Server_Config.PropsTitle)}>))
                                        <{concept}> ?p ?o.
                                    }}";

                SparqlObject sparqlObject = _sparqlUtility.SelectData(mConfigService, mConfigService.GetSparqlGraph(), consultaNombre, ref pXAppServer);
                foreach (Dictionary<string, SparqlObject.Data> row in sparqlObject.results.bindings)
                {
                    nombreArea = row["o"].value;
                }

                //No buscamos en http://www.w3.org/2004/02/skos/core#prefLabel
                string consulta = @$" select * where
                                {{    
                                    select distinct ?s ?o ?rdfType where 
                                    {{      
                                        ?s a ?rdfType.
                                        FILTER(?p in (<{string.Join(">,<", mLinked_Data_Server_Config.PropsTitle.Except(new List<string> { "http://www.w3.org/2004/02/skos/core#prefLabel" }))}>))
                                        ?s ?p ?o.
                                        ?s <http://purl.org/roh#hasKnowledgeArea> <{concept}>.
                                    }}order by asc(?o) asc (?s)
                                }} OFFSET {(pagina - 1) * numResultadosPagina} limit {numResultadosPagina} ";

                sparqlObject = _sparqlUtility.SelectData(mConfigService, mConfigService.GetSparqlGraph(), consulta, ref pXAppServer);
                foreach (Dictionary<string, SparqlObject.Data> row in sparqlObject.results.bindings)
                {
                    if (!searchModelTemplate.entidades.ContainsKey(row["s"].value))
                    {
                        string entityName = row["rdfType"].value;
                        if (communNamePropierties.ContainsKey(entityName))
                        {
                            entityName = communNamePropierties[entityName];
                        }
                        searchModelTemplate.entidades.Add(row["s"].value, new SearchModelTemplate.Entidad(row["o"].value, entityName));
                    }
                }
                //No buscamos en http://www.w3.org/2004/02/skos/core#prefLabel
                string consultaNumero = @$"     select count(distinct ?s) as ?num where 
                                    {{      
                                        ?s a ?rdfType.
                                        FILTER(?p in (<{string.Join(">,<", mLinked_Data_Server_Config.PropsTitle.Except(new List<string> { "http://www.w3.org/2004/02/skos/core#prefLabel" }))}>))
                                        ?s ?p ?o.
                                        ?s <http://purl.org/roh#hasKnowledgeArea> <{concept}>.
                                    }} ";
                SparqlObject sparqlObjectNumero = _sparqlUtility.SelectData(mConfigService, mConfigService.GetSparqlGraph(), consultaNumero, ref pXAppServer);
                searchModelTemplate.numResultados = int.Parse(sparqlObjectNumero.results.bindings[0]["num"].value);
                searchModelTemplate.numResultadosPagina = numResultadosPagina;
                searchModelTemplate.paginaActual = pagina;
                ViewData["Title"] = searchModelTemplate.numResultados + " Resultados para el área de conocimiento '" + nombreArea + "'";
            }
            else if (!string.IsNullOrEmpty(etiqueta))
            {

                //No buscamos en http://www.w3.org/2004/02/skos/core#prefLabel
                string consulta = @$" select * where
                                {{    
                                    select distinct ?s ?o ?rdfType where 
                                    {{      
                                        ?s a ?rdfType.
                                        FILTER(?p in (<{string.Join(">,<", mLinked_Data_Server_Config.PropsTitle.Except(new List<string> { "http://www.w3.org/2004/02/skos/core#prefLabel" }))}>))
                                        ?s ?p ?o.
                                        ?s <http://purl.org/roh/mirror/vivo#freetextKeyword> ?tag
                                        FILTER(lcase(str(?tag))='{etiqueta}')
                                    }}order by asc(?o) asc (?s)
                                }} OFFSET {(pagina - 1) * numResultadosPagina} limit {numResultadosPagina} ";

                SparqlObject sparqlObject = _sparqlUtility.SelectData(mConfigService, mConfigService.GetSparqlGraph(), consulta, ref pXAppServer);
                foreach (Dictionary<string, SparqlObject.Data> row in sparqlObject.results.bindings)
                {
                    if (!searchModelTemplate.entidades.ContainsKey(row["s"].value))
                    {
                        string entityName = row["rdfType"].value;
                        if (communNamePropierties.ContainsKey(entityName))
                        {
                            entityName = communNamePropierties[entityName];
                        }
                        searchModelTemplate.entidades.Add(row["s"].value, new SearchModelTemplate.Entidad(row["o"].value, entityName));
                    }
                }
                //No buscamos en http://www.w3.org/2004/02/skos/core#prefLabel
                string consultaNumero = @$"     select count(distinct ?s) as ?num where 
                                    {{      
                                        ?s a ?rdfType.
                                        FILTER(?p in (<{string.Join(">,<", mLinked_Data_Server_Config.PropsTitle.Except(new List<string> { "http://www.w3.org/2004/02/skos/core#prefLabel" }))}>))
                                        ?s ?p ?o.
                                        ?s <http://purl.org/roh/mirror/vivo#freetextKeyword> ?tag
                                        FILTER(lcase(str(?tag))='{etiqueta}')
                                    }} ";
                SparqlObject sparqlObjectNumero = _sparqlUtility.SelectData(mConfigService, mConfigService.GetSparqlGraph(), consultaNumero, ref pXAppServer);
                searchModelTemplate.numResultados = int.Parse(sparqlObjectNumero.results.bindings[0]["num"].value);
                searchModelTemplate.numResultadosPagina = numResultadosPagina;
                searchModelTemplate.paginaActual = pagina;
                ViewData["Title"] = searchModelTemplate.numResultados + " Resultados para la etiqueta '" + etiqueta + "'";
            }


            return searchModelTemplate;
        }


        /// <summary>
        /// Cargamos las configuraciones 
        /// </summary>
        /// <returns></returns>
        private static Config_Linked_Data_Server LoadLinked_Data_Server_Config()
        {
            return JsonConvert.DeserializeObject<Config_Linked_Data_Server>(System.IO.File.ReadAllText("Config/Linked_Data_Server_Config.json"));
        }

        private RohGraph LoadGraph(string pGraph, ConfigService pConfigService, ref string pXAppServer)
        {
            if (ontologyGraph != null)
            {
                return ontologyGraph;
            }
            else
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
