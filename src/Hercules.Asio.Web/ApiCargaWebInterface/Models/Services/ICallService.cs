// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Interfaz para llamadas api
using ApiCargaWebInterface.Models.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDS.RDF;

namespace ApiCargaWebInterface.Models.Services
{
    /// <summary>
    /// Interfaz para llamadas api
    /// </summary>
    public interface ICallService
    {

        /// /// <summary>
        /// Hace una petición get al api
        /// </summary>
        /// <param name="urlBase">Url donde se encuentra el api</param>
        /// <param name="urlMethod">Url del método dentro del api</param>
        /// <param name="token">token bearer de seguridad</param>
        public string CallGetApi(string urlBase, string urlMethod, TokenBearer token = null);
        /// <summary>
        /// Hace una petición post al api
        /// </summary>
        /// <param name="urlBase">Url donde se encuentra el api</param>
        /// <param name="urlMethod">Url del método dentro del api</param>
        /// <param name="token">token bearer de seguridad</param>
        /// <param name="item">objeto a pasar</param>
        /// <param name="isFile">si el objeto pasado es un fichero</param>
        /// <param name="fileName">nombre del parametro del fichero, en el caso de que el objeto pasado sea un fichero</param>
        public string CallPostApi(string urlBase, string urlMethod, object item, TokenBearer token = null, bool isFile = false, string fileName = "rdfFile", bool sparql = false);
        /// <summary>
        /// Hace una petición post al api
        /// </summary>
        /// <param name="urlBase">Url donde se encuentra el api</param>
        /// <param name="urlMethod">Url del método dentro del api</param>
        /// <param name="token">token bearer de seguridad</param>
        /// <param name="files">diccionario de ficheros a pasar, en el que la clave es el nombre del parametro y el valor el fichero</param>
        public string CallPostApiFiles(string urlBase, string urlMethod, Dictionary<string, IFormFile> files, TokenBearer token = null);
        /// <summary>
        /// Hace una petición put al api
        /// </summary>
        /// <param name="urlBase">Url donde se encuentra el api</param>
        /// <param name="urlMethod">Url del método dentro del api</param>
        /// <param name="token">token bearer de seguridad</param>
        ///  <param name="item">objeto a pasar</param>
        /// <param name="isFile">si el objeto pasado es un fichero</param>
        /// <param name="fileName">nombre del parametro del fichero, en el caso de que el objeto pasado sea un fichero</param>
        public string CallPutApi(string urlBase, string urlMethod, object item, TokenBearer token = null, bool isFile = false, string fileName = "rdfFile");
        /// <summary>
        /// Hace una petición delete
        /// </summary>
        /// <param name="urlBase">Url donde se encuentra el api</param>
        /// <param name="urlMethod">Url del método dentro del api</param>
        /// <param name="token">token bearer de seguridad</param>
        public string CallDeleteApi(string urlBase, string urlMethod, TokenBearer token = null);
    }
}
