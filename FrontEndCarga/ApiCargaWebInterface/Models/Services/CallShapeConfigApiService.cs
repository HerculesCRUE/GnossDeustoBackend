using ApiCargaWebInterface.ViewModels;
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

        public void ModifyShapeConfig(ShapeConfigViewModel repositoryConfigView)
        {
            string result = _serviceApi.CallPutApi(_urlShapeConfigApi, repositoryConfigView);
        }
    }
}
