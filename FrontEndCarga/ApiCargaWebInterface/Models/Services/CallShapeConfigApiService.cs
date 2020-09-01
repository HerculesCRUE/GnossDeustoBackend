// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Servicio para hacer llamadas al api de Shapes
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
    /// <summary>
    /// Servicio para hacer llamadas al api de Shapes
    /// </summary>
    public class CallShapeConfigApiService : ICallShapeConfigService
    {
        readonly ICallService _serviceApi;
        readonly static string _urlShapeConfigApi = "etl-config/Validation";
        readonly TokenBearer _token;
        readonly ConfigUrlService _serviceUrl;
        public CallShapeConfigApiService(ICallService serviceApi, CallTokenService tokenService, ConfigUrlService serviceUrl)
        {
            _serviceUrl = serviceUrl;
            _serviceApi = serviceApi;
            if (tokenService != null)
            {
                _token = tokenService.CallTokenCarga();
            }
        }

        /// <summary>
        /// Añade una configuración de validación mediante un shape SHACL.
        /// </summary>
        /// <param name="newRepositoryConfigView">Configuración de validación a añadir</param>
        /// <returns>Configuración creada</returns>
        public ShapeConfigViewModel CreateShapeConfig(ShapeConfigCreateModel newRepositoryConfigView)
        {
            Guid guidAdded;
            string parameters = $"?name={newRepositoryConfigView.Name}&repositoryID={newRepositoryConfigView.RepositoryID}";

            string result = _serviceApi.CallPostApi(_serviceUrl.GetUrl(),$"{_urlShapeConfigApi}{parameters}", newRepositoryConfigView.ShapeFile, _token, true);
            result = JsonConvert.DeserializeObject<string>(result);
            Guid.TryParse(result, out guidAdded);
            result = _serviceApi.CallGetApi(_serviceUrl.GetUrl(), $"{_urlShapeConfigApi}/{guidAdded}", _token);
            ShapeConfigViewModel resultObject = JsonConvert.DeserializeObject<ShapeConfigViewModel>(result);
            return resultObject;
        }

        /// <summary>
        /// Elimina una configuración de validación
        /// </summary>
        /// <param name="id">Identificador del shape</param>
        /// <returns>Si se ha completado con éxito</returns>
        public bool DeleteShapeConfig(Guid id)
        {
            bool eliminado = false;
            string result = _serviceApi.CallDeleteApi(_serviceUrl.GetUrl(), $"{_urlShapeConfigApi}/{id}", _token);
            if (!string.IsNullOrEmpty(result))
            {
                eliminado = true;
            }
            return eliminado;
        }

        /// <summary>
        /// Obtiene una configuración de validación
        /// </summary>
        /// <param name="id">Identificador del shape</param>
        /// <returns>configuración de validación</returns>
        public ShapeConfigViewModel GetShapeConfig(Guid id)
        {
            string result = _serviceApi.CallGetApi(_serviceUrl.GetUrl(), $"{_urlShapeConfigApi}/{id}",_token);
            ShapeConfigViewModel resultObject = JsonConvert.DeserializeObject<ShapeConfigViewModel>(result);
            return resultObject;
        }
        /// <summary>
        /// Obtiene todos las configuraciones de validación
        /// </summary>
        /// <returns>Lista de configuraciones</returns>
        public List<ShapeConfigViewModel> GetShapeConfigs()
        {
            string result = _serviceApi.CallGetApi(_serviceUrl.GetUrl(), $"{_urlShapeConfigApi}", _token);
            List<ShapeConfigViewModel> resultObject = JsonConvert.DeserializeObject<List<ShapeConfigViewModel>>(result);
            return resultObject;
        }

        /// <summary>
        /// Modifica una validación de configuración
        /// </summary>
        /// <param name="repositoryConfigView">Configuración de validación a modificar</param>
        public void ModifyShapeConfig(ShapeConfigEditModel repositoryConfigView)
        {
            string parameters = $"?name={repositoryConfigView.Name}&repositoryID={repositoryConfigView.RepositoryID}&shapeConfigID={repositoryConfigView.ShapeConfigID}";
            string result = _serviceApi.CallPutApi(_serviceUrl.GetUrl(), $"{_urlShapeConfigApi}{parameters}", repositoryConfigView.ShapeFile, _token, true);
        }
    }
}
