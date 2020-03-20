using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace API_CARGA.Models.Services
{
    public class OaiPublishRDFService
    {
        readonly ConfigUrlService _serviceUrl;
        public OaiPublishRDFService(ConfigUrlService serviceUrl)
        {
            _serviceUrl = serviceUrl;
        }

        public void PublishRepositories(Guid identifier)
        {
            // List<string> listIdentifier = CallListIdentifier(identifier);
            //  List<string> listRdf = CallGetRecord(identifier, listIdentifier);
            List<string> listRdf = new List<string>();
            listRdf.Add("prueba");
            CallDataPublish(listRdf, identifier);
        }

        public List<string> CallListIdentifier(Guid identifierRepo)
        {
            List<string> listIdentifier = new List<string>();
            string xml = CallGetApi($"etl/ListIdentifiers/{identifierRepo}?metadataPrefix=rdf");
            XDocument respuestaXML = XDocument.Load(xml);
            XElement listIdentifierElement = respuestaXML.Element("ListIdentifiers");
            IEnumerable<XElement> listHeader = listIdentifierElement.Descendants("header");
            foreach (var header in listHeader)
            {
                string identifier = header.Element("identifier").Value;
                listIdentifier.Add(identifier);
            }
            return listIdentifier;
        }

        public List<string> CallGetRecord(Guid repoIdentifier, List<string> listIdentifier)
        {
            List<string> listRdf = new List<string>();
            foreach (string indentifier in listIdentifier) 
            {
                string respuesta = CallGetApi($"etl/GetRecord/{repoIdentifier}?identifier={indentifier}&&metadataPrefix=rdf");
                XDocument respuestaXML = XDocument.Load(respuesta);
                listRdf.Add(respuestaXML.Element("GetRecord").Element("metadata").Value);
            }
            return listRdf;
        }

        public void CallDataPublish(List<string> listRdf, Guid identifier)
        {
            string path = $"temp/rdf/{identifier}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            foreach(string rdf in listRdf)
            {
                string fileName = $"{Guid.NewGuid().ToString()}.rdf";
                File.Create($"{path}/{fileName}").Close();
                File.WriteAllText($"{path}/{fileName}", rdf);
                var bytes = File.ReadAllBytes($"{path}/{fileName}");
                var objeto = new { rdfFile = bytes };
                CallPostApi("etl/data-publish", objeto);
            }
        }

        //http://herc-as-front-desa.atica.um.es/etl/ListIdentifiers/13131?metadataPrefix=rdf
        private string CallGetApi(string urlMethod)
        {
            string result = "";
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
                string url = _serviceUrl.GetUrl();
                response = client.GetAsync($"{url}{urlMethod}").Result;
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsStringAsync().Result;
            }
            catch (HttpRequestException)
            {
                if (!string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    throw new HttpRequestException(response.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    throw new HttpRequestException(response.ReasonPhrase);
                }
            }
            return result;
        }

        private string CallPostApi(string urlMethod, object item)
        {
            string stringData = JsonConvert.SerializeObject(item);
            var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");
            string result = "";
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
                string url = _serviceUrl.GetUrl();
                response = client.PostAsync($"{url}{urlMethod}", contentData).Result;
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            catch (HttpRequestException)
            {
                if (!string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    throw new HttpRequestException(response.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    throw new HttpRequestException(response.ReasonPhrase);
                }
            }
        }
    }
}
