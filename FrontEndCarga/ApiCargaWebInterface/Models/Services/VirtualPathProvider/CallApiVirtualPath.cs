// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Servicio para obtener las variables de configuración de urls
using ApiCargaWebInterface.Models.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services.VirtualPathProvider
{
    public class CallApiVirtualPath
    {
        readonly TokenBearer _token;
        readonly ConfigUrlService _serviceUrl;
        readonly ICallService _serviceApi;
        public CallApiVirtualPath(CallTokenService tokenService, ConfigUrlService serviceUrl, ICallService serviceApi)
        {
            _serviceUrl = serviceUrl;
            _serviceApi = serviceApi;
            if (tokenService != null)
            {
                _token = tokenService.CallTokenApiDocumentacion();
            }
        }

        public PageInfo GetPage(string route)
        {
            string result = _serviceApi.CallGetApi(_serviceUrl.GetUrlDocumentacion(), $"page?route={route}", _token);
            PageInfo resultObject = JsonConvert.DeserializeObject<PageInfo>(result);
            return resultObject;
        }

        public List<PageInfo> GetPages()
        {
            string result = _serviceApi.CallGetApi(_serviceUrl.GetUrlDocumentacion(), $"page/list", _token);
            List<PageInfo> resultObject = JsonConvert.DeserializeObject<List<PageInfo>>(result);
            return resultObject;
        }
    }
}
