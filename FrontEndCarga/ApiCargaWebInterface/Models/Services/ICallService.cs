using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services
{
    public interface ICallService
    {
        public string CallGetApi(string urlMethod);
        public string CallPostApi(string urlMethod, object item, bool isFile = false);
        public string CallPutApi(string urlMethod, object item, bool isFile = false);
        public string CallDeleteApi(string urlMethod);
    }
}
