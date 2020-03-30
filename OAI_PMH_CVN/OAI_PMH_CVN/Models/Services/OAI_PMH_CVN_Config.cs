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
        /// <summary>
        /// Ruta del servicio repositorio de CVN
        /// </summary>
        public string XML_CVN_Repository { get; set; }        

        /// <summary>
        /// Ruta del servicio convertidor de CVN a RDF ROH
        /// </summary>
        public string CVN_ROH_converter { get; set; }
    }
}
