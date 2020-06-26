using API_CARGA.Models.Entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace API_CARGA.Models.Services
{
    public class CallApiService : ICallService
    {
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
