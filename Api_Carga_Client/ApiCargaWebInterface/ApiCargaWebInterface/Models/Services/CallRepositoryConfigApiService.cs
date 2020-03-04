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
        public CallRepositoryConfigApiService(ICallService serviceApi)
        {
            _serviceApi = serviceApi;
        }

        public RepositoryConfigView GetRepositoryConfig(Guid id)
        {
            string result = _serviceApi.CallGetApi($"{_urlRepositoryConfigApi}/{id}");
            RepositoryConfigView resultObject = JsonConvert.DeserializeObject<RepositoryConfigView>(result);
            return resultObject;
        }

        public List<RepositoryConfigView> GetRepositoryConfigs()
        {
            string result = _serviceApi.CallGetApi($"{_urlRepositoryConfigApi}");
            List<RepositoryConfigView>  resultObject = JsonConvert.DeserializeObject<List<RepositoryConfigView>>(result);
            return resultObject;
        }

        public bool DeleteRepositoryConfig(Guid id)
        { 
            bool eliminado = false;
            string result = _serviceApi.CallDeleteApi($"{_urlRepositoryConfigApi}/{id}");
            if(!string.IsNullOrEmpty(result))
            {
                eliminado = true;
            }
            return eliminado;
        }

        public RepositoryConfigView CreateRepositoryConfigView(RepositoryConfigView newRepositoryConfigView)
        {
            Guid guidAdded;
            string result = _serviceApi.CallPostApi("etl-config/Repository",newRepositoryConfigView);
            result = JsonConvert.DeserializeObject<string>(result);
            Guid.TryParse(result, out guidAdded);
            result = _serviceApi.CallGetApi($"etl-config/Repository/{guidAdded}");
            RepositoryConfigView resultObject = JsonConvert.DeserializeObject<RepositoryConfigView>(result);
            return resultObject;
        }

        public void ModifyRepositoryConfig(RepositoryConfigView repositoryConfigView)
        {
            string result = _serviceApi.CallPutApi("etl-config/Repository",repositoryConfigView);
        }
    }
}
