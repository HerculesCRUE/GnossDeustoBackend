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

        public void LoadTriples(List<string> triples)
        {
            _callApiService.CallPostApi(_serviceUrl.GetUrlUnidata(), "loadtriples", triples, _token);
        }
    }
}
