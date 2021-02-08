using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VDS.RDF;

namespace Api_Unidata.Models.Services
{
    public class SparqlUtility
    {
        public static void LoadTriples(List<string> triplesInsert)
        {
            ConfigSparql config = new ConfigSparql();
            InsertData(config.GetEndpointUnidata(), config.GetGraphUnidata(), triplesInsert, config.GetQueryParam());
        }

        private static void InsertData(string pSPARQLEndpoint, string pGraph, List<string> triplesInsert, string pQueryParam)
        {
            string query = "";
            query += $" INSERT INTO <{pGraph}>";
            query += " { ";
            query += string.Join(" ", triplesInsert);
            query += " } ";

            string url = pSPARQLEndpoint;
            if (string.IsNullOrEmpty(url))
            {
                Graph graph = new Graph();
                graph.LoadFromString(string.Join(" ", triplesInsert));
            }
            else
            {
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
