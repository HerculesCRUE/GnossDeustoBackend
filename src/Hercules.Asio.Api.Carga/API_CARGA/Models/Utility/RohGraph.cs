// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using System;

namespace VDS.RDF
{
    public class RohGraph : Graph
    {
        /// <summary>
        /// Creates a new unused Blank Node ID and returns it.
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
