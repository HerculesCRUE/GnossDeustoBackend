using ApiCargaWebInterface.ViewModels;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services
{
    public class CallShapeConfigApiService : ICallShapeConfigService
    {
        readonly ICallService _serviceApi;
        readonly static string _urlShapeConfigApi = "etl-config/Validation";
        public CallShapeConfigApiService(ICallService serviceApi)
        {
            _serviceApi = serviceApi;
        }

        public ShapeConfigViewModel CreateShapeConfig(ShapeConfigViewModel newRepositoryConfigView)
        {
            Guid guidAdded;
            string result = _serviceApi.CallPostApi(_urlShapeConfigApi, newRepositoryConfigView);
            result = JsonConvert.DeserializeObject<string>(result);
            Guid.TryParse(result, out guidAdded);
            result = _serviceApi.CallGetApi($"{_urlShapeConfigApi}/{guidAdded}");
            ShapeConfigViewModel resultObject = JsonConvert.DeserializeObject<ShapeConfigViewModel>(result);
            return resultObject;
        }

        public bool DeleteShapeConfig(Guid id)
        {
            bool eliminado = false;
            string result = _serviceApi.CallDeleteApi($"{_urlShapeConfigApi}/{id}");
            if (!string.IsNullOrEmpty(result))
            {
                eliminado = true;
            }
            return eliminado;
        }

        public ShapeConfigViewModel GetShapeConfig(Guid id)
        {
            string result = _serviceApi.CallGetApi($"{_urlShapeConfigApi}/{id}");
            ShapeConfigViewModel resultObject = JsonConvert.DeserializeObject<ShapeConfigViewModel>(result);
            return resultObject;
        }

        public List<ShapeConfigViewModel> GetShapeConfigs()
        {
            string result = _serviceApi.CallGetApi($"{_urlShapeConfigApi}");
            List<ShapeConfigViewModel> resultObject = JsonConvert.DeserializeObject<List<ShapeConfigViewModel>>(result);
            return resultObject;
        }

        public string GetAccessToken(System.Threading.Tasks.Task<string> token)
        {
            //IdentityRequestTokenViewModel identityRequest = new IdentityRequestTokenViewModel()
            //{
            //    client_id = "client",
            //    client_secret = "secret",
            //    grant_type = "client_credentials",
            //    scope = "api1"
            //};

            //string result = _serviceApi.CallPostApiToken("http://localhost:56306/connect/token", identityRequest);
            //result = JsonConvert.DeserializeObject<string>(result);
            //return result;

            string result = _serviceApi.CallPostApiToken("http://localhost:56306/connect/token",token);
            List<ShapeConfigViewModel> resultObject = JsonConvert.DeserializeObject<List<ShapeConfigViewModel>>(result);
            return "";

        }

        public void ModifyShapeConfig(ShapeConfigViewModel repositoryConfigView)
        {
            string result = _serviceApi.CallPutApi(_urlShapeConfigApi, repositoryConfigView);
        }

    }
}
