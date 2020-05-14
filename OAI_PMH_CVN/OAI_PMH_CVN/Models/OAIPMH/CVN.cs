using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace OaiPmhNet.Models.OAIPMH
{
    /// <summary>
    /// Objeto con la información correspondiente al CVN
    /// </summary>
    public class CVN
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pXML_CVN">XML del CVN</param>
        /// <param name="pId">Identificador del CVN</param>
        /// <param name="pRutaCVN_ROH_converter">Ruta del servicio convertidor de CVN a RDF ROH</param>
        public CVN(string pXML_CVN, string pId, string pRutaCVN_ROH_converter)
        {
            if (!string.IsNullOrEmpty(pXML_CVN))
            {
                string id_Aux = pId;
                while (id_Aux.Length<4)
                {
                    id_Aux = "0" + id_Aux;
                }
                var client = new RestClient($"{ pRutaCVN_ROH_converter }?orcid=0000-0001-8055-{id_Aux}");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("application/x-www-form-urlencoded; charset=UTF-8", pXML_CVN, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                if(response.StatusCode!=System.Net.HttpStatusCode.OK)
                {
                    throw new Exception("Se ha producido un error al generar el RDF");
                }
                rdf = response.Content;
            }
            Id = pId.ToString();           
        }

        /// <summary>
        /// RDF del CVN
        /// </summary>
        public string rdf { get; }

        /// <summary>
        /// Identificador
        /// </summary>
        public string Id { get; }
    }
}
