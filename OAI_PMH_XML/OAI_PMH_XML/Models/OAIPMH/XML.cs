// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Objeto con la información correspondiente al XML

namespace OaiPmhNet.Models.OAIPMH
{
    /// <summary>
    /// Objeto con la información correspondiente al XML
    /// </summary>
    public class XML
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pXML">XML</param>
        /// <param name="pId">Identificador</param>
        /// <param name="pSet">Set</param>
        public XML(string pXML, string pId, string pSet)
        {
            set = pSet;
            xml = pXML;
            Id = pId.ToString();           
        }

        /// <summary>
        /// XML del item
        /// </summary>
        public string xml { get; }

        /// <summary>
        /// Identificador
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Set
        /// </summary>
        public string set { get; }
    }
}
