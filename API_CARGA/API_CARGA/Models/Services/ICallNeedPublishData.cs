using API_CARGA.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

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
        public string CallGetApi(string urlMethod, TokenBearer token = null);
        ///<summary>
        ///Realizar una llamda Post para enviar un fichero
        ///</summary>
        ///<param name="urlMethod">método a llamar</param>
        ///<param name="item">Objeto con el fichero</param>
        ///<param name="parameters">parametros adicionales en formato queryString</param>
        public string CallPostApiFile(string urlMethod, MultipartFormDataContent item, TokenBearer token = null, string parameters = null);
        ///<summary>
        ///Realizar una llamda Post al método /etl/data-validate para verificar un rdf
        ///</summary>
        ///<param name="rdf">contenido en rdf a publicar</param>
        ///<param name="repositoryIdentifier">Identificador del repositorio</param>
        public void CallDataValidate(string rdf, Guid repositoryIdentifier, TokenBearer token = null);
        ///<summary>
        ///Realizar una llamda Post al método /etl/data-publish para publicar un rdf
        ///</summary>
        ///<param name="rdf">contenido en rdf a publicar</param>
        public void CallDataPublish(string rdf, TokenBearer token = null);
    }
}
