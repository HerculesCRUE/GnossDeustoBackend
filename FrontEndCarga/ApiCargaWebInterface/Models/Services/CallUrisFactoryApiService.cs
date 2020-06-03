using ApiCargaWebInterface.Models.Entities;
using ApiCargaWebInterface.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services
{
    public class CallUrisFactoryApiService : ICallUrisFactoryApiService
    {
        readonly ICallService _serviceApi;
        readonly static string _urlFactory = "Factory";
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

        public string GetUri(string resourceClass, string identifier)
        {
            string result = _serviceApi.CallGetApi(_serviceUrl.GetUrlUrisFactory(),$"{_urlFactory}?identifier={identifier}&resource_class={resourceClass}", _token);
            return result;
        }
    }
}
