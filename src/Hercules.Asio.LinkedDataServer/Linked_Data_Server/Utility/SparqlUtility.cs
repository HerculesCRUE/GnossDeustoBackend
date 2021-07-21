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
using Linked_Data_Server.Models.Services;
using System.Diagnostics.CodeAnalysis;
using Hercules.Asio.LinkedDataServer.Utility;
using System.Globalization;

namespace Linked_Data_Server.Utility
{
    [ExcludeFromCodeCoverage]
    public class SparqlUtility : ISparqlUtility
    {
        public SparqlObject SelectData(ConfigService pConfigService, string pGraph, string pConsulta, ref string pXAppServer)
        {
            SparqlObject datosSparql = null;
            string urlConsulta = pConfigService.GetSparqlEndpoint();
            if(!string.IsNullOrEmpty(pXAppServer))
            {
                if (pConfigService.GetXAppServer1()==pXAppServer)
                {
                    urlConsulta = pConfigService.GetSparqlEndpoint1();
                }else if (pConfigService.GetXAppServer2() == pXAppServer)
                {
                    urlConsulta = pConfigService.GetSparqlEndpoint2();
                }
            }
            WebClient webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");


            NameValueCollection parametros = new NameValueCollection();
            if (!string.IsNullOrEmpty(pGraph))
            {
                parametros.Add("default-graph-uri", pGraph);
            }
            parametros.Add(pConfigService.GetSparqlQueryParam(), pConsulta);
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
                    if (webClient.ResponseHeaders["X-App-Server"] != null)
                    {
                        pXAppServer = webClient.ResponseHeaders["X-App-Server"];
                    }
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
                datosSparql = JsonConvert.DeserializeObject<SparqlObject>(jsonRespuesta);
            }

            webClient.Dispose();
            return datosSparql;
        }

        public static string GetSearchAutocompletar(string pText)
        {
            pText=string.Join("", pText.Where(x=>char.IsLetterOrDigit(x) || char.IsWhiteSpace(x)));            
            int indiceUltimaPalabra=pText.LastIndexOf(" ");
            string lastWord = pText.Substring(indiceUltimaPalabra+1);
            string txt = "?o bif:contains \"'";
            if (lastWord.Length>3)
            {
                txt += pText+"*";
            }else if(indiceUltimaPalabra>-1)
            {
                txt += pText.Substring(0, indiceUltimaPalabra);
            }else
            {
                return "";
            }
            txt+= "'\" OPTION(score ?sc).";
            return txt;
        }

        public static string GetSearchBuscador(string pText)
        {
            pText=pText.Trim();
            bool busquedaExacta = pText.StartsWith("\"") && pText.EndsWith("\"");
            string txt = "?o bif:contains '";
            if (busquedaExacta)
            {
                txt += pText.Replace("\"","\\\"").Replace("'","\\'");
            }else
            {
                txt += "\"" + string.Join("\" AND \"",pText.Split(new string[] { " "},StringSplitOptions.RemoveEmptyEntries)) + "\"";
            }
            txt += "' OPTION(score ?sc).";
            return txt;
        }

        public static string GetRegexSearch(string pText)
        {
            return RemoveAccentsWithNormalization(pText).ToLower().Replace("a", "[a,á]").Replace("e", "[e,é]").Replace("i", "[i,í]").Replace("o", "[o,ó]").Replace("u", "[u,ú]");
        }

        public static string RemoveAccentsWithNormalization(string inputString)
        {
            string normalizedString = inputString.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < normalizedString.Length; i++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(normalizedString[i]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(normalizedString[i]);
                }
            }
            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }
    }
}
