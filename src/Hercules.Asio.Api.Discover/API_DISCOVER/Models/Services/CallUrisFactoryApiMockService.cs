// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para gestionar los shapes en memoria
using API_DISCOVER.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace API_DISCOVER.Models.Services
{
    ///<summary>
    ///Clase mock par el UrisFactory
    ///</summary>
    public class CallUrisFactoryApiMockService : ICallUrisFactoryApiService
    {
        /// <summary>
        /// Obtiene una rui
        /// </summary>
        /// <param name="resourceClass">Resource class o rdfType</param>
        /// <param name="identifier">Identificador</param>
        /// <returns>uri</returns>
        public string GetUri(string resourceClass, string identifier)
        {
            string url = "http://graph.um.es/" + Guid.NewGuid();
            switch (resourceClass)
            {
                case "Graph":
                    url = "http://graph.um.es/graph/" + identifier;
                    break;
                case "Agent":
                    url = "http://graph.um.es/res/agent/" + identifier;
                    break;
                case "http://purl.org/roh/mirror/foaf#Organization":
                    url = "http://graph.um.es/res/foaf_organization/" + identifier;
                    break;
            }
            return url;
        }
    }
}
