using API_CARGA.Models.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace API_CARGA.Models.Services
{
    public class CallApiNeedInfoPublisData: ICallNeedPublishData
    {
        readonly ConfigUrlService _serviceUrl;
        public CallApiNeedInfoPublisData(ConfigUrlService serviceUrl)
        {
            _serviceUrl = serviceUrl;
        }

        //http://herc-as-front-desa.atica.um.es/etl/ListIdentifiers/13131?metadataPrefix=rdf
        public string CallGetApi(string urlMethod)
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

        public void CallDataPublish(string rdf)
        {
            var bytes = Encoding.UTF8.GetBytes(rdf);
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            multiContent.Add(new ByteArrayContent(bytes), "rdfFile", "rdfFile.rdf");
            CallPostApiFile("etl/data-publish", multiContent);
        }

        public void CallDataValidate(string rdf, Guid repositoryIdentifier)
        {
            var bytes = Encoding.UTF8.GetBytes(rdf);
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            multiContent.Add(new ByteArrayContent(bytes), "rdfFile", "rdfFile.rdf");
            string response = CallPostApiFile("etl/data-validate", multiContent, "repositoryIdentifier=" + repositoryIdentifier.ToString());
            ShapeReport shapeReport = JsonConvert.DeserializeObject<ShapeReport>(response);
            if (!shapeReport.conforms && shapeReport.severity == "http://www.w3.org/ns/shacl#Violation")
            {
                throw new Exception("Se han producido errores en la validación: " + JsonConvert.SerializeObject(shapeReport));
            }
        }



        public string CallPostApiFile(string urlMethod, MultipartFormDataContent item, string parameters = null)
        {
            //string stringData = JsonConvert.SerializeObject(item);
            //var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");
            string result = "";
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
                string url = _serviceUrl.GetUrl() + urlMethod;
                if (parameters != null)
                {
                    url += "?" + parameters;
                }
                response = client.PostAsync(url, item).Result;
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
