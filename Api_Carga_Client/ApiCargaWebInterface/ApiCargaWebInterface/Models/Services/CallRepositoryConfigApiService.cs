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

        public RepositoryConfigViewModel GetRepositoryConfig(Guid id)
        {
            string result = _serviceApi.CallGetApi($"{_urlRepositoryConfigApi}/{id}");
            RepositoryConfigViewModel resultObject = JsonConvert.DeserializeObject<RepositoryConfigViewModel>(result);
            return resultObject;
        }

        public List<RepositoryConfigViewModel> GetRepositoryConfigs()
        {
            string result = _serviceApi.CallGetApi($"{_urlRepositoryConfigApi}");
            List<RepositoryConfigViewModel>  resultObject = JsonConvert.DeserializeObject<List<RepositoryConfigViewModel>>(result);
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

        public RepositoryConfigViewModel CreateRepositoryConfigView(RepositoryConfigViewModel newRepositoryConfigView)
        {
            Guid guidAdded;
            string result = _serviceApi.CallPostApi( _urlRepositoryConfigApi,newRepositoryConfigView);
            result = JsonConvert.DeserializeObject<string>(result);
            Guid.TryParse(result, out guidAdded);
            result = _serviceApi.CallGetApi($"{_urlRepositoryConfigApi}/{guidAdded}");
            RepositoryConfigViewModel resultObject = JsonConvert.DeserializeObject<RepositoryConfigViewModel>(result);
            return resultObject;
        }

        public void ModifyRepositoryConfig(RepositoryConfigViewModel repositoryConfigView)
        {
            string result = _serviceApi.CallPutApi(_urlRepositoryConfigApi, repositoryConfigView);
        }
    }
}
