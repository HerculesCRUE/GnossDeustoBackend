using System;
using System.Collections.Generic;
using System.Linq;

namespace OaiPmhNet.Models.Services
{
    /// <summary>
    /// COnfiguración del OAI-PMH
    /// </summary>
    public class OAI_PMH_CVN_Config
    {

        //public string XML_CVN_Repository { get; set; }        

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
