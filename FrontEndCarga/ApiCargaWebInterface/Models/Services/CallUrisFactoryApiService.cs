// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// clase para realizar llamadas al api de uris factory
using ApiCargaWebInterface.Models.Entities;
using ApiCargaWebInterface.ViewModels;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services
{
    /// <summary>
    /// clase para realizar llamadas al api de uris factory
    /// </summary>
    public class CallUrisFactoryApiService : ICallUrisFactoryApiService
    {
        readonly ICallService _serviceApi;
        readonly static string _urlFactory = "Factory";
        readonly static string _urlSchema = "Schema";
        readonly TokenBearer _token;
        readonly ConfigUrlService _serviceUrl;
        public CallUrisFactoryApiService(ICallService serviceApi, CallTokenService tokenService,ConfigUrlService serviceUrl)
        {
            _serviceUrl = serviceUrl;
            _serviceApi = serviceApi;
            if (tokenService != null)
            {
                _token = tokenService.CallTokenUrisFactory();
            }
        }

        /// <summary>
        /// Obtiene una rui
        /// </summary>
        /// <param name="resourceClass">Resource class o rdfType</param>
        /// <param name="identifier">Identificador</param>
        /// <param name="uriGetEnum">Configurador para indicar si el parametro pasado en resourceClass es un</param>
        /// <returns>uri</returns>
        public string GetUri(string resourceClass, string identifier, UriGetEnum uriGetEnum)
        {
            string result = _serviceApi.CallGetApi(_serviceUrl.GetUrlUrisFactory(),$"{_urlFactory}?identifier={identifier}&resource_class={resourceClass}&eleccion_uri={uriGetEnum}", _token);
            return result;
        }
        /// <summary>
        /// Obtiene el esquema de uris configurado
        /// </summary>
        /// <returns>Esquema de uris</returns>
        public string GetSchema()
        {
            string result = _serviceApi.CallGetApi(_serviceUrl.GetUrlUrisFactory(), $"{_urlSchema}", _token);
            return result;
        }
        /// <summary>
        /// Reemplaza el esquema de uris por uno nuevo
        /// </summary>
        /// <param name="newFile">Fichero  nuevo de configuración de uris</param>
        public void ReplaceSchema(IFormFile newFile)
        {
            string result = _serviceApi.CallPostApi(_serviceUrl.GetUrlUrisFactory(), $"{_urlSchema}", newFile, _token, true, "newSchemaConfig");
        }
        /// <summary>
        /// Obtiene una estructura de uris
        /// </summary>
        /// <param name="uriStructure">Nombre de la estructura</param>
        /// <returns>estructura de uris</returns>
        public string GetStructure(string uriStructure)
        {
            string result = _serviceApi.CallGetApi(_serviceUrl.GetUrlUrisFactory(), $"{_urlSchema}/{uriStructure}", _token);
            return result;
        }
        /// <summary>
        /// Elimina una estructura uri
        /// </summary>
        /// <param name="uriStructure">Nombre de la estructura</param>
        public void DeleteStructure(string uriStructure)
        {
            _serviceApi.CallDeleteApi(_serviceUrl.GetUrlUrisFactory(), $"{_urlSchema}?name={uriStructure}",_token);
        }
        /// <summary>
        /// Añade una estructura uri nueva
        /// </summary>
        /// <param name="structure">nueva estructura en formato json</param>
        public void AddStructure(string structure)
        {
            InfoUriStructureViewModel infoUriStructure = JsonConvert.DeserializeObject<InfoUriStructureViewModel>(structure);
            object item = new
            {
                infoUriStructure = infoUriStructure
            };
            string result = _serviceApi.CallPutApi(_serviceUrl.GetUrlUrisFactory(), $"{_urlSchema}", infoUriStructure, _token);
           // return result;
        }
    }
}
