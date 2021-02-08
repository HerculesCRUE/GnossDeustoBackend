// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para llamar a los métodos que ofrece el controlador etl del API_CARGA 
using Linked_Data_Server.Extra.Exceptions;
using Linked_Data_Server.Models.Entities;
using Linked_Data_Server.ViewModels;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;
using VDS.RDF;

namespace Linked_Data_Server.Models.Services
{
    /// <summary>
    /// Clase para llamar a los métodos que ofrece el controlador etl del API_CARGA 
    /// </summary>
    public class CallEtlApiService
    {
        readonly ConfigUrlService _serviceUrl;
        readonly TokenBearer _token;

        static RohGraph ontologia;
        static string hash;

        public CallEtlApiService(ConfigUrlService serviceUrl, CallTokenService tokenService)
        {
            _serviceUrl = serviceUrl;
            if (tokenService != null)
            {
                _token = tokenService.CallTokenCarga();
            }
        }

        /// <summary>
        /// Comprueba si la ontología ha cambiado. Si es así devuelve la nueva.
        /// </summary>
        /// <returns>La ontología actualizada</returns>
        public RohGraph CallGetOntology()
        {
            string response = CallGetApi($"etl/getontologyhash", _token);
            if (response == hash)
            {
                return ontologia;
            }
            else
            {
                string response2 = CallGetApi($"etl/getontology", _token);
                ontologia = new RohGraph();
                ontologia.LoadFromString(response2);
                hash = response;
                return ontologia;
            }
        }

        /// <summary>
        /// Hace una petición get al apiCarga
        /// </summary>
        /// <param name="urlMethod">Url del método dentro del apiCron</param>
        /// <param name="token">token bearer de seguridad</param>
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

                string url = _serviceUrl.GetUrlCarga();
                response = client.GetAsync($"{url}{urlMethod}").Result;
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
    }
}
