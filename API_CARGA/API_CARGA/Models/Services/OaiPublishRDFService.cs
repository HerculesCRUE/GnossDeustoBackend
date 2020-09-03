// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para crear una sincronización 
using API_CARGA.Models.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace API_CARGA.Models.Services
{
    ///<summary>
    ///Clase para crear una sincronización
    ///</summary>
    public class OaiPublishRDFService
    {
        readonly EntityContext _context;
        readonly ICallNeedPublishData _publishData;
        readonly TokenBearer _token;
        public OaiPublishRDFService(EntityContext context, ICallNeedPublishData publishData, CallTokenService tokenService)
        {
            if (tokenService != null)
            {
                _token = tokenService.CallTokenCarga();
            }
            _context = context;
            _publishData = publishData;
        }

        /// <summary>
        /// Hace la sincronización del repositorio
        /// </summary>
        /// <param name="identifier">fIdentificador del repositorio</param>
        /// <param name="fechaFrom">fecha a partir de la cual se debe actualizar</param>
        /// <param name="set">tipo del objeto, usado para filtrar por agrupaciones</param>
        /// <param name="codigoObjeto">codigo del objeto a sincronizar, es necesario pasar el parametro set si se quiere pasar este parámetro</param>
        /// <param name="jobId">En el caso de que haya sido una tarea la que ha lanzado la acción representa el identificador de la tarea</param>
        ///<param name="jobCreatedDate">En el caso de que haya sido una tarea la que ha lanzado la acción representa la fecha de creación de dicha tarea</param>
        public void PublishRepositories(Guid identifier, DateTime? fechaFrom = null, string set = null, string codigoObjeto = null, string jobId = null, DateTime? jobCreatedDate = null)
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
                    int totalCount = listIdentifier.Count();
                    foreach (IdentifierOAIPMH identifierOAIPMH in listIdentifier)
                    {
                        if (!string.IsNullOrEmpty(jobId))
                        {
                            AddProcessingState(identifierOAIPMH.Identifier, identifier, jobId, listIdentifier.IndexOf(identifierOAIPMH), totalCount);
                        }
                        string rdf = CallGetRecord(identifier, identifierOAIPMH.Identifier);
                        _publishData.CallDataValidate(rdf, identifier, _token);
                        _publishData.CallDataPublish(rdf, jobId, jobCreatedDate, _token);
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
                    _publishData.CallDataValidate(rdf, identifier, _token);
                    _publishData.CallDataPublish(rdf, jobId, jobCreatedDate, _token);
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

        /// <summary>
        /// Añade un objeto de sincronización en base de datos
        /// </summary>
        /// <param name="lastSyncro">Objeto identificador de OAIPMH que contiene la fecha</param>
        /// <param name="repositoryId">Identificador del repositorio</param>
        /// <param name="set">tipo del objeto, usado para filtrar por agrupaciones</param>
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

        /// <summary>
        /// Añade o actualiza a la tabla ProcessingJobState una entrada para saber en que estado se encuentra la sincronización
        /// </summary>
        /// <param name="identifierOAIPMH">Objeto identificador de OAIPMH </param>
        /// <param name="repositoryId">Identificador del repositorio</param>
        /// <param name="jobId">Identificador de la tarea</param>
        /// <param name="index">Número del elemento que se está procesando</param>
        /// <param name="totalOfElements">Elementos a procesar</param>
        public void AddProcessingState(string identifierOAIPMH, Guid repositoryId, string jobId, int index, int totalOfElements)
        {
            var processingJobState = _context.ProcessingJobState.FirstOrDefault(item => item.JobId.Equals(jobId));
            if(processingJobState == null)
            {
                ProcessingJobState processingJobStateNew = new ProcessingJobState()
                {
                    Id = Guid.NewGuid(),
                    JobId = jobId,
                    LastIdentifierOAIPMH = identifierOAIPMH,
                    ProcessNumIdentifierOAIPMH = index,
                    RepositoryId = repositoryId,
                    TotalNumIdentifierOAIPMH = totalOfElements
                };
                _context.ProcessingJobState.Add(processingJobStateNew);
            }
            else
            {
                processingJobState.LastIdentifierOAIPMH = identifierOAIPMH;
                processingJobState.ProcessNumIdentifierOAIPMH = index;
            }
            _context.SaveChanges();
        }

        /// <summary>
        /// Obtiene una lista de identificadores
        /// </summary>
        /// <param name="identifierRepo">Identificador del repositorio</param>
        /// <param name="fechaFrom">fecha a partir de la cual se debe actualizar</param>
        /// <param name="set">tipo del objeto, usado para filtrar por agrupaciones</param>
        /// <returns>Lista de identificadores</returns>
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
            string xml = _publishData.CallGetApi(uri, _token);
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
            string respuesta = _publishData.CallGetApi($"etl/GetRecord/{repoIdentifier}?identifier={identifier}&&metadataPrefix=rdf", _token);
            XDocument respuestaXML = XDocument.Parse(respuesta);
            XNamespace nameSpace = respuestaXML.Root.GetDefaultNamespace();
            string rdf = respuestaXML.Root.Element(nameSpace + "GetRecord").Descendants(nameSpace + "metadata").First().FirstNode.ToString();
            return rdf;
        }

        
    }
}
