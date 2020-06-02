using API_CARGA.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace API_CARGA.Models.Services
{
    public class CallUri
    {
        readonly TokenBearer _token;
        public CallUri(CallTokenService tokenService)
        {
            if (tokenService != null)
            {
                _token = tokenService.CallTokenOAIPMH();
            }
        }
        public byte[] GetUri(string url)
        {
            byte[] result;
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
                if (_token != null)
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"{_token.token_type} {_token.access_token}");
                }
                try
                {
                    response = client.GetAsync(url).Result;
                }
                catch (TaskCanceledException ex)
                {
                    throw new TaskCanceledException(ex.Message);
                }
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsByteArrayAsync().Result;
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
    }
}
