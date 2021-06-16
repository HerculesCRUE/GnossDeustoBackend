// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase que sirve para realizar las llamadas necesarias al controlador etl para poder realizar una sincronización
using API_CARGA.Models.Entities;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace API_CARGA.Models.Services
{
    [ExcludeFromCodeCoverage]
    ///<summary>
    ///Clase que sirve para realizar las llamadas necesarias al controlador etl para poder realizar una sincronización
    ///</summary>
    public class CallApiNeedInfoPublisData: ICallNeedPublishData
    {

        readonly ConfigUrlService _serviceUrl;
        public CallApiNeedInfoPublisData(ConfigUrlService serviceUrl)
        {
            _serviceUrl = serviceUrl;
        }

        ///<summary>
        ///Realizar una llamda Get a una url que tiene como base la configurada en el appSettings.json en la propiedad ConfigUrl
        ///</summary>
        ///<param name="urlMethod">método al que se hace la llamada</param>
        /// <param name="token"></param>
        //http://herc-as-front-desa.atica.um.es/etl/ListIdentifiers/13131?metadataPrefix=rdf
        public string CallGetApi(string urlMethod, TokenBearer token = null)
        {
            string result = "";
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
                if (token != null)
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"{token.token_type} {token.access_token}");
                }
                string url = _serviceUrl.GetUrl();
                try
                {
                    string ruta = $"{url}{urlMethod}";
                    response = client.GetAsync(ruta).Result;
                }
                catch (TaskCanceledException ex)
                {
                    throw new TaskCanceledException(ex.Message);
                }
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsStringAsync().Result;
            }
            catch (HttpRequestException)
            {
                if (!string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    throw new HttpRequestException(response.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    throw new HttpRequestException(response.ReasonPhrase);
                }
            }

            return result;
        }

        ///<summary>
        ///Realizar una llamda Post al método /etl/data-publish para publicar un rdf
        ///</summary>
        ///<param name="rdf">contenido en rdf a publicar</param>
        ///<param name="jobId">En el caso de que haya sido una tarea la que ha lanzado la acción representa el identificador de la tarea</param>
        ///<param name="discoverProcessed">En el caso de que ya se haya procesado el descubrimiento se envía true y se inserta tal cual en BBDD (sin procesar descubrimiento)</param>
        ///<param name="token">Token</param>
        public void CallDataPublish(string rdf, string jobId = null, bool discoverProcessed = false, TokenBearer token = null)
        {
            var bytes = Encoding.UTF8.GetBytes(rdf);
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            multiContent.Add(new ByteArrayContent(bytes), "rdfFile", "rdfFile.rdf");
            string query = $"discoverProcessed={discoverProcessed}";
            if (!string.IsNullOrEmpty(jobId))
            {
                query = $"&jobId={jobId}";
            }           
            CallPostApiFile("etl/data-publish", multiContent, token, query);
        }

        ///<summary>
        ///Realizar una llamda Post al método /etl/data-validate para verificar un rdf
        ///</summary>
        ///<param name="rdf">contenido en rdf a publicar</param>
        ///<param name="repositoryIdentifier">Identificador del repositorio</param>
        ///<param name="token">Token de tipo Bearer para la seguridad entre apis</param>
        public void CallDataValidate(string rdf, Guid repositoryIdentifier, TokenBearer token = null)
        {
            var bytes = Encoding.UTF8.GetBytes(rdf);
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            multiContent.Add(new ByteArrayContent(bytes), "rdfFile", "rdfFile.rdf");
            string response = CallPostApiFile("etl/data-validate", multiContent, token, "repositoryIdentifier=" + repositoryIdentifier.ToString());
            ShapeReport shapeReport = JsonConvert.DeserializeObject<ShapeReport>(response);
            if (!shapeReport.conforms && shapeReport.severity == "http://www.w3.org/ns/shacl#Violation")
            {
                throw new ValidationException(/*"Se han producido errores en la validación: " + */JsonConvert.SerializeObject(shapeReport));
            }
        }



        ///<summary>
        ///Realizar una llamda Post para enviar un fichero
        ///</summary>
        ///<param name="urlMethod">método a llamar</param>
        ///<param name="item">Objeto con el fichero</param>
        ///<param name="parameters">parametros adicionales en formato queryString</param>
        ///<param name="token">Token de tipo Bearer para la seguridad entre apis</param>
        public string CallPostApiFile(string urlMethod, MultipartFormDataContent item, TokenBearer token = null, string parameters = null)
        {
            //string stringData = JsonConvert.SerializeObject(item);
            //var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");
            string result = "";
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
                if (token != null)
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"{token.token_type} {token.access_token}");
                }
                string url = _serviceUrl.GetUrl() + urlMethod;
                if (parameters != null)
                {
                    url += "?" + parameters;
                }
                response = client.PostAsync(url, item).Result;
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            catch (HttpRequestException)
            {
                if (!string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    throw new HttpRequestException(response.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    throw new HttpRequestException(response.ReasonPhrase);
                }
            }
        }
    }
}
