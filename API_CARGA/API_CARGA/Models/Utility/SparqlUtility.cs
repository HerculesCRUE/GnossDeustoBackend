using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace API_CARGA.Models.Utility
{
    public static class SparqlUtility
    {
        public static XmlDocument GetRDFFromFile(IFormFile pRDFFile)
        {
            var result = new StringBuilder();
            using (var reader = new StreamReader(pRDFFile.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                    result.AppendLine(reader.ReadLine());
            }
            string xml = result.ToString();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            return xmlDoc;
        }

        public static List<string> GetTriplesFromRDF(XmlDocument pXMLRDF)
        {
            List<string> triples = new List<string>();

            XmlNode nodoRDF = pXMLRDF.GetElementsByTagName("rdf:RDF")[0];
            foreach (XmlNode entidad in nodoRDF.ChildNodes)
            {
                string sujeto = entidad.Attributes["rdf:about"].Value;
                string rdfType = entidad.NamespaceURI + entidad.LocalName;
                triples.Add($"<{sujeto}>    <http://www.w3.org/1999/02/22-rdf-syntax-ns#type>   <{rdfType}>.");
                foreach (XmlNode tripe in entidad.ChildNodes)
                {
                    string propiedad = tripe.NamespaceURI + tripe.LocalName;
                    if (tripe.Attributes["rdf:resource"] != null)
                    {
                        string entidadobjeto = tripe.Attributes["rdf:resource"].Value;
                        triples.Add($"<{sujeto}>    <{propiedad}>   <{entidadobjeto}>.");
                    }
                    else
                    {
                        string objeto = tripe.InnerText.Replace("\"", "\\\"");
                        string dataType = "";

                        string lang = "";
                        if (tripe.Attributes["xml:lang"] != null)
                        {
                            lang = "@" + tripe.Attributes["xml:lang"].Value;
                        }
                        else if (tripe.Attributes["rdf:datatype"] != null)
                        {
                            dataType = "^^<" + tripe.Attributes["rdf:datatype"].Value + ">";
                            if(tripe.Attributes["rdf:datatype"].Value=="http://www.w3.org/2001/XMLSchema#float")
                            {
                                objeto = objeto.Replace(",", ".");
                            }
                        }
                        triples.Add($"<{sujeto}>    <{propiedad}>   \"{objeto}\"{lang}{dataType}.");
                    }
                }
            }
            return triples;
        }

        public static void LoadTriples( List<string> pTriples, string pSPARQLEndpoint, string pQueryParam, string pGraph)
        {
            int maxTriples = 500;

            List<List<string>> list = new List<List<string>>();
            for (int i = 0; i < pTriples.Count; i += maxTriples)
            {
                list.Add(pTriples.GetRange(i, Math.Min(maxTriples, pTriples.Count - i)));
            }

            foreach (List<string> sublist in list)
            {
                string query = "";
                query += $" INSERT INTO <{pGraph}>";
                query += " { ";
                query += string.Join(" ", sublist);
                query += " } ";

                string url = pSPARQLEndpoint;
                NameValueCollection parametros = new NameValueCollection();
                parametros.Add(pQueryParam, query);
                WebClient webClient = new WebClient();
                try
                {
                    webClient.UploadValues(url, "POST", parametros);
                }
                catch (WebException ex)
                {
                    if (ex.Response != null)
                    {
                        string response = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                        throw new Exception(response);
                    }
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    webClient.Dispose();
                }
            }
        }
    }
}
