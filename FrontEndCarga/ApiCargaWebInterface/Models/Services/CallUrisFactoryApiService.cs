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

        public string GetUri(string resourceClass, string identifier, UriGetEnum uriGetEnum)
        {
            string result = _serviceApi.CallGetApi(_serviceUrl.GetUrlUrisFactory(),$"{_urlFactory}?identifier={identifier}&resource_class={resourceClass}&eleccion_uri={uriGetEnum}", _token);
            return result;
        }

        public string GetSchema()
        {
            string result = _serviceApi.CallGetApi(_serviceUrl.GetUrlUrisFactory(), $"{_urlSchema}", _token);
            return result;
        }

        public void ReplaceSchema(IFormFile newFile)
        {
            string result = _serviceApi.CallPostApi(_serviceUrl.GetUrlUrisFactory(), $"{_urlSchema}", newFile, _token, true, "newSchemaConfig");
        }

        public string GetStructure(string uriStructure)
        {
            string result = _serviceApi.CallGetApi(_serviceUrl.GetUrlUrisFactory(), $"{_urlSchema}/{uriStructure}", _token);
            return result;
        }

        public void DeleteStructure(string uriStructure)
        {
            _serviceApi.CallDeleteApi(_serviceUrl.GetUrlUrisFactory(), $"{_urlSchema}?name={uriStructure}",_token);
        }

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
