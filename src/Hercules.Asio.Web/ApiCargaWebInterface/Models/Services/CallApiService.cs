// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Servicio para hacer llamadas api
using ApiCargaWebInterface.Extra.Exceptions;
using ApiCargaWebInterface.Models.Entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace ApiCargaWebInterface.Models.Services
{
    /// <summary>
    /// Servicio para hacer llamadas a un api
    /// </summary>
    public class CallApiService : ICallService
    {
        
        public CallApiService()
        {
        }
        /// <summary>
        /// Hace una petición delete
        /// </summary>
        /// <param name="urlBase">Url donde se encuentra el api</param>
        /// <param name="urlMethod">Url del método</param>
        /// <param name="token">token bearer de seguridad</param>
        public string CallDeleteApi(string urlBase, string urlMethod, TokenBearer token = null)
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
                response = client.DeleteAsync($"{urlBase}{urlMethod}").Result;
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

        /// <summary>
        /// Hace una petición get
        /// </summary>
        /// <param name="urlBase">Url donde se encuentra el api</param>
        /// <param name="urlMethod">Url del método</param>
        /// <param name="token">token bearer de seguridad</param>
        public string CallGetApi(string urlBase, string urlMethod, TokenBearer token = null)
        {
            string result = "";
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
                if (token != null) 
                { 
                    client.DefaultRequestHeaders.Add("Authorization",$"{token.token_type} {token.access_token}");
                }
                response = client.GetAsync($"{urlBase}{urlMethod}").Result;
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsStringAsync().Result;
            }
            catch (HttpRequestException)
            {
                StringBuilder except = new StringBuilder();
                except.AppendLine($"Url del intento de llamada: {urlBase}{urlMethod} --------- error: ");
                if (!string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    except.AppendLine(response.Content.ReadAsStringAsync().Result);
                    throw new HttpRequestException(except.ToString());
                }
                else
                {
                    except.AppendLine(response.ReasonPhrase);
                    throw new HttpRequestException(except.ToString());
                }
            }
            catch(Exception ex)
            {
                StringBuilder except = new StringBuilder();
                except.AppendLine($"Url del intento de llamada: {urlBase}{urlMethod} --------- error: ");
                except.AppendLine(ex.Message);
                throw new Exception(except.ToString());
            }
            return result;
        }

        /// <summary>
        /// Hace una petición post al api
        /// </summary>
        /// <param name="urlBase">Url donde se encuentra el api</param>
        /// <param name="urlMethod">Url del método</param>
        /// <param name="token">token bearer de seguridad</param>
        /// <param name="item">objeto a pasar</param>
        /// <param name="isFile">si el objeto pasado es un fichero</param>
        /// <param name="fileName">nombre del parametro del fichero, en el caso de que el objeto pasado sea un fichero</param>
        public string CallPostApi(string urlBase, string urlMethod, object item, TokenBearer token = null, bool isFile = false, string fileName = "rdfFile", bool sparql = false)
        {
            HttpContent contentData = null;
            if (!isFile)
            {
                if (item != null)
                {
                    string stringData = JsonConvert.SerializeObject(item);
                    string contentType = "application/json";
                    if (sparql)
                    {
                        stringData = (string)item;
                        contentType = "application/x-www-form-urlencoded";
                    }
                    contentData = new StringContent(stringData, System.Text.Encoding.UTF8, contentType);
                    
                }
            }
            else
            {
                byte[] data;
                using (var br = new BinaryReader(((IFormFile)item).OpenReadStream()))
                {
                    data = br.ReadBytes((int)((IFormFile)item).OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                contentData = new MultipartFormDataContent();
                ((MultipartFormDataContent)contentData).Add(bytes, fileName, ((IFormFile)item).FileName);
            }
            string result = "";
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
                if (token != null)
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"{token.token_type} {token.access_token}");
                }
                response = client.PostAsync($"{urlBase}{urlMethod}", contentData).Result;
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            catch (HttpRequestException ex)
            {
                if (response.StatusCode.Equals(HttpStatusCode.BadRequest))
                {
                    throw new BadRequestException(response.Content.ReadAsStringAsync().Result);
                }
                else if (!string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    throw new HttpRequestException(response.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    throw new HttpRequestException(response.ReasonPhrase);
                }
            }
        }

        /// <summary>
        /// Hace una petición post
        /// </summary>
        /// <param name="urlBase">Url donde se encuentra el api/param>
        /// <param name="urlMethod">Url del método</param>
        /// <param name="token">token bearer de seguridad</param>
        /// <param name="files">diccionario de ficheros a pasar, en el que la clave es el nombre del parametro y el valor el fichero</param>
        public string CallPostApiFiles(string urlBase, string urlMethod, Dictionary<string, IFormFile> files, TokenBearer token = null)
        {
            MultipartFormDataContent contentData = contentData = new MultipartFormDataContent();
            foreach (var file in files)
            {
                IFormFile item = file.Value;
                byte[] data;
                using (var br = new BinaryReader(item.OpenReadStream()))
                {
                    data = br.ReadBytes((int)item.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                contentData.Add(bytes, file.Key, item.FileName);
            }
            string result = "";
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
                if (token != null)
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"{token.token_type} {token.access_token}");
                }
                response = client.PostAsync($"{urlBase}{urlMethod}", contentData).Result;
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            catch (HttpRequestException)
            {
                if (response.StatusCode.Equals(HttpStatusCode.BadRequest))
                {
                    throw new BadRequestException(response.Content.ReadAsStringAsync().Result);
                }
                else if (!string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    throw new HttpRequestException(response.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    throw new HttpRequestException(response.ReasonPhrase);
                }
            }
        }
        /// <summary>
        /// Hace una petición put
        /// </summary>
        /// <param name="urlBase">Url donde se encuentra el api</param>
        /// <param name="urlMethod">Url del método</param>
        /// <param name="token">token bearer de seguridad</param>
        ///  <param name="item">objeto a pasar</param>
        /// <param name="isFile">si el objeto pasado es un fichero</param>
        /// <param name="fileName">nombre del parametro del fichero, en el caso de que el objeto pasado sea un fichero</param>
        public string CallPutApi(string urlBase, string urlMethod, object item, TokenBearer token = null, bool isFile=false, string fileName = "rdfFile")
        {
            HttpContent contentData = null;
            if (!isFile)
            {
                string stringData = JsonConvert.SerializeObject(item);
                contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");
            }
            else
            {
                if (item != null)
                { 
                    byte[] data;
                    using (var br = new BinaryReader(((IFormFile)item).OpenReadStream()))
                    {
                        data = br.ReadBytes((int)((IFormFile)item).OpenReadStream().Length);
                    }
                    ByteArrayContent bytes = new ByteArrayContent(data);
                    contentData = new MultipartFormDataContent();
                    ((MultipartFormDataContent)contentData).Add(bytes, fileName, ((IFormFile)item).FileName);
                }
            }
            string result = "";
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
                if (token != null)
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"{token.token_type} {token.access_token}");
                }
                response = client.PutAsync($"{urlBase}{urlMethod}", contentData).Result;
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            catch (HttpRequestException)
            {
                if (response.StatusCode.Equals(HttpStatusCode.BadRequest))
                {
                    throw new BadRequestException(response.Content.ReadAsStringAsync().Result);
                }
                else if (!string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
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
