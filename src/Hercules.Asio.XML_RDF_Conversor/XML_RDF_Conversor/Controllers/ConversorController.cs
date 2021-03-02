using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using VDS.RDF;
using VDS.RDF.Writing;
using System.Xml;
using System.IO;
using System.Text;
using System.Net;
using System.Web;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Conversor_XML_RDF.Models.ConfigJson;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Hercules.Asio.XML_RDF_Conversor.Models.Services;
using Hercules.Asio.XML_RDF_Conversor.Models.Entities;

namespace Conversor_XML_RDF.Controllers
{
    /// <summary>
    /// Controlador encargado para hacer una conversión de ficheros XML a RDF.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ConversorController : Controller
    {
        /// <summary>
        /// Propiedad encargada de almacenar la configuración.
        /// </summary>
        public IConfiguration _configuration { get; set; }

        /// <summary>
        /// Propiedad encargada de almacenar la URL del servicio de UrisFactory.
        /// </summary>
        private static string urlUrisFactory { get; set; }

        readonly CallTokenService _tokenService;
        public ConversorController(CallTokenService tokenService, IConfiguration configuration)
        {
            _tokenService = tokenService;
            _configuration = configuration;
        }

        /// <summary>
        /// Permite visualizar una lista con los archivos de configuración JSON disponibles.
        /// </summary>
        /// <returns>Lista con los nombres de los ficheros JSON de configuración.</returns>
        [HttpPost("ConfigurationFilesList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public List<string> ConfigurationFilesList()
        {
            return ConfigFilesList();
        }

        /// <summary>
        /// Permite convertir un archivo XML a RDF.
        /// Se requiere una configuración previa del archivo JSON.
        /// </summary>
        /// <param name="pXmlFile">Archivo XML que se desea convertir.</param>
        /// <param name="pType">Nombre del JSON que se desea cargar la configuración.</param>
        /// <returns>Fichero RDF.</returns>
        [HttpPost("Convert")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Convert(IFormFile pXmlFile, string pType)
        {
            try
            {
                // Carga la URL de la configuración.
                if (string.IsNullOrEmpty(urlUrisFactory))
                {
                    urlUrisFactory = LoadUrlUrisFactoryConfig();
                }

                // Ruta del fichero de configuración JSON.
                string rutaJson = String.Empty;
                List<string> listaConfiguraciones = ConfigFilesList();
                if (listaConfiguraciones.Contains(pType))
                {
                    rutaJson = "Config/" + pType + ".json";
                }
                else
                {
                    return Problem("El fichero de configuración JSON no existe.");
                }

                // Conversión a JSON.
                ConversorConfig objJson = JsonConvert.DeserializeObject<ConversorConfig>(System.IO.File.ReadAllText(rutaJson));

                // Lectura del archivo XML.
                StringBuilder resultXml = new StringBuilder();
                using (StreamReader reader = new StreamReader(pXmlFile.OpenReadStream()))
                {
                    while (reader.Peek() >= 0)
                    {
                        resultXml.AppendLine(reader.ReadLine());
                    }
                }
                string ficheroXml = resultXml.ToString();

                // Conversión a XML.
                XmlDocument documento = new XmlDocument();
                documento.LoadXml(ficheroXml);

                // Namespace.
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(documento.NameTable);

                // Creación del RDF virtual.                
                RohGraph dataGraph = new RohGraph();
                CreateEntities(dataGraph, objJson.entities, documento, nsmgr); // Método Recursivo.

                // Consulta de comprobación.
                Object consulta = dataGraph.ExecuteQuery("SELECT * WHERE {?s ?p ?o}");

                // Creación del RDF.
                System.IO.StringWriter sw = new System.IO.StringWriter();
                RdfXmlWriter rdfXmlWriter = new RdfXmlWriter();
                rdfXmlWriter.Save(dataGraph, sw);

                // Comprobación RDF.
                string graphRDF = sw.ToString();

                // Obtener RDF.
                byte[] array = Encoding.UTF8.GetBytes(graphRDF);
                return File(array, "application/rdf+xml");
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        /// <summary>
        /// Crea todas las entidades ontológicas del XML.
        /// </summary>
        /// <param name="pDataGraph">Grafo.</param>
        /// <param name="pLista">Lista de entidades.</param>
        /// <param name="pNodo">Nodo del que se va a hacer la búsqueda.</param>
        /// <param name="pNsmgr">Namespace del nodo.</param>
        /// <returns>Lista de entidades.</returns>
        private List<string> CreateEntities(RohGraph pDataGraph, Entity[] pLista, XmlNode pNodo, XmlNamespaceManager pNsmgr)
        {
            List<string> listaEntities = new List<string>();

            if (pLista != null)
            {
                foreach (Entity entidad in pLista)
                {
                    // Si está vacío, no genera el namespace.
                    if (!string.IsNullOrEmpty(entidad.nameSpace))
                    {
                        pNsmgr.AddNamespace("ns", entidad.nameSpace);
                    }

                    XmlNodeList listaEntidades = pNodo.SelectNodes(entidad.source, pNsmgr);

                    foreach (XmlNode nodo in listaEntidades)
                    {
                        // Comprobar si un hijo en concreto está vacío o no.
                        bool comprobar = true;
                        if (!string.IsNullOrEmpty(entidad.comprobarSubentidad))
                        {
                            foreach (XmlNode child in nodo.ChildNodes)
                            {
                                if (child.Name == entidad.comprobarSubentidad && child.ChildNodes.Count <= 0)
                                {
                                    comprobar = false;
                                }
                            }
                        }

                        if (nodo.ChildNodes.Count > 0 && comprobar == true)
                        {
                            // Entidades.
                            string uriEntity = GetURI(entidad, nodo, pNsmgr); // Obtención del URI.
                            listaEntities.Add(uriEntity);

                            INode sujeto = CreateINode(uriEntity, pDataGraph); // Creación de UriNode o BlankNode.
                            INode propRdftype = pDataGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));
                            INode rdfType;

                            if (entidad.rdftype != null)
                            {
                                rdfType = pDataGraph.CreateUriNode(UriFactory.Create(entidad.rdftype));
                            }
                            else
                            {
                                if (entidad.rdftypeproperty != null)
                                {
                                    rdfType = pDataGraph.CreateUriNode(UriFactory.Create(GetTarget(entidad, nodo, pNsmgr)));
                                }
                                else
                                {
                                    throw new Exception("No se ha podido obtener el rdf:type de la entidad: " + nodo.Name);
                                }
                            }

                            pDataGraph.Assert(new Triple(sujeto, propRdftype, rdfType)); // Creación del Triple.


                            // Propiedades.
                            if (entidad.property != null)
                            {
                                INode prop = pDataGraph.CreateUriNode(UriFactory.Create(entidad.property));
                                INode val = CreateINodeType(pDataGraph, nodo.InnerText, entidad.datatype);
                                pDataGraph.Assert(new Triple(sujeto, prop, val)); // Creación del Triple.
                            }

                            if (entidad.properties != null)
                            {
                                foreach (Property propiedad in entidad.properties)
                                {
                                    string value = string.Empty;

                                    foreach (string source in propiedad.source.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries))
                                    {
                                        if (!string.IsNullOrEmpty(entidad.nameSpace))
                                        {
                                            pNsmgr.AddNamespace("ns", entidad.nameSpace);
                                        }

                                        XmlNode node = nodo.SelectSingleNode(source, pNsmgr);

                                        if (node != null)
                                        {
                                            value += " " + node.InnerText;
                                        }
                                    }

                                    value = value.Trim();

                                    if (!string.IsNullOrEmpty(value))
                                    {
                                        IUriNode predicado = pDataGraph.CreateUriNode(UriFactory.Create(propiedad.property));
                                        ILiteralNode objeto = CreateILiteralNodeType(pDataGraph, value, propiedad.datatype);
                                        pDataGraph.Assert(new Triple(sujeto, predicado, objeto)); // Creación del Triple.
                                    }
                                }
                            }

                            // Subentidades.
                            if (entidad.subentities != null)
                            {
                                foreach (Subentity subentity in entidad.subentities)
                                {
                                    List<string> listado = CreateEntities(pDataGraph, subentity.entities, nodo, pNsmgr); // Método recursivo.

                                    int numSeq = 1; // Número de la secuencia.

                                    foreach (string ent in listado)
                                    {
                                        string property = subentity.property;

                                        // Si el rdftype es de tipo secuencia, crea la propiedad + número de Seq.
                                        if (entidad.rdftype == "http://www.w3.org/1999/02/22-rdf-syntax-ns#Seq")
                                        {
                                            property = "http://www.w3.org/1999/02/22-rdf-syntax-ns#_" + numSeq;
                                        }

                                        INode objeto = CreateINode(ent, pDataGraph);

                                        // Propiedad Directa.
                                        if (!string.IsNullOrEmpty(property))
                                        {
                                            INode predicado = pDataGraph.CreateUriNode(UriFactory.Create(property));
                                            pDataGraph.Assert(new Triple(sujeto, predicado, objeto)); // Creación del Triple.
                                        }

                                        // Propiedad Inversa.
                                        if (!string.IsNullOrEmpty(subentity.inverseProperty))
                                        {
                                            INode predicadoInverso = pDataGraph.CreateUriNode(UriFactory.Create(subentity.inverseProperty));
                                            pDataGraph.Assert(new Triple(objeto, predicadoInverso, sujeto)); // Creación del Triple.
                                        }

                                        numSeq++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return listaEntities;
        }

        /// <summary>
        /// Crea un UriNode o BlankNode a partir de una entidad.
        /// </summary>
        /// <param name="pEntidad">Entidad en cuestión.</param>
        /// <param name="pDataGraph">Grafo.</param>
        /// <returns>UriNode o BlankNode.</returns>
        private INode CreateINode(string pEntidad, RohGraph pDataGraph)
        {
            if (Uri.IsWellFormedUriString(pEntidad, UriKind.Absolute))
            {
                return pDataGraph.CreateUriNode(UriFactory.Create(pEntidad)); // UriNode.
            }
            else
            {
                return pDataGraph.CreateBlankNode(pEntidad); // BlankNode.
            }
        }

        /// <summary>
        /// Crea un LiteralNode según el tipo de dato del contenido.
        /// </summary>
        /// <param name="pDataGraph">Grafo.</param>
        /// <param name="pContenido">Contenido del nodo.</param>
        /// <param name="pDatatype">Tipo del dato del contenido.</param>
        /// <returns>Nodo construido según el tipo de dato.</returns>
        private INode CreateINodeType(RohGraph pDataGraph, string pContenido, string pDatatype)
        {
            if (string.IsNullOrEmpty(pDatatype))
            {
                return pDataGraph.CreateLiteralNode(pContenido, new Uri("http://www.w3.org/2001/XMLSchema#string"));
            }
            else
            {
                return pDataGraph.CreateLiteralNode(pContenido, new Uri(pDatatype));
            }
        }

        /// <summary>
        /// Crea un LiteralNode según el tipo de dato del contenido.
        /// </summary>
        /// <param name="pDataGraph">Grafo.</param>
        /// <param name="pContenido">Contenido del nodo.</param>
        /// <param name="pDatatype">Tipo del dato del contenido.</param>
        /// <returns>Nodo construido según el tipo de dato.</returns>
        private ILiteralNode CreateILiteralNodeType(RohGraph pDataGraph, string pContenido, string pDatatype)
        {
            if (string.IsNullOrEmpty(pDatatype))
            {
                return pDataGraph.CreateLiteralNode(pContenido, new Uri("http://www.w3.org/2001/XMLSchema#string"));
            }
            else
            {
                return pDataGraph.CreateLiteralNode(pContenido, new Uri(pDatatype));
            }
        }

        /// <summary>
        /// Obtiene la URI a partir del ID.
        /// </summary>
        /// <param name="pEntidad">Entidad que se le quiera obtener el ID.</param>
        /// <param name="pNodo">Nodo dónde se almacena el ID.</param>
        /// <param name="pNsmgr">Namespace del nodo.</param>
        /// <returns>URI de la entidad.</returns>
        private string GetURI(Entity pEntidad, XmlNode pNodo, XmlNamespaceManager pNsmgr)
        {
            if (string.IsNullOrEmpty(pEntidad.id))
            {
                return Guid.NewGuid().ToString(); // Generación de GUID aleatorio.
            }
            else
            {
                if (pNodo.SelectSingleNode(pEntidad.id) != null && !string.IsNullOrEmpty(pNodo.SelectSingleNode(pEntidad.id).InnerText) && pEntidad.rdftype != null)
                {
                    return GetURL(pEntidad.rdftype, pNodo.SelectSingleNode(pEntidad.id).InnerText);
                }
                else
                {
                    if (pEntidad.rdftypeproperty != null)
                    {
                        string target = GetTarget(pEntidad, pNodo, pNsmgr);
                        if (pNodo.SelectSingleNode(pEntidad.id) != null && !string.IsNullOrEmpty(pNodo.SelectSingleNode(pEntidad.id).InnerText))
                        {
                            return GetURL(target, pNodo.SelectSingleNode(pEntidad.id).InnerText);
                        }
                        return GetURL(target, Guid.NewGuid().ToString());
                    }

                    return GetURL(pEntidad.rdftype, Guid.NewGuid().ToString());
                }
            }
        }

        /// <summary>
        /// Obtiene el target de la entidad.
        /// </summary>
        /// <param name="pEntidad">Entidad en la que se le quiere obtener el target.</param>
        /// <param name="pNodo">Nodo.</param>
        /// <param name="pNsmgr">Namespace del nodo.</param>
        /// <returns>Devuelve el tipo de rdf que es.</returns>
        private string GetTarget(Entity pEntidad, XmlNode pNodo, XmlNamespaceManager pNsmgr)
        {
            if (pEntidad.rdftypeproperty != null)
            {
                foreach (Mapping mapa in pEntidad.mappingrdftype)
                {
                    pNsmgr.AddNamespace("ns", mapa.nameSpace);
                    XmlNode node = pNodo.SelectSingleNode(pEntidad.rdftypeproperty, pNsmgr);

                    if (node.InnerText == mapa.source)
                    {
                        return mapa.target;
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// Obtiene la URI del grafo.
        /// </summary>
        /// <param name="pRdfType">Tipo del RDF.</param>
        /// <param name="pId">ID.</param>
        /// <returns>String con la URI del grafo construida.</returns>
        private string GetURL(string pRdfType, string pId)
        {
            string requestUrl = $"{urlUrisFactory}Factory?resource_class={HttpUtility.UrlEncode(pRdfType)}&identifier={HttpUtility.UrlEncode(pId)}&eleccion_uri=1";
            HttpWebRequest request = HttpWebRequest.CreateHttp(requestUrl);

            string responseBodyFromRemoteServer;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    responseBodyFromRemoteServer = reader.ReadToEnd();
                }
            }

            return responseBodyFromRemoteServer;
        }

        /// <summary>
        /// Devuelve un listado con todos los ficheros de configuración JSON disponibles.
        /// </summary>
        /// <returns>Lista con el nombre de los ficheros disponibles.</returns>
        private List<string> ConfigFilesList()
        {
            // Directorio de los ficheros de configuración.
            DirectoryInfo directorio = new DirectoryInfo("Config/");

            List<string> listaFicheros = new List<string>();

            foreach (FileInfo fichero in directorio.GetFiles())
            {
                listaFicheros.Add(fichero.Name.Substring(0, fichero.Name.IndexOf(".")));
            }

            return listaFicheros;
        }

        /// <summary>
        /// Permite cargar la URL de la configuración del fichero appsetings.json.
        /// </summary>
        /// <returns></returns>
        private string LoadUrlUrisFactoryConfig()
        {
            IDictionary environmentVariables = Environment.GetEnvironmentVariables();

            if (environmentVariables.Contains("UrlUrisFactory"))
            {
                return environmentVariables["UrlUrisFactory"] as string;
            }
            else
            {
                return _configuration["UrlUrisFactory"];
            }
        }
    }
}
