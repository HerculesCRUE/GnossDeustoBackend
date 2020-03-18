using System;
using System.Collections.Generic;
using System.Linq;

namespace OaiPmhNet.Models.OAIPMH
{
    public class CVN
    {
        
        public CVN(string pRDF)
        {
            rdf_cvn = pRDF;
        }

        public string rdf_cvn { get; }

        public string oai_dc {
            get
            {
                if(!string.IsNullOrEmpty(rdf_cvn))
                {
                    return rdf_cvn;
                }
                throw new Exception("Los datos RDF no existen");
            }
        }

        public int Id { get; }
        public DateTime Date { get; }
        public string Name { get; }
    }
}
