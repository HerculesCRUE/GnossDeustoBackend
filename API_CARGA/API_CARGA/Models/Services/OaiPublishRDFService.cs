using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
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

        public void PublishRepositories(Guid identifier, DateTime? fechaFrom = null, string set = null, string codigoObjeto = null)
        {
            List<string> listIdentifier = CallListIdentifier(identifier,set,fechaFrom);
            if (listIdentifier.Count > 2)
            {
                listIdentifier = listIdentifier.GetRange(0, 2);
            }
            List<string> listRdf = CallGetRecord(identifier, listIdentifier);
            CallDataPublish(listRdf, identifier);
        }

        public List<string> CallListIdentifier(Guid identifierRepo, string set = null, DateTime? fechaFrom = null)
        {
            string uri = $"etl/ListIdentifiers/{identifierRepo}?metadataPrefix=rdf";
            if (set != null )
            {
                uri += $"&set={set}";
            }
            if(fechaFrom != null)
            {
                uri += $"&from={fechaFrom}";
            }
            List<string> listIdentifier = new List<string>();
            string xml = CallGetApi(uri);
            XDocument respuestaXML = XDocument.Load(new StringReader(xml));
            XNamespace nameSpace = respuestaXML.Root.GetDefaultNamespace();
            XElement listIdentifierElement = respuestaXML.Root.Element(nameSpace + "ListIdentifiers");
            IEnumerable<XElement> listHeader = listIdentifierElement.Descendants(nameSpace + "header");
            foreach (var header in listHeader)
            {
                string identifier = header.Element(nameSpace + "identifier").Value;
                string fecha = header.Element(nameSpace + "datestamp").Value;
                DateTime fechaSincro = DateTime.Parse(fecha);
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
                XDocument respuestaXML = XDocument.Parse(respuesta);
                XNamespace nameSpace = respuestaXML.Root.GetDefaultNamespace();
                listRdf.Add(respuestaXML.Root.Element(nameSpace + "GetRecord").Descendants(nameSpace + "metadata").First().FirstNode.ToString());
            }
            return listRdf;
        }

        public void CallDataPublish(List<string> listRdf, Guid identifier)
        {
            foreach(string rdf in listRdf)
            {
                var bytes = Encoding.UTF8.GetBytes(rdf);
                MultipartFormDataContent multiContent = new MultipartFormDataContent();
                multiContent.Add(new ByteArrayContent(bytes), "rdfFile", "rdfFile.rdf");
                CallPostApiFile("etl/data-publish", multiContent);
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

        private string CallPostApiFile(string urlMethod, MultipartFormDataContent item)
        {
            //string stringData = JsonConvert.SerializeObject(item);
            //var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");
            string result = "";
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
                string url = _serviceUrl.GetUrl();
                response = client.PostAsync($"{url}{urlMethod}", item).Result;
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
