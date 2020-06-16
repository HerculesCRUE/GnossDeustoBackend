// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Servicio para hacer llamadas al api de repositorios
﻿using ApiCargaWebInterface.Models.Entities;
using ApiCargaWebInterface.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services
{
    public class CallRepositoryConfigApiService : ICallRepositoryConfigService
    {
        readonly ICallService _serviceApi;
        readonly static string _urlRepositoryConfigApi = "etl-config/Repository";
        readonly TokenBearer _token;
        readonly ConfigUrlService _serviceUrl;
        public CallRepositoryConfigApiService(ICallService serviceApi, CallTokenService tokenService, ConfigUrlService serviceUrl)
        {
            _serviceUrl = serviceUrl;
            _serviceApi = serviceApi;
            if (tokenService != null) 
            {
                _token = tokenService.CallTokenCarga();
            }
        }

        public RepositoryConfigViewModel GetRepositoryConfig(Guid id)
        {
            string result = _serviceApi.CallGetApi(_serviceUrl.GetUrl(),$"{_urlRepositoryConfigApi}/{id}", _token);
            RepositoryConfigViewModel resultObject = JsonConvert.DeserializeObject<RepositoryConfigViewModel>(result);
            return resultObject;
        }

        public List<RepositoryConfigViewModel> GetRepositoryConfigs()
        {
            string result = _serviceApi.CallGetApi(_serviceUrl.GetUrl(), $"{_urlRepositoryConfigApi}", _token);
            List<RepositoryConfigViewModel>  resultObject = JsonConvert.DeserializeObject<List<RepositoryConfigViewModel>>(result);
            return resultObject;
        }

        public bool DeleteRepositoryConfig(Guid id)
        { 
            bool eliminado = false;
            string result = _serviceApi.CallDeleteApi(_serviceUrl.GetUrl(), $"{_urlRepositoryConfigApi}/{id}", _token);
            if(!string.IsNullOrEmpty(result))
            {
                eliminado = true;
            }
            return eliminado;
        }

        public RepositoryConfigViewModel CreateRepositoryConfigView(RepositoryConfigViewModel newRepositoryConfigView)
        {
            Guid guidAdded;
            string result = _serviceApi.CallPostApi(_serviceUrl.GetUrl(), _urlRepositoryConfigApi,newRepositoryConfigView, _token);
            result = JsonConvert.DeserializeObject<string>(result);
            Guid.TryParse(result, out guidAdded);
            result = _serviceApi.CallGetApi(_serviceUrl.GetUrl(), $"{_urlRepositoryConfigApi}/{guidAdded}",_token);
            RepositoryConfigViewModel resultObject = JsonConvert.DeserializeObject<RepositoryConfigViewModel>(result);
            return resultObject;
        }

        public void ModifyRepositoryConfig(RepositoryConfigViewModel repositoryConfigView)
        {
            string result = _serviceApi.CallPutApi(_serviceUrl.GetUrl(), _urlRepositoryConfigApi, repositoryConfigView, _token);
        }
    }
}
