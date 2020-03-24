using System;
using System.Collections.Generic;
using System.Linq;

namespace OaiPmhNet.Models.OAIPMH
{
    /// <summary>
    /// COnfiguración del OAI-PMH
    /// </summary>
    public class OAI_PMHConfig
    {
        /// <summary>
        /// Indica si soporta sets OAI-PMH
        /// </summary>
        public bool SupportSets { get; set; }

        /// <summary>
        /// Nombre del repositorio
        /// </summary>
        public string RepositoryName { get; set; }

        /// <summary>
        /// Tipo de borrado OAI-PMH
        /// </summary>
        public string DeletedRecord { get; set; }

        /// <summary>
        /// Emails de los administradores del sitio OAI-PMH
        /// </summary>
        public string[] AdminEmails { get; set; }
        
        /// <summary>
        /// Granularidad de las fechas 
        /// </summary>
        public string Granularity { get; set; }


        //public string XML_CVN_Repository { get; set; }

        /// <summary>
        /// Lista de MetadataFormat OAI-PMH disponibles
        /// </summary>
        public List<MetadataFormat> MetadataFormats { get; set; }

        /// <summary>
        /// Lista de Sets OAI-PMH disponibles
        /// </summary>
        public List<Set> Sets { get; set; }

        /// <summary>
        /// Ruta del Script de Python encargado de transformar el XML del CVN a RDF
        /// </summary>
        public string PythonScript { get; set; }

        /// <summary>
        /// Ruta del ejecutable de Pyhton
        /// </summary>
        public string PythonExe { get; set; }
    }
}
