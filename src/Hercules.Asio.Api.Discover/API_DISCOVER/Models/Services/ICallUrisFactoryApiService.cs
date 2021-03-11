// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Interfaz para realizar llamadas al api de uris factory
using API_DISCOVER.Models.Entities;
using Microsoft.AspNetCore.Http;

namespace API_DISCOVER.Models.Services
{
    /// <summary>
    /// Interfaz para realizar llamadas al api de uris factory
    /// </summary>
    public interface ICallUrisFactoryApiService
    {

        /// <summary>
        /// Obtiene una rui
        /// </summary>
        /// <param name="resourceClass">Resource class o rdfType</param>
        /// <param name="identifier">Identificador</param>
        /// <returns>uri</returns>
        public string GetUri(string resourceClass, string identifier);
    }
}
