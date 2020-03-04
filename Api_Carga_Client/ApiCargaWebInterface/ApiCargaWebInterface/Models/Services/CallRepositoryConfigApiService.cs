using ApiCargaWebInterface.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services
{
    public class CallRepositoryConfigApiService : ICallRepositoryConfigService
    {
        readonly ConfigUrlService _serviceUrl;
        public CallRepositoryConfigApiService(ConfigUrlService serviceUrl)
        {
            _serviceUrl = serviceUrl;
        }

        public RepositoryConfigView GetRepositoryConfig(Guid id)
        {
            string result = "";
            RepositoryConfigView resultObject = null;
            try
            {
                result = CallApi($"etl-config/Repository/{id}");
                resultObject = JsonConvert.DeserializeObject<RepositoryConfigView>(result);
            }
            catch (HttpRequestException e)
            {
                throw new HttpRequestException(e.Message);
            }
            return resultObject;
        }

        public List<RepositoryConfigView> GetRepositoryConfigs()
        {
            string result = "";
            List<RepositoryConfigView> resultObject = new List<RepositoryConfigView>();
            try
            {
                result = CallApi("etl-config/Repository");
                resultObject = JsonConvert.DeserializeObject<List<RepositoryConfigView>>(result);
            }
            catch (HttpRequestException e)
            {
                throw new HttpRequestException(e.Message);
            }
            return resultObject;
        }

        public bool DeleteRepositoryConfig(Guid id)
        {
            bool eliminado = false;
            string result = "";
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
                string url = _serviceUrl.GetUrl();
                response = client.DeleteAsync($"{url}etl-config/Repository/{id}").Result;
                response.EnsureSuccessStatusCode();
                eliminado = true;
            }
            catch (HttpRequestException)
            {
                throw new HttpRequestException(response.Content.ReadAsStringAsync().Result);
            }
            return eliminado;
        }

        public RepositoryConfigView CreateRepositoryConfigView(RepositoryConfigView newRepositoryConfigView)
        {
            Guid guidAdded = Guid.Empty;
            string result = "";
            string stringData = JsonConvert.SerializeObject(newRepositoryConfigView);
            var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");
            RepositoryConfigView resultObject = null;
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
                string url = _serviceUrl.GetUrl();
                response = client.PostAsync($"{url}etl-config/Repository", contentData).Result;
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsStringAsync().Result;
                result = JsonConvert.DeserializeObject<string>(result);
                Guid.TryParse(result, out guidAdded);
                result = CallApi($"etl-config/Repository/{guidAdded}");
                resultObject = JsonConvert.DeserializeObject<RepositoryConfigView>(result);
            }
            catch (HttpRequestException)
            {
                if (response.StatusCode.Equals(HttpStatusCode.BadRequest))
                {
                    throw new BadResquestException(response.Content.ReadAsStringAsync().Result);
                }
                throw new HttpRequestException(response.Content.ReadAsStringAsync().Result);
            }
            return resultObject;
        }

        private string CallApi(string urlMethod)
        {
            string result = "";
            try
            {
                HttpClient client = new HttpClient();
                string url = _serviceUrl.GetUrl();
                HttpResponseMessage response = client.GetAsync($"{url}{urlMethod}").Result;
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsStringAsync().Result;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            return result;
        }

        public void ModifyRepositoryConfig(RepositoryConfigView repositoryConfigView)
        {
            string stringData = JsonConvert.SerializeObject(repositoryConfigView);
            var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
                string url = _serviceUrl.GetUrl();
                response = client.PutAsync($"{url}etl-config/Repository", contentData).Result;
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException)
            {
                if (response.StatusCode.Equals(HttpStatusCode.BadRequest))
                {
                    throw new BadResquestException(response.Content.ReadAsStringAsync().Result);
                }
                throw new HttpRequestException(response.Content.ReadAsStringAsync().Result);
            }
        }
    }
}
