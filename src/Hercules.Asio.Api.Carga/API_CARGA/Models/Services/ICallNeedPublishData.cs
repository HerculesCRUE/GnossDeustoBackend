// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase que sirve para realizar las llamadas necesarias al controlador etl para poder realizar una sincronización
using API_CARGA.Models.Entities;
using System;
using System.Net.Http;

namespace API_CARGA.Models.Services
{
    ///<summary>
    ///Clase que sirve para realizar las llamadas necesarias al controlador etl para poder realizar una sincronización
    ///</summary>
    public interface ICallNeedPublishData
    {
        ///<summary>
        ///Realizar una llamda Get a una url que tiene como base la configurada en el appSettings.json en la propiedad ConfigUrl
        ///</summary>
        ///<param name="urlMethod">método al que se hace la llamada</param>
        ///<param name="token">token</param>
        public string CallGetApi(string urlMethod, TokenBearer token = null);
        ///<summary>
        ///Realizar una llamda Post para enviar un fichero
        ///</summary>
        ///<param name="urlMethod">método a llamar</param>
        ///<param name="item">Objeto con el fichero</param>
        ///<param name="parameters">parametros adicionales en formato queryString</param>
        ///<param name = "token" >token</param>
        public string CallPostApiFile(string urlMethod, MultipartFormDataContent item, TokenBearer token = null, string parameters = null);
        ///<summary>
        ///Realizar una llamda Post al método /etl/data-validate para verificar un rdf
        ///</summary>
        ///<param name="rdf">contenido en rdf a publicar</param>
        ///<param name="repositoryIdentifier">Identificador del repositorio</param>
        ///<param name="token">token</param>
        public void CallDataValidate(string rdf, Guid repositoryIdentifier, TokenBearer token = null);
        ///<summary>
        ///Realizar una llamda Post al método /etl/data-publish para publicar un rdf
        ///</summary>
        ///<param name="rdf">contenido en rdf a publicar</param>
        ///<param name="jobId">En el caso de que haya sido una tarea la que ha lanzado la acción representa el identificador de la tarea</param>
        ///<param name="discoverProcessed">En el caso de que ya se haya procesado el descubrimiento se envía true y se inserta tal cual en BBDD (sin procesar descubrimiento)</param>
        ///<param name="token">token</param>
        public void CallDataPublish(string rdf, string jobId = null, bool discoverProcessed=false, TokenBearer token = null);
    }
}
