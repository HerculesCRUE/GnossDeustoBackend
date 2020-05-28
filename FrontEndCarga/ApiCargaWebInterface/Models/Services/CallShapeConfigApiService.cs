using ApiCargaWebInterface.Models.Entities;
using ApiCargaWebInterface.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services
{
    public class CallShapeConfigApiService : ICallShapeConfigService
    {
        readonly ICallService _serviceApi;
        readonly static string _urlShapeConfigApi = "etl-config/Validation";
        readonly TokenBearer _token;
        public CallShapeConfigApiService(ICallService serviceApi, CallTokenService tokenService)
        {
            _serviceApi = serviceApi;
            if (tokenService != null)
            {
                _token = tokenService.CallTokenCarga();
            }
        }

        public ShapeConfigViewModel CreateShapeConfig(ShapeConfigCreateModel newRepositoryConfigView)
        {
            Guid guidAdded;
            string parameters = $"?name={newRepositoryConfigView.Name}&repositoryID={newRepositoryConfigView.RepositoryID}";

            string result = _serviceApi.CallPostApi($"{_urlShapeConfigApi}{parameters}", newRepositoryConfigView.ShapeFile, _token, true);
            result = JsonConvert.DeserializeObject<string>(result);
            Guid.TryParse(result, out guidAdded);
            result = _serviceApi.CallGetApi($"{_urlShapeConfigApi}/{guidAdded}", _token);
            ShapeConfigViewModel resultObject = JsonConvert.DeserializeObject<ShapeConfigViewModel>(result);
            return resultObject;
        }

        public bool DeleteShapeConfig(Guid id)
        {
            bool eliminado = false;
            string result = _serviceApi.CallDeleteApi($"{_urlShapeConfigApi}/{id}", _token);
            if (!string.IsNullOrEmpty(result))
            {
                eliminado = true;
            }
            return eliminado;
        }

        public ShapeConfigViewModel GetShapeConfig(Guid id)
        {
            string result = _serviceApi.CallGetApi($"{_urlShapeConfigApi}/{id}",_token);
            ShapeConfigViewModel resultObject = JsonConvert.DeserializeObject<ShapeConfigViewModel>(result);
            return resultObject;
        }

        public List<ShapeConfigViewModel> GetShapeConfigs()
        {
            string result = _serviceApi.CallGetApi($"{_urlShapeConfigApi}", _token);
            List<ShapeConfigViewModel> resultObject = JsonConvert.DeserializeObject<List<ShapeConfigViewModel>>(result);
            return resultObject;
        }

        public void ModifyShapeConfig(ShapeConfigEditModel repositoryConfigView)
        {
            string parameters = $"?name={repositoryConfigView.Name}&repositoryID={repositoryConfigView.RepositoryID}&shapeConfigID={repositoryConfigView.ShapeConfigID}";
            string result = _serviceApi.CallPutApi($"{_urlShapeConfigApi}{parameters}", repositoryConfigView.ShapeFile, _token, true);
        }
    }
}
