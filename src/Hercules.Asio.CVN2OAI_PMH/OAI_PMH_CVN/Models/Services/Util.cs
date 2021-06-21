using Newtonsoft.Json;
using OAI_PMH_CVN.Models.Services;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Hercules.Asio.CVN2OAI_PMH.Models.Services
{
    [ExcludeFromCodeCoverage]
    public class Util : IUtil
    {
        /// <summary>
        /// Obtiene los IDs de los curriculums desde una fecha de inicio
        /// </summary>
        /// <param name="pInicio">Fecha de inicio</param>
        /// <param name="pXML_CVN_Repository">Ruta del repositorio de CVN</param>
        /// <returns>Identificadores de os curriculums</returns>
        public HashSet<string> GetCurriculumsIDs(DateTime pInicio, string pXML_CVN_Repository)
        {
           
            var client = new RestClient($"{pXML_CVN_Repository}changes?date={pInicio.ToString("yyyy")}-{pInicio.ToString("MM")}-{pInicio.ToString("dd")}");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("application", "asio");
            request.AddHeader("key", "asiokey");
            XML_CVN_Repository_Response respuesta = JsonConvert.DeserializeObject<XML_CVN_Repository_Response>(client.Execute(request).Content);
            return new HashSet<string>(respuesta.ids.Select(x => x.ToString()));
            
        }
    }
}
