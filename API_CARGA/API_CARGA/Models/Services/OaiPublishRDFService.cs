using API_CARGA.Models.Entities;
using API_CARGA.Models.Transport;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        readonly EntityContext _context;
        public OaiPublishRDFService(ConfigUrlService serviceUrl, EntityContext context)
        {
            _serviceUrl = serviceUrl;
            _context = context;
        }

        public void PublishRepositories(Guid identifier, DateTime? fechaFrom = null, string set = null, string codigoObjeto = null)
        {
            List<IdentifierOAIPMH> listIdentifier = new List<IdentifierOAIPMH>();
            if (codigoObjeto == null)
            {
                listIdentifier = CallListIdentifier(identifier, set, fechaFrom);
            }
            IdentifierOAIPMH lastSyncro = null;
            try
            {
                if (codigoObjeto == null)
                {
                    foreach (IdentifierOAIPMH identifierOAIPMH in listIdentifier)
                    {
                        string rdf = CallGetRecord(identifier, identifierOAIPMH.Identifier);
                        CallDataValidate(rdf, identifier);
                        CallDataPublish(rdf);
                        lastSyncro = identifierOAIPMH;
                    }
                    if (lastSyncro != null)
                    {
                        AddSyncro(lastSyncro, set, identifier);
                    }
                }
                else
                {
                    string rdf = CallGetRecord(identifier, codigoObjeto);
                    CallDataValidate(rdf, identifier);
                    CallDataPublish(rdf);
                }

            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(codigoObjeto) && lastSyncro != null)
                {
                    AddSyncro(lastSyncro, set, identifier);
                }
                throw new Exception(ex.Message);
            }

        }

        private void AddSyncro(IdentifierOAIPMH lastSyncro, string set, Guid repositoryId)
        {
            if (_context.RepositorySync.Any(item => item.RepositoryId.Equals(repositoryId)))
            {
                if (!string.IsNullOrEmpty(set))
                {
                    RepositorySync repoSync = _context.RepositorySync.FirstOrDefault(item => item.RepositoryId.Equals(repositoryId) && item.Set == null);
                    repoSync.UltimaFechaDeSincronizacion = lastSyncro.Fecha;

                }
                else
                {
                    RepositorySync repoSync = _context.RepositorySync.FirstOrDefault(item => item.RepositoryId.Equals(repositoryId) && item.Set.Equals(set));
                    if (repoSync == null)
                    {
                        repoSync.UltimaFechaDeSincronizacion = lastSyncro.Fecha;
                    }
                    else
                    {
                        RepositorySync repoSyncAdd = new RepositorySync()
                        {
                            Id = Guid.NewGuid(),
                            RepositoryId = repositoryId,
                            Set = set,
                            UltimaFechaDeSincronizacion = lastSyncro.Fecha
                        };
                        _context.RepositorySync.Add(repoSyncAdd);
                    }
                }
            }
            else
            {
                RepositorySync repoSyncAdd = new RepositorySync()
                {
                    Id = Guid.NewGuid(),
                    RepositoryId = repositoryId,
                    Set = null,
                    UltimaFechaDeSincronizacion = lastSyncro.Fecha
                };
                _context.RepositorySync.Add(repoSyncAdd);
            }
            _context.SaveChanges();
        }

        public List<IdentifierOAIPMH> CallListIdentifier(Guid identifierRepo, string set = null, DateTime? fechaFrom = null)
        {
            string uri = $"etl/ListIdentifiers/{identifierRepo}?metadataPrefix=rdf";
            if (set != null)
            {
                uri += $"&set={set}";
            }
            if (fechaFrom != null)
            {
                DateTime until = DateTime.Now.AddYears(1);
                uri += $"&from={fechaFrom.Value.ToString("u",CultureInfo.InvariantCulture)}&until={until.ToString("u", CultureInfo.InvariantCulture)}";
            }
            List<IdentifierOAIPMH> listIdentifier = new List<IdentifierOAIPMH>();
            string xml = CallGetApi(uri);
            XDocument respuestaXML = XDocument.Load(new StringReader(xml));
            XNamespace nameSpace = respuestaXML.Root.GetDefaultNamespace();
            XElement listIdentifierElement = respuestaXML.Root.Element(nameSpace + "ListIdentifiers");
            IEnumerable<XElement> listHeader = listIdentifierElement.Descendants(nameSpace + "header");
            foreach (var header in listHeader)
            {
                string identifier = header.Element(nameSpace + "identifier").Value;
                string fecha = header.Element(nameSpace + "datestamp").Value;
                DateTime fechaSincro = DateTime.Parse(fecha).ToUniversalTime();
                IdentifierOAIPMH identifierOAIPMH = new IdentifierOAIPMH()
                {
                    Fecha = fechaSincro,
                    Identifier = identifier
                };
                listIdentifier.Add(identifierOAIPMH);
            }
            return listIdentifier;
        }

        /// <summary>
        /// Obtiene el rdf del identificador en el repositorio
        /// </summary>
        /// <param name="repoIdentifier">Identificador del repositorio</param>
        /// <param name="identifier">Identificador del elemento</param>
        /// <returns>RDF</returns>
        public string CallGetRecord(Guid repoIdentifier, string identifier)
        {
            string respuesta = CallGetApi($"etl/GetRecord/{repoIdentifier}?identifier={identifier}&&metadataPrefix=rdf");
            XDocument respuestaXML = XDocument.Parse(respuesta);
            XNamespace nameSpace = respuestaXML.Root.GetDefaultNamespace();
            string rdf = respuestaXML.Root.Element(nameSpace + "GetRecord").Descendants(nameSpace + "metadata").First().FirstNode.ToString();
            return rdf;
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
            string response= CallPostApiFile("etl/data-validate", multiContent, "repositoryIdentifier="+ repositoryIdentifier.ToString());
            ShapeReport shapeReport= JsonConvert.DeserializeObject<ShapeReport>(response);
            if (!shapeReport.conforms && shapeReport.severity == "http://www.w3.org/ns/shacl#Violation")
            {
                throw new Exception("Se han producido errores en la validación: "+ JsonConvert.SerializeObject(shapeReport));
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

        private string CallPostApiFile(string urlMethod, MultipartFormDataContent item,string parameters=null)
        {
            //string stringData = JsonConvert.SerializeObject(item);
            //var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");
            string result = "";
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
                string url = _serviceUrl.GetUrl()+ urlMethod;
                if(parameters!=null)
                {
                    url += "?"+parameters;
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
