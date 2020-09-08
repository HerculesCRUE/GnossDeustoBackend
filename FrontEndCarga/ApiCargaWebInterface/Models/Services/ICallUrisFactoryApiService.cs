// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Interfaz para realizar llamadas al api de uris factory
using ApiCargaWebInterface.Models.Entities;
using Microsoft.AspNetCore.Http;

namespace ApiCargaWebInterface.Models.Services
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
        /// <param name="uriGetEnum">Configurador para indicar si el parametro pasado en resourceClass es un resource class o rdf type</param>
        /// <returns>uri</returns>
        public string GetUri(string resourceClass, string identifier, UriGetEnum uriGetEnum);
        /// <summary>
        /// Obtiene el esquema de uris configurado
        /// </summary>
        /// <returns>Esquema de uris</returns>
        public string GetSchema();
        /// <summary>
        /// Reemplaza el esquema de uris por uno nuevo
        /// </summary>
        /// <param name="newFile">Fichero  nuevo de configuración de uris</param>
        public void ReplaceSchema(IFormFile newFile);
        /// <summary>
        /// Obtiene una estructura de uris
        /// </summary>
        /// <param name="uriStructure">Nombre de la estructura</param>
        /// <returns>estructura de uris</returns>
        public string GetStructure(string uriStructure);
        /// <summary>
        /// Elimina una estructura uri
        /// </summary>
        /// <param name="uriStructure">Nombre de la estructura</param>
        public void DeleteStructure(string uriStructure);
        /// <summary>
        /// Añade una estructura uri nueva
        /// </summary>
        /// <param name="structure">nueva estructura en formato json</param>
        public void AddStructure(string structure);
    }
}
