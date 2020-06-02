using API_CARGA.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace API_CARGA.Models.Services
{
    public interface ICallNeedPublishData
    {
        public string CallGetApi(string urlMethod, TokenBearer token = null);
        public string CallPostApiFile(string urlMethod, MultipartFormDataContent item, TokenBearer token = null, string parameters = null);
        public void CallDataValidate(string rdf, Guid repositoryIdentifier, TokenBearer token = null);
        public void CallDataPublish(string rdf, TokenBearer token = null);
    }
}
