using API_DISCOVER.Models.Entities;
using API_DISCOVER.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace API_DISCOVER.Models.Services
{
    public class SparqlUtilityMock : I_SparqlUtility
    {
        public SparqlObject SelectData(string pSPARQLEndpoint, string pGraph, string pConsulta, string pQueryParam)
        {
            SparqlObject sparqlObject = new SparqlObject();
            sparqlObject.results = new SparqlObject.Results();
            sparqlObject.results.bindings = new List<Dictionary<string, SparqlObject.Data>>();
            return sparqlObject;
        }
    }
}
