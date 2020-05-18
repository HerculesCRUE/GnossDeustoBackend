using API_CARGA.Models;
using API_CARGA.Models.Services;
using API_CARGA.Models.Transport;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Xml.Linq;
using Xunit;

namespace XUnitTestIntegracion
{
    public class UnitTest_OAIPMH
    {
        [Fact]
        public void TestGetRecord()
        {
            string url = "http://herc-as-front-desa.atica.um.es/carga/etl/GetRecord/5efac0ad-ec4e-467d-bbf5-ce3f64edb46a?identifier=1&metadataPrefix=rdf";
            string respuesta = CallGetApi(url);
            XDocument respuestaXML = XDocument.Parse(respuesta);
            XNamespace nameSpace = respuestaXML.Root.GetDefaultNamespace();
            string GetRecord = respuestaXML.Root.Element(nameSpace + "GetRecord").ToString();
            Assert.True(!string.IsNullOrEmpty(GetRecord));
        }

        [Fact]
        public void TestIdentify()
        {
            string url = "http://herc-as-front-desa.atica.um.es/carga/etl/Identify/5efac0ad-ec4e-467d-bbf5-ce3f64edb46a";
            string respuesta = CallGetApi(url);
            XDocument respuestaXML = XDocument.Parse(respuesta);
            XNamespace nameSpace = respuestaXML.Root.GetDefaultNamespace();
            string Identify = respuestaXML.Root.Element(nameSpace + "Identify").ToString();
            Assert.True(!string.IsNullOrEmpty(Identify));
        }

        [Fact]
        public void TestListIdentifiers()
        {
            string url = $"http://herc-as-front-desa.atica.um.es/carga/etl/ListIdentifiers/5efac0ad-ec4e-467d-bbf5-ce3f64edb46a?metadataPrefix=rdf&from={DateTime.Now.AddDays(-1).ToString("u")}&until={DateTime.Now.AddDays(1).ToString("u")}";
            string respuesta = CallGetApi(url);
            XDocument respuestaXML = XDocument.Parse(respuesta);
            XNamespace nameSpace = respuestaXML.Root.GetDefaultNamespace();
            string ListIdentifiers = respuestaXML.Root.Element(nameSpace + "ListIdentifiers").ToString();
            Assert.True(!string.IsNullOrEmpty(ListIdentifiers));
        }

        [Fact]
        public void TestListMetadataFormats()
        {
            string url = "http://herc-as-front-desa.atica.um.es/carga/etl/ListMetadataFormats/5efac0ad-ec4e-467d-bbf5-ce3f64edb46a";
            string respuesta = CallGetApi(url);
            XDocument respuestaXML = XDocument.Parse(respuesta);
            XNamespace nameSpace = respuestaXML.Root.GetDefaultNamespace();
            string ListMetadataFormats = respuestaXML.Root.Element(nameSpace + "ListMetadataFormats").ToString();
            Assert.True(!string.IsNullOrEmpty(ListMetadataFormats));
        }

        [Fact]
        public void TestListRecords()
        {
            string url = $"http://herc-as-front-desa.atica.um.es/carga/etl/ListRecords/5efac0ad-ec4e-467d-bbf5-ce3f64edb46a?metadataPrefix=rdf&from={DateTime.Now.AddDays(-1).ToString("u")}&until={DateTime.Now.AddDays(1).ToString("u")}";
            string respuesta = CallGetApi(url);
            XDocument respuestaXML = XDocument.Parse(respuesta);
            XNamespace nameSpace = respuestaXML.Root.GetDefaultNamespace();
            string ListRecords = respuestaXML.Root.Element(nameSpace + "ListRecords").ToString();
            Assert.True(!string.IsNullOrEmpty(ListRecords));
        }

        [Fact]
        public void TestListSets()
        {
            string url = $"http://herc-as-front-desa.atica.um.es/carga/etl/ListSets/5efac0ad-ec4e-467d-bbf5-ce3f64edb46a";
            string respuesta = CallGetApi(url);
            XDocument respuestaXML = XDocument.Parse(respuesta);
            XNamespace nameSpace = respuestaXML.Root.GetDefaultNamespace();
            string ListSets = respuestaXML.Root.Element(nameSpace + "ListSets").ToString();
            Assert.True(!string.IsNullOrEmpty(ListSets));
        }

        private string CallGetApi(string url)
        {
            string result = "";
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
                response = client.GetAsync(url).Result;
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
    }
}
