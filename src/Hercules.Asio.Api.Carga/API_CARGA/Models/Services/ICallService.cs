// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Interfaz para hacer llamadas api
using API_CARGA.Models.Entities;

namespace API_CARGA.Models.Services
{
    /// <summary>
    /// Clase para hacer llamadas api
    /// </summary>
    public interface ICallService
    {
        /// <summary>
        /// Hace la llamada post a una url
        /// </summary>
        /// <param name="urlBase">Url donde se encuentra el api</param>
        /// <param name="urlMethod">url del método</param>
        /// <param name="item">objeto a pasar</param>
        /// <param name="token">token bearer en caso de que sea necesario para la autenticación</param>
        /// <param name="isFile">si el objeto pasado es un fichero</param>
        /// <param name="fileName">nombre del parametro del fichero</param>
        public string CallPostApi(string urlBase, string urlMethod, object item, TokenBearer token = null, bool isFile = false, string fileName = "rdfFile");
    }
}
