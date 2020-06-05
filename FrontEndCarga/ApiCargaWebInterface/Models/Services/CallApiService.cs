using ApiCargaWebInterface.Extra.Exceptions;
using ApiCargaWebInterface.Models.Entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;

namespace ApiCargaWebInterface.Models.Services
{
    public class CallApiService : ICallService
    {
        
        public CallApiService()
        {
        }

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

        public string CallPostApi(string urlBase, string urlMethod, object item, TokenBearer token = null, bool isFile = false, string fileName = "rdfFile")
        {
            HttpContent contentData = null;
            if (!isFile)
            {
                string stringData = JsonConvert.SerializeObject(item);
                contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");
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
