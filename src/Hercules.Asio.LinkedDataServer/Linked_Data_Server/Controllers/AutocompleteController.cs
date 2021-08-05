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
using System.Web;
using Newtonsoft.Json;
using Hercules.Asio.LinkedDataServer.Utility;

namespace Linked_Data_Server.Controllers
{

    public class AutocompleteController : Controller
    {
        private readonly static ConfigService mConfigService = new ConfigService();
        private readonly static Config_Linked_Data_Server mLinked_Data_Server_Config = LoadLinked_Data_Server_Config();
        private readonly ILogger<HomeController> _logger;
        private readonly ISparqlUtility _sparqlUtility;
        private static RohGraph ontologyGraph;

        public AutocompleteController(ISparqlUtility sparqlUtility, ILogger<HomeController> logger)
        {
            _logger = logger;
            _sparqlUtility = sparqlUtility;
        }


        public IActionResult Index(string q)
        {
            string pXAppServer = "";
            //Cargamos la ontología y obtenemos la afinidad
            RohGraph ontologyGraph = LoadGraph(mConfigService.GetOntologyGraph(), mConfigService, ref pXAppServer);
            SparqlResultSet sparqlResultSetNombresPropiedades = (SparqlResultSet)ontologyGraph.ExecuteQuery(@"select distinct ?entidad ?nombre lang(?nombre) as ?lang where 
                                                                                                            { 
                                                                                                                ?entidad <http://www.w3.org/2000/01/rdf-schema#label> ?nombre. 
                                                                                                            }");
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

            List<KeyValuePair<string, string>> response = new List<KeyValuePair<string, string>>();
            string searchAutocompletar = SparqlUtility.GetSearchAutocompletar(q);
            if (!string.IsNullOrEmpty(searchAutocompletar))
            {
                Dictionary<int, List<KeyValuePair<string, string>>> listaAutocompletar = new Dictionary<int, List<KeyValuePair<string, string>>>();
                {
                    string consulta = @$"     select distinct ?s ?sc ?o ?rdfType where 
                                    {{                                        
                                        FILTER(?p in (<{string.Join(">,<", mLinked_Data_Server_Config.PropsTitle)}>))
                                        ?s a ?rdfType.
                                        ?s ?p ?o.
                                        {searchAutocompletar}
                                    }}order by desc(?sc) asc(?o) asc (?s) limit 10 ";
                    SparqlObject sparqlObject = _sparqlUtility.SelectData(mConfigService, mConfigService.GetSparqlGraph(), consulta, ref pXAppServer);
                    foreach (Dictionary<string, SparqlObject.Data> row in sparqlObject.results.bindings)
                    {
                        string rdftypename = row["rdfType"].value;
                        int score = int.Parse(row["sc"].value);
                        if(!listaAutocompletar.ContainsKey(score))
                        {
                            listaAutocompletar.Add(score,new List<KeyValuePair<string, string>>());
                        }
                        if (communNamePropierties.ContainsKey(rdftypename))
                        {
                            rdftypename = communNamePropierties[rdftypename];
                        }
                        //Las categorías tienen otro comportamiento (revisar categorías no unesko)
                        if (row["rdfType"].value == "http://www.w3.org/2004/02/skos/core#Concept")
                        {
                            string url = $"{Request.Scheme}://{Request.Host}/Search?concept={row["s"].value}";
                            listaAutocompletar[score].Add(new KeyValuePair<string, string>($"{ row["o"].value } - {rdftypename}", url));
                        }
                        else
                        {
                            listaAutocompletar[score].Add(new KeyValuePair<string, string>($"{row["o"].value} - {rdftypename}", row["s"].value));
                        }
                    }
                }
                {
                    string consultaTags = @$"     select distinct lcase(?o) as ?o ?sc where 
                                    {{  
                                        ?s <http://purl.org/roh/mirror/vivo#freetextKeyword> ?o.
                                        {searchAutocompletar}
                                    }}order by desc(?sc) asc(?o) limit 10 ";
                    SparqlObject sparqlObject = _sparqlUtility.SelectData(mConfigService, mConfigService.GetSparqlGraph(), consultaTags, ref pXAppServer);
                    foreach (Dictionary<string, SparqlObject.Data> row in sparqlObject.results.bindings)
                    {
                        int score = int.Parse(row["sc"].value);
                        if (!listaAutocompletar.ContainsKey(score))
                        {
                            listaAutocompletar.Add(score, new List<KeyValuePair<string, string>>());
                        }
                        string url = $"{Request.Scheme}://{Request.Host}/Search?etiqueta={row["o"].value.ToLower()}";
                        listaAutocompletar[score].Add(new KeyValuePair<string, string>($"{ row["o"].value.ToLower() } - Palabra clave", url));
                    }
                }
                int num = 0;
                foreach(List<KeyValuePair<string, string>> list in listaAutocompletar.OrderByDescending(x=>x.Key).ToDictionary(x=>x.Key,y=>y.Value).Values)
                {
                    foreach(KeyValuePair<string, string> item in list)
                    {
                        if(num==10)
                        {
                            break;
                        }
                        num++;
                        response.Add(item);
                    }
                }

            }
            return Json(response);
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
