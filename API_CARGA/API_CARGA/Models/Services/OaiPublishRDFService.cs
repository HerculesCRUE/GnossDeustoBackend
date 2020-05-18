using API_CARGA.Models.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;

namespace API_CARGA.Models.Services
{
    public class OaiPublishRDFService
    {
        readonly ConfigUrlService _serviceUrl;
        readonly EntityContext _context;
        readonly ICallNeedPublishData _publishData;
        public OaiPublishRDFService(ConfigUrlService serviceUrl, EntityContext context, ICallNeedPublishData publishData)
        {
            _serviceUrl = serviceUrl;
            _context = context;
            _publishData = publishData;
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
                        _publishData.CallDataValidate(rdf, identifier);
                        _publishData.CallDataPublish(rdf);
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
                    _publishData.CallDataValidate(rdf, identifier);
                    _publishData.CallDataPublish(rdf);
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
            string xml = _publishData.CallGetApi(uri);
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
            string respuesta = _publishData.CallGetApi($"etl/GetRecord/{repoIdentifier}?identifier={identifier}&&metadataPrefix=rdf");
            XDocument respuestaXML = XDocument.Parse(respuesta);
            XNamespace nameSpace = respuestaXML.Root.GetDefaultNamespace();
            string rdf = respuestaXML.Root.Element(nameSpace + "GetRecord").Descendants(nameSpace + "metadata").First().FirstNode.ToString();
            return rdf;
        }

        
    }
}
