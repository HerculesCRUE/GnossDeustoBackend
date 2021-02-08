// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using System.Collections.Generic;

namespace API_DISCOVER.Models.Entities
{
    /// <summary>
    /// Datos para apoyar en la realización de la desambiguación
    /// </summary>
    public class DisambiguationData
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DisambiguationData()
        {
            properties = new List<DataProperty>();
            identifiers = new Dictionary<string, HashSet<string>>();
        }
        /// <summary>
        /// Datos de una propiedad para apoyar en la realización de la desambiguación
        /// </summary>
        public class DataProperty
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="pProperty">Propiedad para apoyar en la realización de la desambiguación</param>
            /// <param name="pValues">Valores de la propiedad para apoyar en la realización de la desambiguación</param>
            public DataProperty(Disambiguation.Property pProperty, HashSet<string> pValues)
            {
                property = pProperty;
                values = pValues;
            }
            /// <summary>
            /// Propiedad para apoyar en la realización de la desambiguación
            /// </summary>
            public Disambiguation.Property property { get; set; }
            /// <summary>
            /// Valores de la propiedad para apoyar en la realización de la desambiguación
            /// </summary>
            public HashSet<string> values { get; set; }
        }
        /// <summary>
        /// Configuración de desambiguación utilizada para apoyar en la realización de la desambiguación
        /// </summary>
        public Disambiguation disambiguation { get; set; }
        /// <summary>
        /// Identificadores para apoyar en la realización de la desambiguación, la clave es la url de la propiedad y los valores son listados con los identificadores
        /// </summary>
        public Dictionary<string, HashSet<string>> identifiers { get; set; }
        /// <summary>
        /// Propiedades utilizadas para la realización de la desambiguación
        /// </summary>
        public List<DataProperty> properties { get; set; }
    }
}
