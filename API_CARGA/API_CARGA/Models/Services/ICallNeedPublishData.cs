using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace API_CARGA.Models.Services
{
    public interface ICallNeedPublishData
    {
        public string CallGetApi(string urlMethod);
        public string CallPostApiFile(string urlMethod, MultipartFormDataContent item, string parameters = null);
        public void CallDataValidate(string rdf, Guid repositoryIdentifier);
        public void CallDataPublish(string rdf);
    }
}
