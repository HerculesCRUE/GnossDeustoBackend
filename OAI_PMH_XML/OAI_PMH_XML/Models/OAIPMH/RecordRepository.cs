// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Implementación de IRecordRepository
using OAI_PMH_XML.Models.Services;
using OaiPmhNet.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace OaiPmhNet.Models.OAIPMH
{
    /// <summary>
    /// Implementación de IRecordRepository
    /// </summary>
    public class RecordRepository : IRecordRepository
    {
        private readonly IDateConverter _dateConverter;

        /// <summary>
        /// Constructor
        /// </summary>
        public RecordRepository()
        {
            _dateConverter = new DateConverter();
        }

        /// <summary>
        /// Obtiene un Record OAI-PMH
        /// </summary>
        /// <param name="identifier">Identificador del record</param>
        /// <param name="metadataPrefix">Prefijo del metadata</param>
        /// <returns></returns>
        public Record GetRecord(string identifier, string metadataPrefix)
        {
            string xmlRoute = "XML"+ Path.DirectorySeparatorChar + identifier.Substring(0, identifier.IndexOf("_"))
                + Path.DirectorySeparatorChar + identifier.Substring(identifier.IndexOf("_") + 1) + ".xml";
            if (File.Exists(xmlRoute))
            {
                Record rec = ToRecord(GetXML(xmlRoute), metadataPrefix);
                return rec;
            }
            return null;
        }

        /// <summary>
        /// Obtiene los records del repositorio en función de los argumentos pasados
        /// </summary>
        /// <param name="arguments">Parámetros de la consulta</param>
        /// <param name="resumptionToken">Token de reanudación</param>
        /// <returns></returns>
        public RecordContainer GetRecords(ArgumentContainer arguments, IResumptionToken resumptionToken = null)
        {
            RecordContainer container = new RecordContainer();
            DateTime inicio = DateTime.MinValue;
            if (_dateConverter.TryDecode(arguments.From, out DateTime from))
            {
                inicio = from;
            }

            List<XML> listxml = new List<XML>();

            Random r = new Random();

            if (arguments.Verb == OaiVerb.ListIdentifiers.ToString())
            {
                //ListIdentifiers
                HashSet<string> files = GetXMLFiles(arguments.Set);
                foreach (string file in files)
                {
                    //Cogemos aleatoriamente el 50% de los ficheros encontrados
                    if (r.Next(100) < 50)
                    {
                        listxml.Add(GetXML(file));
                    }
                }
                container.Records = listxml.Select(r => ToIdentifiersRecord(r));
            }
            else
            {
                //ListRecords
                HashSet<string> files = GetXMLFiles(arguments.Set);
                foreach (string file in files)
                {
                    //Cogemos aleatoriamente el 50% de los ficheros encontrados
                    if (r.Next(100) < 50)
                    {
                        listxml.Add(GetXML(file));
                    }
                }
                container.Records = listxml.Select(r => ToRecord(r, arguments.MetadataPrefix));

            }
            container.Records = container.Records.Where(x => x.Header.Datestamp > inicio).ToList();
            return container;
        }


        /// <summary>
        /// Obtiene los identificadores del repositorio en función de los argumentos pasados
        /// </summary>
        /// <param name="arguments">Parámetros de la consulta</param>
        /// <param name="resumptionToken">Token de reanudación</param>
        /// <returns></returns>
        public RecordContainer GetIdentifiers(ArgumentContainer arguments, IResumptionToken resumptionToken = null)
        {
            return GetRecords(arguments, resumptionToken);
        }

        /// <summary>
        /// Convierte un XML en un Record OAI-PMH sólo con cabecera
        /// </summary>
        /// <param name="pXML">XML</param>
        /// <returns>Record OAI-PMH</returns>
        private Record ToIdentifiersRecord(XML pXML)
        {
            if (pXML == null)
                return null;
            Record record = new Record()
            {
                Header = new RecordHeader()
                {
                    Identifier = pXML.Id.ToString(),
                    SetSpecs = new List<string>() { pXML.set },
                    Datestamp = DateTime.UtcNow
                }
            };

            return record;
        }

        /// <summary>
        /// Convierte un XML en un Record OAI-PMH completo
        /// </summary>
        /// <param name="pXML">XML</param>
        /// <param name="pMetadataPrefix">Prefijo de metadatos</param>
        /// <returns>Record OAI-PMH</returns>
        private Record ToRecord(XML pXML, string pMetadataPrefix)
        {
            if (pXML == null)
                return null;
            Record record = new Record()
            {
                Header = new RecordHeader()
                {
                    Identifier = pXML.Id,
                    SetSpecs = new List<string>() { pXML.set },
                    Datestamp = DateTime.UtcNow
                }
            };

            switch (pMetadataPrefix)
            {
                case "XML":
                    record.Metadata = new RecordMetadata()
                    {
                        Content = XElement.Parse(pXML.xml)
                    };
                    break;
            }
            return record;
        }

        /// <summary>
        /// Obtiene las rutas de los XML
        /// </summary>
        /// <param name="set">Ruta del XML</param>
        /// <returns>Rutas de los XML</returns>
        private HashSet<string> GetXMLFiles(string set)
        {
            HashSet<string> files = new HashSet<string>();
            if (string.IsNullOrEmpty(set))
            {
                DirectoryInfo di = new DirectoryInfo(@"XML");
                foreach (DirectoryInfo diIn in di.GetDirectories())
                {
                    if (diIn.Exists)
                    {
                        foreach (var fi in diIn.GetFiles())
                        {
                            files.Add(fi.FullName);
                        }
                    }
                }
            }
            else
            {
                DirectoryInfo di = new DirectoryInfo(@"XML"+ Path.DirectorySeparatorChar + set);
                if (di.Exists)
                {
                    foreach (var fi in di.GetFiles())
                    {
                        files.Add(fi.FullName);
                    }
                }
            }

            return files;
        }

        /// <summary>
        /// Obtiene un XML
        /// </summary>
        /// <param name="pRoute">Ruta</param>
        /// <returns></returns>
        private XML GetXML(string pRoute)
        {
            string[] routeSplit = pRoute.Split(new string[] { Path.DirectorySeparatorChar.ToString() }, StringSplitOptions.RemoveEmptyEntries);

            string set = routeSplit[routeSplit.Length - 2];
            string id = set + "_" + routeSplit[routeSplit.Length - 1];
            id = id.Substring(0,id.LastIndexOf("."));
            string xml = XElement.Load(pRoute).ToString();
            return new XML(xml, id, set);
        }

    }
}
