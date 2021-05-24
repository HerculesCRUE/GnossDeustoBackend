using API_DISCOVER.Models.Entities;
using API_DISCOVER.Models.Services;
using System.Collections.Generic;
using VDS.RDF;
using VDS.RDF.Query;

namespace API_DISCOVER.Utility
{
    public class SparqlUtilityMock : I_SparqlUtility
    {
        private RohGraph _dataGraph;

        public SparqlUtilityMock(RohGraph dataGraph)
        {
            _dataGraph = dataGraph;
        }

        public SparqlObject SelectData(RabbitMQService pRabbitMQService, string pSPARQLEndpoint, string pGraph, string pConsulta, string pQueryParam,string pUsername,string pPassword)
        {
            SparqlResultSet sparqlResultSet = (SparqlResultSet)_dataGraph.ExecuteQuery(pConsulta.ToString());
            SparqlObject sparqlObject = new SparqlObject();
            sparqlObject.results = new SparqlObject.Results();
            sparqlObject.results.bindings = new List<Dictionary<string, SparqlObject.Data>>();
            foreach(var sparqlResult in sparqlResultSet)
            {
                Dictionary<string, SparqlObject.Data> dict = new Dictionary<string, SparqlObject.Data>();
                foreach (var variable in sparqlResult.Variables)
                {
                    SparqlObject.Data data = new SparqlObject.Data();
                    if(sparqlResult[variable] != null)
                    {
                        data.type = variable;
                        if(sparqlResult[variable] is LiteralNode)
                        {
                            data.value = ((LiteralNode)(sparqlResult[variable])).Value;
                            if(((LiteralNode)sparqlResult[variable]).DataType != null)
                            {
                                data.datatype = ((LiteralNode)sparqlResult[variable]).DataType.ToString();
                            }
                        }
                        else
                        {
                            data.value = sparqlResult[data.type].ToString();
                        }
                        dict.Add(data.type, data);
                    }
                }
                
                sparqlObject.results.bindings.Add(dict);
            }
            return sparqlObject;
        }
    }
}
