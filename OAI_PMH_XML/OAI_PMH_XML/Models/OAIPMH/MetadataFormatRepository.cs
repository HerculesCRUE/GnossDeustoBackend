// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Implementación de IMetadataFormatRepository
using System.Collections.Generic;
using System.Linq;

namespace OaiPmhNet.Models.OAIPMH
{
    /// <summary>
    /// Implementación de IMetadataFormatRepository
    /// </summary>
    public class MetadataFormatRepository : IMetadataFormatRepository
    {
        private readonly Dictionary<string, MetadataFormat> _dictionary;

        /// <summary>
        /// Constructor
        /// </summary>
        public MetadataFormatRepository()
        {
            MetadataFormat xml = new MetadataFormat("XML", "", "", "");
            _dictionary = new Dictionary<string, MetadataFormat>();
            _dictionary.Add("XML", xml);
        }

        /// <summary>
        /// Obtiene un formato de metadatos a través de su prefijo
        /// </summary>
        /// <param name="prefix">Prefijo del MetadataFormat</param>
        /// <returns>Formato de metadatos</returns>
        public MetadataFormat GetMetadataFormat(string prefix)
        {
            if (_dictionary.TryGetValue(prefix, out MetadataFormat format))
                return format;
            else
                return null;
        }

        /// <summary>
        /// Otbtiene un listada con todos los formatos de metadatos
        /// </summary>
        /// <returns>Formato de metadatos</returns>
        public IEnumerable<MetadataFormat> GetMetadataFormats()
        {
            return _dictionary.Select(o => o.Value);
        }
    }
}
