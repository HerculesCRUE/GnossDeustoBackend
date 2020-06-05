using ApiCargaWebInterface.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services
{
    public interface ICallService
    {
        public string CallGetApi(string urlBase, string urlMethod, TokenBearer token = null);
        public string CallPostApi(string urlBase, string urlMethod, object item, TokenBearer token = null, bool isFile = false, string fileName = "rdfFile");
        public string CallPutApi(string urlBase, string urlMethod, object item, TokenBearer token = null, bool isFile = false, string fileName = "rdfFile");
        public string CallDeleteApi(string urlBase, string urlMethod, TokenBearer token = null);
    }
}
