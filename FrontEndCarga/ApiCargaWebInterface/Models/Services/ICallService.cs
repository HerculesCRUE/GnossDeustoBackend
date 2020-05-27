using ApiCargaWebInterface.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services
{
    public interface ICallService
    {
        public string CallGetApi(string urlMethod, TokenBearer token = null);
        public string CallPostApi(string urlMethod, object item, TokenBearer token = null, bool isFile = false);
        public string CallPutApi(string urlMethod, object item, TokenBearer token = null, bool isFile = false);
        public string CallDeleteApi(string urlMethod, TokenBearer token = null);
    }
}
