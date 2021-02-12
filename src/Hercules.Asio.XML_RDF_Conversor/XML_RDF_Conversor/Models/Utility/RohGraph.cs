// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using System;

namespace VDS.RDF
{
    /// <summary>
    /// Grafo.
    /// </summary>
    public class RohGraph : Graph
    {
        /// <summary>
        /// Crea un nuevo Blank Node ID sin usar y lo devuelve.
        /// </summary>
        /// <returns></returns>
        public override String GetNextBlankNodeID()
        {
            String id = Guid.NewGuid().ToString();
            id="N" + id.Replace("-", "");
            return id;
        }
    }
}
