// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// clase para realizar llamadas al api de uris factory
using Hercules.Asio.XML_RDF_Conversor.Models.Entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hercules.Asio.XML_RDF_Conversor.Models.Services
{
    /// <summary>
    /// clase para realizar llamadas al api de uris factory
    /// </summary>
    public class CallUrisFactoryApiService 
    {
        readonly CallApiService _serviceApi;
        readonly static string _urlFactory = "Factory";
        readonly TokenBearer _token;
        readonly ConfigUrlService _serviceUrl;

        /// <summary>
        /// Servicio.
        /// </summary>
        /// <param name="serviceApi"></param>
        /// <param name="tokenService"></param>
        /// <param name="serviceUrl"></param>
        public CallUrisFactoryApiService(CallApiService serviceApi, CallTokenService tokenService,ConfigUrlService serviceUrl)
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
        /// <returns>uri</returns>
        public string GetUri(string resourceClass, string identifier)
        {
            string result = _serviceApi.CallGetApi(_serviceUrl.GetUrlUrisFactory(),$"{_urlFactory}?identifier={identifier}&resource_class={resourceClass}", _token);
            return result;
        }
    }
}
