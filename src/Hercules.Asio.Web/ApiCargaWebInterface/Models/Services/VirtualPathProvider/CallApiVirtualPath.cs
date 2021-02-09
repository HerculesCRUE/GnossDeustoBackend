// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Servicio para obtener las variables de configuración de urls
using ApiCargaWebInterface.Models.Entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services.VirtualPathProvider
{
    public class CallApiVirtualPath
    {
        private TokenBearer _token;
        readonly ConfigUrlService _serviceUrl;
        readonly ICallService _serviceApi;
        readonly CallTokenService _tokenService;
        public CallApiVirtualPath(CallTokenService tokenService, ConfigUrlService serviceUrl, ICallService serviceApi)
        {
            _serviceUrl = serviceUrl;
            _serviceApi = serviceApi;
            _tokenService = tokenService;
        }
        private void LoadToken()
        {
            if (_token == null)
            {
                bool tokenCargado = false;
                while (!tokenCargado)
                {
                    try
                    {
                        Stopwatch sw = new Stopwatch();
                        sw.Start(); // Iniciar la medición.
                        _token = _tokenService.CallTokenApiDocumentacion();
                        tokenCargado = true;
                        sw.Stop();
                        Log.Information($"llamar al token de documentacion : {sw.Elapsed.ToString("hh\\:mm\\:ss\\.fff")}\n");
                    }
                    catch (Exception ex)
                    {
                        tokenCargado = false;
                    }
                }
            }
        }
        public PageInfo GetPage(string route)
        {
            LoadToken();
            string result = _serviceApi.CallGetApi(_serviceUrl.GetUrlDocumentacion(), $"page?route={route}", _token);
            PageInfo resultObject = JsonConvert.DeserializeObject<PageInfo>(result);
            return resultObject;
        }

        public List<PageInfo> GetPages()
        {
            LoadToken();
            string result = _serviceApi.CallGetApi(_serviceUrl.GetUrlDocumentacion(), $"page/list", _token);
            List<PageInfo> resultObject = JsonConvert.DeserializeObject<List<PageInfo>>(result);
            return resultObject;
        }

        public void CreatePage(Guid pageId,string route, IFormFile pageHtml)
        {
            LoadToken();
            string method = $"page/load?route={route}";
            if (!Guid.Empty.Equals(pageId))
            {
                method += $"&pageId={pageId}";
            }
            if (pageHtml != null)
            {
                _serviceApi.CallPostApi(_serviceUrl.GetUrlDocumentacion(), method, pageHtml, _token, true, "html_page");
            }
            else
            {
                _serviceApi.CallPostApi(_serviceUrl.GetUrlDocumentacion(), method, pageHtml, _token);
            }
            
        }

        public void DeletePage(Guid pageId)
        {
            LoadToken();
            _serviceApi.CallDeleteApi(_serviceUrl.GetUrlDocumentacion(), $"page/delete?pageId={pageId}",_token);
        }
    }
}
