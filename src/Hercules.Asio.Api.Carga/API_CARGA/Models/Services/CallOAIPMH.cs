// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para hacer llamadas a las urls configuradas que pertenecen al OAIPMH
using API_CARGA.Models.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace API_CARGA.Models.Services
{
    [ExcludeFromCodeCoverage]
    public class CallOAIPMH
    {
        readonly TokenBearer _token;
        public CallOAIPMH(CallTokenService tokenService)
        {
            if (tokenService != null)
            {
                _token = tokenService.CallTokenOAIPMH();
            }
        }
        /// <summary>
        /// Obtiene el resultado
        /// </summary>
        /// <param name="url">url a llamar</param>
        /// <returns></returns>
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
