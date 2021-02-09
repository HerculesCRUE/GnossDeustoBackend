// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para hacer llamadas api de unidata
using API_CARGA.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_CARGA.Models.Services
{
    public class CallApiUnidata
    {
        readonly CallApiService _callApiService;
        readonly TokenBearer _token;
        readonly ConfigUrlService _serviceUrl;
        public CallApiUnidata(CallApiService callApiService, CallTokenService callTokenService, ConfigUrlService serviceUrl)
        {
            _callApiService = callApiService;
            _token = callTokenService.CallTokenUnidata();
            _serviceUrl = serviceUrl;
        }

        /// <summary>
        /// Carga los triples en el gráfo unidata
        /// </summary>
        /// <param name="triplesInsertar">triples a insertar</param>
        public void LoadTriples(List<string> triplesInsertar)
        {
            string triples = "";
            foreach (string triple in triplesInsertar)
            {
                triples = $"triples={triple}&";
            }
            triples = triples.Remove(triples.Length - 1);
            _callApiService.CallPostApi(_serviceUrl.GetUrlUnidata(), $"loadtriples?{triples}", null, _token);
        }
    }
}
