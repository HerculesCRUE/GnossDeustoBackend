// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Servicio para hacer llamadas al api de Shapes
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
        public CallShapeConfigApiService(ICallService serviceApi)
        {
            _serviceApi = serviceApi;
        }

        public ShapeConfigViewModel CreateShapeConfig(ShapeConfigCreateModel newRepositoryConfigView)
        {
            Guid guidAdded;
            string parameters = $"?name={newRepositoryConfigView.Name}&repositoryID={newRepositoryConfigView.RepositoryID}";

            string result = _serviceApi.CallPostApi($"{_urlShapeConfigApi}{parameters}", newRepositoryConfigView.ShapeFile, true);
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

        public void ModifyShapeConfig(ShapeConfigEditModel repositoryConfigView)
        {
            string parameters = $"?name={repositoryConfigView.Name}&repositoryID={repositoryConfigView.RepositoryID}&shapeConfigID={repositoryConfigView.ShapeConfigID}";
            string result = _serviceApi.CallPutApi($"{_urlShapeConfigApi}{parameters}", repositoryConfigView.ShapeFile, true);
        }
    }
}
