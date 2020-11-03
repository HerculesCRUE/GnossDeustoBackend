// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using VDS.RDF;
using VDS.RDF.Parsing;
using Linked_Data_Server.Models.Entities;

namespace Linked_Data_Server.Utility
{
    public static class SparqlUtility
    {
        public static SparqlObject SelectData(string pSPARQLEndpoint, string pGraph, string pConsulta, string pQueryParam)
        {
            SparqlObject datosDBpedia = null;
            string urlConsulta = pSPARQLEndpoint;
            WebClient webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");


            NameValueCollection parametros = new NameValueCollection();
            if (string.IsNullOrEmpty(pGraph))
            {
                parametros.Add("default-graph-uri", pGraph);
            }
            parametros.Add(pQueryParam, pConsulta);
            parametros.Add("format", "application/sparql-results+json");
           
            byte[] responseArray = null;
            int numIntentos = 0;
            Exception exception = null;
            while (responseArray == null && numIntentos < 5)
            {
                numIntentos++;
                try
                {
                    responseArray = webClient.UploadValues(urlConsulta, "POST", parametros);
                    exception = null;
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }
            if(exception!=null)
            {
                throw exception;
            }
            string jsonRespuesta = System.Text.Encoding.UTF8.GetString(responseArray);

            if (!string.IsNullOrEmpty(jsonRespuesta))
            {
                datosDBpedia = JsonConvert.DeserializeObject<SparqlObject>(jsonRespuesta);
            }
            return datosDBpedia;
        }
    }
}
