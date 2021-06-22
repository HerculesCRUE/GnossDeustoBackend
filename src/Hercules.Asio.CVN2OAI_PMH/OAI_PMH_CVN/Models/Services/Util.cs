using Newtonsoft.Json;
using OAI_PMH_CVN.Models.Services;
using OaiPmhNet.Models.OAIPMH;
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
            HashSet<string> x = new HashSet<string>();
            x.Add("1");
            x.Add("2");
            return x;
            var client = new RestClient($"{pXML_CVN_Repository}changes?date={pInicio.ToString("yyyy")}-{pInicio.ToString("MM")}-{pInicio.ToString("dd")}");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("application", "asio");
            request.AddHeader("key", "asiokey");
            XML_CVN_Repository_Response respuesta = JsonConvert.DeserializeObject<XML_CVN_Repository_Response>(client.Execute(request).Content);
            return new HashSet<string>(respuesta.ids.Select(x => x.ToString()));
            
        }

        /// <summary>
        /// Obtiene un CVN
        /// </summary>
        /// <param name="pId">Identificador</param>
        /// <param name="pOnlyIDs">Obtiene solo el CVN con el IDID</param>
        /// <param name="pXML_CVN_Repository">Ruta del repositorio de CVN</param>
        /// <returns></returns>
        public string GetCurriculum(string pId, bool pOnlyIDs, string pXML_CVN_Repository)
        {
            string xml = "";
            if (!pOnlyIDs)
            {
                var client = new RestClient($"{pXML_CVN_Repository}cvn?id={pId}");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("application", "asio");
                request.AddHeader("key", "asiokey");
                xml = client.Execute(request).Content;
            }
            return xml;
        }
    }
}
