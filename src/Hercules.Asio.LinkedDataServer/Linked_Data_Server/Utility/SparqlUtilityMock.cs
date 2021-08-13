using Linked_Data_Server.Models.Entities;
using Linked_Data_Server.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Query;

namespace Hercules.Asio.LinkedDataServer.Utility
{
    public class SparqlUtilityMock : ISparqlUtility
    {
        public RohGraph _dataGraph;

        public SparqlUtilityMock(RohGraph dataGraph)
        {
            _dataGraph = dataGraph;
        }

        public SparqlObject SelectData(ConfigService pConfigService, string pGraph, string pConsulta, ref string pXAppServer)
        {
            SparqlResultSet sparqlResultSet = (SparqlResultSet)_dataGraph.ExecuteQuery(pConsulta.ToString());
            SparqlObject sparqlObject = new SparqlObject();
            sparqlObject.results = new SparqlObject.Results();
            sparqlObject.results.bindings = new List<Dictionary<string, SparqlObject.Data>>();
            sparqlObject.head = new SparqlObject.Head();
            sparqlObject.head.vars = new HashSet<string>();

            foreach (var sparqlResult in sparqlResultSet)
            {
                Dictionary<string, SparqlObject.Data> dict = new Dictionary<string, SparqlObject.Data>();
                foreach (var variable in sparqlResult.Variables)
                {
                    sparqlObject.head.vars.Add(variable);
                    SparqlObject.Data data = new SparqlObject.Data();
                    if (sparqlResult[variable] != null)
                    {
                        data.type = sparqlResult[variable].NodeType.ToString().ToLower();
                        if (data.type == "blank")
                        {
                            data.type = "bnode";
                        }
                        if (sparqlResult[variable] is LiteralNode)
                        {
                            data.value = ((LiteralNode)(sparqlResult[variable])).Value;
                            if (((LiteralNode)sparqlResult[variable]).DataType != null)
                            {
                                data.datatype = ((LiteralNode)sparqlResult[variable]).DataType.ToString();
                            }
                        }
                        else
                        {
                            data.value = sparqlResult[variable].ToString();
                        }
                        //bnode uri typed-literal literal
                        dict.Add(variable, data);
                    }
                }

                sparqlObject.results.bindings.Add(dict);
            }
            return sparqlObject;
        }

        public string GetSearchAutocompletar(string pText)
        {
            return $"FILTER(regex(lcase(?o), '^{pText.ToLower()}') || regex(lcase(?o), ' {pText.ToLower()}')) bind(1 as ?sc)";
        }

        public string GetSearchBuscador(string pText, string pVar, string pScoreVar)
        {
            return $"FILTER(regex(lcase(?o), '^{pText.ToLower()}') || regex(lcase(?o), ' {pText.ToLower()}')) bind(1 as ?sc)";
        }
    }
}
